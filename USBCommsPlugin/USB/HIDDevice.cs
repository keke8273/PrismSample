using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using DataLinkLayer.Diagnostics;
using DataLinkLayer.IO;
using DataLinkLayer.IO.Protocol;
using Microsoft.Win32.SafeHandles;
using System.Runtime.InteropServices;

namespace USBCommsPlugin.USB
{
    /// <summary>
    /// Class used to encapsulate an underlying USB HID device.
    /// </summary>
    public class HIDDevice: Win32Usb, IDisposable
    {
        #region Constructor

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="devPath">The full USB device path of the device</param>
        public HIDDevice(string devPath)
        {
            DevicePath = devPath;
            Initialise();
        }

        #endregion Constructor

        #region Private Data

        /// <summary>File stream we can use to read/write from</summary>
        private FileStream m_oFile;

        /// <summary>Length of input report : device gives us this</summary>
        private int m_nInputReportLength;

        /// <summary>Length if output report : device gives us this</summary>
        private int m_nOutputReportLength;

        /// <summary>Handle to the device</summary>
        private IntPtr m_hHandle;

        /// <summary>
        /// The HID string length
        /// </summary>
        private readonly int m_hid_string_len = 100;
       
        #endregion

        #region Public Data
        

        public static readonly int ReportReservedLen = 1;

        /// <summary>
        /// Event for when data is received on the USB
        /// </summary>
        public event EventHandler<DataRecievedEventArgs> DataRecieved;

        /// <summary>
        /// The path to the underlying USB device.
        /// </summary>
        public string DevicePath;

        /// <summary>
        /// Event handler called when device has had an error
        /// </summary>
        public event EventHandler OnDeviceError;

        /// <summary>
        /// Accessor for output report length
        /// </summary>
        public int OutputReportLength
        {
            get
            {
                return m_nOutputReportLength;
            }
        }

        /// <summary>
        /// Accessor for input report length
        /// </summary>
        public int InputReportLength
        {
            get
            {
                return m_nInputReportLength;
            }
        }

        #endregion Public Data

        #region Private Methods

        /// <summary>
        /// Initialises the device
        /// </summary>
        private void Initialise()
        {
            // Create the file from the device path
            m_hHandle = AquireFileHandle(DevicePath);

            if (m_hHandle != InvalidHandleValue || m_hHandle == null)	// if the open worked...
            {
                IntPtr lpData;
                if (HidD_GetPreparsedData(m_hHandle, out lpData))	// get windows to read the device data into an public buffer
                {
                    try
                    {
                        HidCaps oCaps;
                        HidP_GetCaps(lpData, out oCaps);	// extract the device capabilities from the public buffer
                        m_nInputReportLength = oCaps.InputReportByteLength;	// get the input...
                        m_nOutputReportLength = oCaps.OutputReportByteLength;	// ... and output report lengths

                        //Set the data sizes in the protocol
                        DeviceProtocol.Write_Block_Size = m_nOutputReportLength - ReportReservedLen;
                        DeviceProtocol.Read_Block_Size = m_nInputReportLength - ReportReservedLen;

                        m_oFile = new FileStream(new SafeFileHandle(m_hHandle, false), FileAccess.Read | FileAccess.Write, m_nInputReportLength, true);

                        Logger.LogMessage(Logger.UsbSwitch, TraceLevel.Info, oCaps.ToString());

                        BeginAsyncRead();	// kick off the first asynchronous read                              
                    }
                    catch (Exception)
                    {
                        throw HIDDeviceException.GenerateWithWinError("Failed to get the detailed data from the hid.");
                    }
                    finally
                    {
                        HidD_FreePreparsedData(ref lpData);	// before we quit the function, we must free the public buffer reserved in GetPreparsedData
                    }
                }
                else	// GetPreparsedData failed? Chuck an exception
                {
                    throw HIDDeviceException.GenerateWithWinError("GetPreparsedData failed");
                }
            }
            else	// File open failed? Chuck an exception
            {
                m_hHandle = IntPtr.Zero;
                throw HIDDeviceException.GenerateWithWinError("Failed to create device file");
            }
        }

        /// <summary>
        /// Helper method to acquire a low level USB device file handle. This will make multiple attempts to acquire the device 
        /// file handle with a sleep of 20ms between each attempt in case something else such as an anti-virus program has
        /// the file locked when the create file method is called this will allow them to complete their processing and release
        /// the file handle.
        /// </summary>
        /// <param name="devicePath">The USB device path to acquire a file handle for</param>
        /// <returns>an IntPtr containing either InvalidHandleValue if it fails to acquire the file handle or
        /// a handle to the actual device file</returns>
        private IntPtr AquireFileHandle(string devicePath)
        {
            var loopCount = 0;
            var MaxRetry = 50;
            var aquired = false;
            var handle = InvalidHandleValue;
            var aquisitionStatus = "FAILED";

            while ((loopCount < MaxRetry) && (!aquired))
            {
                handle = CreateFile(devicePath,
                                  GENERIC_READ | GENERIC_WRITE,
                                  FILE_SHARE_READ | FILE_SHARE_WRITE | FILE_SHARE_DELETE,
                                  IntPtr.Zero,
                                  OPEN_EXISTING,
                                  FILE_FLAG_OVERLAPPED,
                                  IntPtr.Zero);

                if (handle == InvalidHandleValue)
                {
                    aquisitionStatus = "FAILED"; 
                    Thread.Sleep(20);
                }
                else
                {
                    aquisitionStatus = "SUCCEEDED"; 
                    aquired = true;
                }
                loopCount++;

                Logger.LogMessage(Logger.UsbSwitch,
                                  TraceLevel.Info,
                                  string.Format("Attempt {0} of {2} to aquire a USB device file handle {1}", loopCount, aquisitionStatus, MaxRetry));
            }
            
            return handle;
        }

        /// <summary>
        /// Kicks off an asynchronous read which completes when data is read or when the device
        /// is disconnected. Uses a callback.
        /// </summary>
        private void BeginAsyncRead()
        {
            if (m_oFile != null)
            {
                var arrInputReport = new byte[m_nInputReportLength];
                // put the buff we used to receive the stuff as the async state then we can get at it when the read completes


                Logger.LogMessage(Logger.UsbSwitch,
                                  TraceLevel.Verbose,
                                  "Beginning Async read");

                var res = m_oFile.BeginRead(arrInputReport, 0, m_nInputReportLength, new AsyncCallback(ReadCompleted), arrInputReport);

                if (res == null)
                {
                    throw new HIDDeviceException("Problem occured starting the read");
                }
            }
            else
            {
                Logger.LogMessage(Logger.UsbSwitch, 
                                  TraceLevel.Warning,
                                  "Unable to begin read");
            }
        }

        /// <summary>
        /// Callback for above. Care with this as it will be called on the background thread from the async read
        /// </summary>
        /// <param name="iResult">Async result parameter</param>
        private void ReadCompleted(IAsyncResult iResult)
        {
            var arrBuff = (byte[])iResult.AsyncState;	// retrieve the read buffer
            try
            {
                if (m_oFile != null)
                {

                    //only do this if the file still exists if it doesn't it has either been removed or disposed of
                    //in which case there is nothing to read anyway.    
                    var result = m_oFile.EndRead(iResult);	// call end read : this throws any exceptions that happened during the read
                    Logger.LogMessage(Logger.UsbSwitch,
                                      TraceLevel.Verbose,
                                      "Async read ended and returned " + result.ToString());
                    try
                    {
                        Report oInRep = new InputReport(this, InputReportLength);	// Create the input report for the device
                        oInRep.SetBuffer(arrBuff,0, arrBuff.Length);	// and set the data portion - this processes the data received into a more easily understood format depending upon the report type
                        
                        Logger.LogMessage(Logger.UsbSwitch,
                                          TraceLevel.Info,
                                          "USB Report received " + arrBuff.Length.ToString());
                        
                        HandleDataReceived(oInRep);	// pass the new input report on to the higher level handler
                    }
                    finally
                    {
                        BeginAsyncRead();	// when all that is done, kick off another read for the next report
                    }
                }
            }
            catch (IOException ex)	// if we got an IO exception, the device was removed
            {
                Logger.LogException(Logger.UsbSwitch, ex, "HID_Proteus.ReadCompleted()");

                if (OnDeviceError != null)
                {
                    OnDeviceError(this, new EventArgs());
                }
                Dispose();
            }
        }

        /// <summary>
        /// handler for any action to be taken when data is received.
        /// </summary>
        /// <param name="oInRep">The input report that was received</param>
        private void HandleDataReceived(Report oInRep)
        {
            // Fire the event handler if assigned
            if (DataRecieved != null)
            {
                DataRecieved(this, new DataRecievedEventArgs(oInRep.Buffer));
            }
        }

      

        #endregion

        #region Public Methods

        /// <summary>
        /// Write an output report to the device.
        /// </summary>
        /// <param name="oOutRep">Output report to write</param>
        public void Send(Report oOutRep)
        {
            try
            {
                Logger.LogMessage(Logger.UsbSwitch, TraceLevel.Info, "HID Proteus sending data ");

                m_oFile.Write(oOutRep.Buffer, 0, oOutRep.BufferLength);
            }
            catch (IOException ex)
            {
                var msg = "An issue occured sending the report";
                Logger.LogException(Logger.UsbSwitch, ex, msg);

                // The device was removed!
                if (OnDeviceError != null)
                {
                    OnDeviceError(this, new EventArgs());
                }
            }
            catch (Exception exx)
            {
                Logger.LogException(Logger.UsbSwitch, exx, "");
            }
        }

        public bool GetProductString(out string ProductString)
        {
            var ok = false;
            ProductString = null;

            if (m_hHandle != InvalidHandleValue || m_hHandle == null)	// if the open worked...
            {
                var buff = Marshal.AllocCoTaskMem(m_hid_string_len);

                if (HidD_GetProductString(m_hHandle, buff, m_hid_string_len))
                {
                    try
                    {
                        var ProdStringChars = new char[m_hid_string_len];
                        Marshal.Copy(buff, ProdStringChars, 0, m_hid_string_len);
                        
                        ProductString = new string(ProdStringChars);
                        ProductString = ProductString.Substring(0, ProductString.IndexOf((char)0));
                        ok = true;
                        
                    }
                    catch (Exception)
                    {
                        throw HIDDeviceException.GenerateWithWinError("Error parsing the Product String from the hid.");
                    }

                }
                else
                {
                    throw HIDDeviceException.GenerateWithWinError("Failed to get the Product String from the hid.");
                }
            }
            return ok;
        }

        public bool GetManufacturerString(out string ManufacturerString)
        {
            var ok = false;
            ManufacturerString = null;

            if (m_hHandle != InvalidHandleValue || m_hHandle == null)	// if the open worked...
            {

                var buff = Marshal.AllocCoTaskMem(m_hid_string_len);

                if (HidD_GetManufacturerString(m_hHandle, buff, m_hid_string_len))
                {
                    try
                    {
                        var ManuString = new char[m_hid_string_len];

                        Marshal.Copy(buff, ManuString, 0, m_hid_string_len);

                        ManufacturerString = new string(ManuString);
                        ManufacturerString = ManufacturerString.Substring(0, ManufacturerString.IndexOf((char)0));
                        ok = true;
                    }
                    catch (Exception)
                    {
                        throw HIDDeviceException.GenerateWithWinError("Error parsing the Manufacturer String from the hid.");
                    }

                }
                else
                {
                    throw HIDDeviceException.GenerateWithWinError("Failed to get the Manufacturer String from the hid.");
                }
            }
            return ok;
        }

        public bool GetSerialNumberString(out string SerialNumberString)
        {
            var ok = false;
            SerialNumberString = null;

            if (m_hHandle != InvalidHandleValue || m_hHandle == null)	// if the open worked...
            {

                var buff = Marshal.AllocCoTaskMem(m_hid_string_len);

                if (HidD_GetSerialNumberString(m_hHandle, buff, m_hid_string_len))
                {
                    try
                    {
                        var SerialNumberChars = new char[m_hid_string_len];

                        Marshal.Copy(buff, SerialNumberChars, 0, m_hid_string_len);

                        SerialNumberString = new string(SerialNumberChars);
                        SerialNumberString = SerialNumberString.Substring(0, SerialNumberString.IndexOf((char)0));
                        ok = true;
                    }
                    catch (Exception)
                    {
                        throw HIDDeviceException.GenerateWithWinError("Error parsing the Serial Number String from the hid.");
                    }

                }
                else
                {
                    throw HIDDeviceException.GenerateWithWinError("Failed to get the Serial Number String from the hid.");
                }
            }
            return ok;
        }

        public bool GetHidDAttributes(out HIDAttributes HidAttr)
        {
            var ok = false;
            HidAttr = null;

            if (m_hHandle != InvalidHandleValue || m_hHandle == null)	// if the open worked...
            {
                HidAttr = new HIDAttributes();
               
                var HidAttrPtr = Marshal.AllocCoTaskMem(HidAttr.DataSize());

                Marshal.Copy(HidAttr.ToArray(), 0, HidAttrPtr, HidAttr.DataSize());
                
                if (HidD_GetAttributes(m_hHandle, HidAttrPtr))
                {
                    try
                    {
                        var HidAttrBytes = new byte[HidAttr.DataSize()];

                        Marshal.Copy(HidAttrPtr, HidAttrBytes, 0, HidAttr.DataSize());

                        HidAttr.FromArray(HidAttrBytes, 0);

                        ok = true;
                    }
                    catch (Exception)
                    {
                        throw HIDDeviceException.GenerateWithWinError("Error parsing HID Attributes.");
                    }
                }
                else
                {
                    throw HIDDeviceException.GenerateWithWinError("Failed to get the HID Attributes.");
                }
            }
            return ok;

        }

        #endregion Public Methods

        #region IDisposable Members

        bool disposed;

        /// <summary>
        /// Dispose method
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
        }
        /// <summary>
        /// Disposer called by both dispose and finalise
        /// </summary>
        /// <param name="bDisposing">True if disposing</param>
        protected virtual void Dispose(bool bDisposing)
        {
            try
            {
                if (!disposed)
                {
                    disposed = true;
                    if (bDisposing)	// if we are disposing, need to close the managed resources
                    {
                        if (m_oFile != null)
                        {
                            m_oFile.Close();
                            m_oFile = null;
                            GC.SuppressFinalize(this);
                        }
                    }
                    if (m_hHandle != IntPtr.Zero)	// Dispose and finalize, get rid of unmanaged resources
                    {
                        try
                        {
                            if (CloseHandle(m_hHandle) == 0)
                            {
                                throw new HIDDeviceException("m_hHandle didn't close correctly");
                            }
                        }
                        catch (Exception ex)
                        {
                            Logger.LogException(Logger.UsbSwitch, ex, "");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.LogMessage(Logger.UsbSwitch, TraceLevel.Warning, ex.ToString());
            }
        }
        #endregion
    }
    
    /// <summary>
    /// Event args class used when data is received on the USB
    /// </summary>
    public class DataRecievedEventArgs : EventArgs
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="data">The data that has been received</param>
        public DataRecievedEventArgs(byte[] data)
        {
            this.data = data;
        }

        public readonly byte[] data;
    }
}
