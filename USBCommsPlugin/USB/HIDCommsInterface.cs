using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using DataLinkLayer.Diagnostics;
using DataLinkLayer.IO;
using DataLinkLayer.Utils;

namespace USBCommsPlugin.USB
{
    /// <summary>
    /// Implementation of the comms interface that gives access to USB Proteus and Rubix Meters.
    /// </summary>
    public class HIDCommsInterface : Win32Usb, ICommsInterface
    {
        #region Constructor 

        /// <summary>
        /// Constructor
        /// </summary>
        public HIDCommsInterface()
        {
            devListener = new DeviceConnectionListener(HandleConnection);
            devices = new List<string>();
        }

        #endregion Constructor

        #region Private Data
        
        /// <summary>
        /// List of device connection strings used to keep track of which devices are
        /// currently connected.
        /// </summary>
        List<string> devices;

        /// <summary>
        /// Object used to listen for device connection events
        /// </summary>
        private DeviceConnectionListener devListener;

        /// <summary>
        /// Property used to flag if the interface has been initialised.
        /// </summary>
        /// <remarks>Initialization occurs after the during the first call to GetAvailablePorts</remarks>
        private bool Initialised
        {
            get;
            set;
        }

        private readonly object _lock = new object();

        #endregion Private Data

        #region Public Data 
        
        /// <summary>
        /// The Vendor ID of the devices we are interested in.
        /// </summary>
        public const int VID = 0x1111;

        /// <summary>
        /// The Proteus HID Product ID
        /// </summary>
        public const int ProteusPID = 0x000A;

        /// <summary>
        /// The Rubix HID Product ID
        /// </summary>
        public const int RubixPID = 0x0014;
        
        #endregion Public Data

        #region Private methods

        /// <summary>
        /// Helper method to get a device path from the underlying Windows interface
        /// </summary>
        /// <returns>The device connection string</returns>
        private string GetDevicePath(IntPtr hInfoSet, ref DeviceInterfaceData oInterface, ref DeviceInfoData oDeviceInfoData )
        {
            uint nRequiredSize = 0;
            string targetPath = null;

            // Get the device interface details
            if (!SetupDiGetDeviceInterfaceDetail(hInfoSet, ref oInterface, IntPtr.Zero, nRequiredSize, ref nRequiredSize, IntPtr.Zero))
            {
                var oDetail = new DeviceInterfaceDetailData();

                oDetail.Size = 5;     	// hardcoded to 5! Sorry, but this works and trying more future proof versions by setting the size to the struct sizeof failed miserably. If you manage to sort it...
                if (SetupDiGetDeviceInterfaceDetail(hInfoSet, ref oInterface, ref oDetail, nRequiredSize, ref nRequiredSize, ref oDeviceInfoData))
                {
                    targetPath = oDetail.DevicePath;
                }
            }
            return targetPath;
        }
        /// <summary>
        /// Method used as a delegate for the Device Listener to perform its action when an 
        /// device connection event occurs.
        /// </summary>
        /// <param name="deviceStatus">
        /// false if a device has been removed otherwise true if a new connection has been made.
        /// </param>
        private void HandleConnection(bool deviceStatus)
        {
            lock (_lock)
            {
                var currentDevices = (this as ICommsInterface).GetAvailablePorts();
                if ((devices != null) && (devices.Count != currentDevices.Count))
                {
                    List<string> newDevices = null;
                    if (deviceStatus == true)
                    {
                        Logger.LogMessage(Logger.UsbSwitch, TraceLevel.Info, "Comms interface connecting devices");
                        //device have been connected work out which ones
                        newDevices = new List<string>(currentDevices.Except(devices));
                    }
                    else
                    {
                        Logger.LogMessage(Logger.UsbSwitch, TraceLevel.Info, "Comms interface disconnecting devices");
                        //devices removed so work out which ones
                        newDevices = new List<string>(devices.Except(currentDevices));
                    }

                    foreach (var item in newDevices)
                    {
                        Logger.LogMessage(Logger.UsbSwitch, TraceLevel.Warning, string.Format("{0} => {1}", item, deviceStatus));
                        (this as IPublisher).NotifySubscribers(new CommsInterfaceEventArgs(item, deviceStatus));
                    }
                }
                //update the device list to the most recent
                devices = currentDevices;
            }
        }

        private bool isInstalled(IntPtr hDeviceInfo, DeviceInfoData da)
        {
            var installed = false;

            if (hDeviceInfo != InvalidHandleValue)
            {
                // now we can get some more detailed information
                UInt32 requiredSize = 0;

                // get the Device Description and DriverKeyName
                UInt32 RegType;
                var buff = new byte[BUFFER_SIZE];

                if (SetupDiGetDeviceRegistryProperty(hDeviceInfo, ref da, SPDRP_INSTALL_STATE, out RegType, buff, BUFFER_SIZE, out requiredSize))
                {
                    var status = BitConverter.ToInt64(buff, 0);

                    if (status == 0)
                    {
                        installed = true;
                    }
                }

            }

            return installed;
        }


        #endregion Private methods

        #region ICommsInterface Members

        List<string> ICommsInterface.GetAvailablePorts()
        {
            var strPath = string.Empty;

            var proSearchPattern = string.Format("vid_{0:x4}&pid_{1:x4}", VID, ProteusPID); // first, build the path search string
            var rubSearchPattern = string.Format("vid_{0:x4}&pid_{1:x4}", VID, RubixPID); // first, build the path search string
            var devPaths = new List<string>();

            var gHid = HIDGuid;
            
            // this gets a list of all HID devices currently connected to the computer (InfoSet)
            var hInfoSet = SetupDiGetClassDevs(ref gHid, null, IntPtr.Zero, DIGCF_DEVICEINTERFACE | DIGCF_PRESENT);
            try
            {
                // build up a device interface data block
                var oInterface = new DeviceInterfaceData();
                oInterface.Size = Marshal.SizeOf(oInterface);

                var oDeviceInfoData = new DeviceInfoData();
                oDeviceInfoData.Size = Marshal.SizeOf(oDeviceInfoData);

                // Now iterate through the InfoSet memory block assigned within Windows in the call to SetupDiGetClassDevs
                // to get device details for each device connected
                var nIndex = 0;

                // this gets the device interface information for a device at index 'nIndex' in the memory block
                // and check to see if it matches the correct VID, PID combination
                while (SetupDiEnumDeviceInterfaces(hInfoSet, 0, ref gHid, (uint)nIndex, ref oInterface))
                {
                    // get the device path (see helper method 'GetDevicePath')
                    var strDevicePath = GetDevicePath(hInfoSet, ref oInterface, ref oDeviceInfoData);

                    // do a string search, if we find the VID/PID string then we found a device!
                    if ((strDevicePath.IndexOf(proSearchPattern) >= 0) || (strDevicePath.IndexOf(rubSearchPattern) >= 0))
                    {
                        // Check device is installed
                        if (isInstalled(hInfoSet, oDeviceInfoData))
                        {
                            //add it to the list of available ports
                            devPaths.Add(strDevicePath);
                        }
                        else
                        {
                            Logger.LogMessage(Logger.UsbSwitch, TraceLevel.Warning, "Device Driver not installed");
                        }
                    }
                    nIndex++;
                }
                var lastErr = Marshal.GetLastWin32Error();
            }
            catch (Exception ex)
            {
                throw HIDDeviceException.GenerateError(ex.ToString());
                //Console.WriteLine(ex.ToString());
            }
            finally
            {
                // Before we go, we have to free up the InfoSet memory reserved by SetupDiGetClassDevs
                SetupDiDestroyDeviceInfoList(hInfoSet);
            }

            if (!Initialised)
            {
                Initialised = true;
                devices = devPaths;
            }
            return devPaths;
        }

        bool ICommsInterface.IsValidPortName(string connectioString)
        {
            return true;
        }

        IIOPort ICommsInterface.GetPort(string connectionString)
        {
            IIOPort newPort = new HIDIOPort(connectionString);

            return newPort;
        }

        #endregion

        #region IPublisher Members

        /// <summary>
        /// Event backing the publisher interface.
        /// </summary>
        private event EventHandler<CommsInterfaceEventArgs> _connectionEvent;

        /// <summary>
        /// Register interest in receiving event from this comms interface
        /// </summary>
        /// <param name="Subscriber">The subscriber interface used to notify the client of events</param>
        void IPublisher.RegisterSubscriber<TSubscriber>(TSubscriber Subscriber)
        {
            _connectionEvent += new EventHandler<CommsInterfaceEventArgs>(Subscriber.OnNotification);
        }

        /// <summary>
        /// Un-register interest in receiving event from this comms interface
        /// </summary>
        /// <param name="Subscriber">The subscriber interface used to notify the client of events</param>
        void IPublisher.UnregisterSubscriber<TSubscriber>(TSubscriber Subscriber)
        {
            _connectionEvent -= new EventHandler<CommsInterfaceEventArgs>(Subscriber.OnNotification);
        }

        /// <summary>
        /// Notify clients(subscribers) of an event
        /// </summary>
        /// <param name="args">The argument object to sent with the event</param>
        void IPublisher.NotifySubscribers(EventArgs args)
        {
            if (_connectionEvent != null)
            {
                _connectionEvent(this, args as CommsInterfaceEventArgs);
            }
        }

        #endregion

        #region IDisposable Members

        private bool _disposed;

        private void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    devListener.Dispose();
                    GC.SuppressFinalize(this);
                }
                _disposed = true;
            }
        }

        void IDisposable.Dispose()
        {
            Dispose(true);
        }

        ~HIDCommsInterface()
        {
            Dispose(false);
        }

        #endregion

        #region Device Connection Listener

        /// <summary>
        /// Delegate used to signal a device connection message has been received.
        /// </summary>
        /// <param name="deviceStatus">true if the device connection message is indicating a device has been connected
        /// otherwise it will be false indicating that a removal has occurred.</param>
        private delegate void DeviceConnectedDelegate(bool deviceStatus);

        /// <summary>
        /// A class used to listen for Windows System Device connection events.
        /// </summary>
        private class DeviceConnectionListener : Form
        {
            /// <summary>
            /// Constructor
            /// </summary>
            /// <param name="connectionHandler">The delegate used to signal that a connection message has been received.</param>
            public DeviceConnectionListener(DeviceConnectedDelegate connectionHandler)
            {
                //register with the system to receive connection/removal notification for all HID devices
                RegisteredHandle = RegisterForUsbEvents(Handle, HIDGuid);
                _handler = connectionHandler;
            }

            /// <summary>
            /// The handler delegate called when a connection message is received.
            /// </summary>
            private DeviceConnectedDelegate _handler;

            private IntPtr RegisteredHandle;

            /// <summary>
            /// The Windows message handler that will receive the OS messages related to device connection events.
            /// </summary>
            /// <param name="m"></param>
            protected override void WndProc(ref Message m)
            {
                base.WndProc(ref m);

                if (m.Msg == WM_DEVICECHANGE)	// we got a device change message! A USB device was inserted or removed
                {
                    switch (m.WParam.ToInt32())	// Check the W parameter to see if a device was inserted or removed
                    {
                        case DEVNODES_CHANGED:
                            Logger.LogMessage(Logger.UsbSwitch, TraceLevel.Info, "DEVNODES_CHANGED");
                            break;
                        
                        case DEVICE_ARRIVAL:	// inserted
                            Logger.LogMessage(Logger.UsbSwitch, TraceLevel.Info, "DEVICE_ARRIVAL");
                            
                            _handler(true);
                            break;

                        case DEVICE_REMOVECOMPLETE:	// removed
                            Logger.LogMessage(Logger.UsbSwitch, TraceLevel.Info, "DEVICE_REMOVECOMPLETE");

                            _handler(false);
                            break;

                        default:
                            Logger.LogMessage(Logger.UsbSwitch, TraceLevel.Warning, string.Format("Unhandled WM_DEVICECHANGE param = {0}", m.WParam.ToInt32()));
                            break;
                    }
                }
            }

            /// <summary>
            /// Disposer
            /// </summary>
            /// <param name="disposing">Flag used to determine if we are disposing of managed resources as well.</param>
            protected override void Dispose(bool disposing)
            {
                base.Dispose(disposing);
                
                if (IsDisposed)
                {
                    //deregister from the connection/removal system event notification
                    UnregisterForUsbEvents(RegisteredHandle);
                }

            }
        }

        #endregion Device Connection Listener
    }
} 


