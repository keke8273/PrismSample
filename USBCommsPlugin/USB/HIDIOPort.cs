using System;
using System.Collections.Generic;
using System.Diagnostics;
using DataLinkLayer.Diagnostics;
using DataLinkLayer.IO;

namespace USBCommsPlugin.USB
{
    /// <summary>
    /// Class that wraps a HID USB device interface with a serial port like interface.
    /// </summary>
    public class HIDIOPort: IIOPort
    {
        #region Constructors

        /// <summary>
        /// Constructor to setup the port to be backed by a particular device, as identified by
        /// the device path.
        /// </summary>
        /// <param name="devicePath">Device path of the port you want to back the IOport.</param>
        public HIDIOPort(string devicePath)
        {
            Vid = Convert.ToInt32(devicePath.Substring(devicePath.IndexOf("vid_") + 4, 4), 16);
            Pid = Convert.ToInt32(devicePath.Substring(devicePath.IndexOf("&pid_") + 5, 4), 16);

            _devicePath = devicePath;
            _receivedData = new List<byte>();
            _lock = new object();
        }

        #endregion

        #region Private Data

        /// <summary>
        /// event to store listeners to the DataReceived events.
        /// </summary>
        private event PortDataReceivedEventHandler DataReceivedHandlers;

        private event EventHandler DeviceError;

        /// <summary>
        /// Reference to the underlying device object controlled by this port.
        /// </summary>
        private HIDDevice _specficDevice;

        /// <summary>
        /// List of the data that has been received
        /// </summary>
        private List<byte> _receivedData;

        /// <summary>
        /// A simple lock primitive used to protect shared resources.
        /// </summary>
        private object _lock;

        /// <summary>
        /// cache used to store the device path of the underlying HID device
        /// </summary>
        private string _devicePath;

        private string _productString;

        private string _manufacturerString;

        private string _serialNumberString;

        private HIDAttributes _hidAttributes;



        #endregion

        #region Public Data

        /// <summary>
        /// The Vender ID of the underlying device.
        /// </summary>
        public int Vid { get; private set; }

        /// <summary>
        /// The Product ID of the underlying device.
        /// </summary>
        public int Pid { get; private set; }
        
        #endregion

        #region Private Methods


        /// <summary>
        /// Event handler for the asynchronous data received event. This event comes from 
        /// the lower level device. This event handler will store the data that has been 
        /// received and notify listeners that data is available. 
        /// </summary>
        /// <param name="sender">Originator of the event (the low level device)</param>
        /// <param name="args">Event specific data this contains the data that was read.</param>
        private void _asyncDataRecieved(object sender, DataRecievedEventArgs args)
        {
            var data = args.data;

            lock (_lock)
            {
                //cache the data that has been read
                lock (_receivedData)
                {
                    //copy the report data minus the report tag into the public buffer
                    for (var i = 1; i < data.Length; i++)
                    {
                        _receivedData.Add(data[i]);
                    }
                }
                //make sure there are some listeners
                if (DataReceivedHandlers != null)
                {
                    //send an event to inform listeners that data is available
                    DataReceivedHandlers(this, new PortDataReceivedEventArgs(_receivedData.Count));
                }
            }
        }

        /// <summary>
        /// Event handler for an underlying device error
        /// </summary>
        /// <param name="sender">should be the underlying port</param>
        /// <param name="e">event arguments</param>
        void _specficDevice_OnDeviceError(object sender, EventArgs e)
        {
            Logger.LogMessage(Logger.UsbSwitch, TraceLevel.Warning, "USB device error detected");

            if (DeviceError != null)
            {
                DeviceError(this, e);
            }
        }

        /// <summary>
        /// Helper method to create an output report and send it to the device.
        /// </summary>
        /// <param name="buffer">The data buffer containing data to be sent</param>
        /// <param name="offset">The offset into the buffer byte array to get the data from</param>
        /// <param name="count">The number of bytes to put into the output report starting at 
        /// offset</param>
        private void _fileReport(byte[] buffer, int offset, int count)
        {
            var outputRep = new Report(_specficDevice, _specficDevice.OutputReportLength);

            outputRep.SetBuffer(buffer, offset, count);
            _specficDevice.Send(outputRep);
        }

        #endregion Private Methods

        #region IIOPort Members

        /// <summary>
        /// Concrete implementation of the IOPort interface event <see cref="ProteusCore.Data.IO.IOPort.DataReceived" />
        /// </summary>
        event PortDataReceivedEventHandler IIOPort.DataReceived
        {
            add
            {
                //lock to make sure modification to the event handlers are atomic
                lock (_lock)
                {
                    //add the new handler
                    DataReceivedHandlers += value;
                }
            }
            remove 
            {
                //lock to make sure modification to the event handlers are atomic
                lock (_lock)
                {
                    //remove the handler
                    DataReceivedHandlers -= value;
                }
            }
        }

        event EventHandler IIOPort.PortDeviceError
        {
            add
            {
                //lock to make sure modification to the event handlers are atomic
                lock (_lock)
                {
                    //add the new handler
                    DeviceError += value;
                }
            }
            remove
            {
                //lock to make sure modification to the event handlers are atomic
                lock (_lock)
                {
                    //remove the handler
                    DeviceError -= value;

                }
            }
        }

        /// <summary>
        /// Implementation of the interface for close.
        /// </summary>
        /// <remarks>
        /// When closed the USB port will clear data received event listeners and any cached 
        /// data. 
        /// </remarks>
        void IIOPort.Close()
        {
            //clear the list of listeners
            DataReceivedHandlers = null;
            
            //clear the cached data
            _receivedData.Clear();

            if (_specficDevice != null)
            {
                _specficDevice.Dispose();
                //make sure the device is now null
                _specficDevice = null;
            }
        }

        /// <summary>
        /// Concrete implementation of the interface for opening a port.
        /// </summary>
        /// <exception cref="UsbLibrary.HIDDeviceException">
        /// Thrown if unable to open the device for any reason.
        /// </exception>
        void IIOPort.Open()
        {
            //only open the device if it is not already open which can be assumed if the _specific device isn't null
            if (_specficDevice == null)
            {
                _specficDevice = new HIDDevice(_devicePath);

                if (_specficDevice != null)
                {
                    //register in data received events with the lower level device 
                    _specficDevice.DataRecieved += new EventHandler<DataRecievedEventArgs>(_asyncDataRecieved);
                    _specficDevice.OnDeviceError += new EventHandler(_specficDevice_OnDeviceError);

                     if(!_specficDevice.GetHidDAttributes(out _hidAttributes))
                     {
                        throw HIDDeviceException.GenerateError("Could not get HID Attributes");
                     }

                     if (!_specficDevice.GetProductString(out _productString))
                     {
                         throw HIDDeviceException.GenerateError("Could not get Product String");
                     }

                     if (!_specficDevice.GetManufacturerString(out _manufacturerString))
                     {
                         throw HIDDeviceException.GenerateError("Could not get Manufacturer String");
                     }

                     if (!_specficDevice.GetSerialNumberString(out _serialNumberString))
                     {
                         throw HIDDeviceException.GenerateError("Could not get Serial Number String");
                     }

                }
                else
                {
                    throw HIDDeviceException.GenerateError("Unable to open the usb device");
                }
            }
        }

        /// <summary>
        /// Concrete implementation of the IOPort interface method <see cref="ProteusCore.Data.IO.IOPort.Read"/>
        /// </summary>
        /// <param name="buffer">an array the data will be read into</param>
        /// <param name="offset">an offset into the array that the data will start to be read into</param>
        /// <param name="count">number of bytes to read.</param>
        /// <returns>The number of bytes that were read.</returns>
        /// <remarks>
        /// The read will consume bytes from the input buffer. The actual number of bytes consumed is
        /// returned as the number of bytes read.
        /// </remarks>
        int IIOPort.Read(byte[] buffer, int offset, int count)
        {
            List<byte> readData = null;

            //make sure there is enough room to store the data
            if ( (buffer.Length < count)
              || ((buffer.Length - offset) < count) )
            {
                throw HIDDeviceException.GenerateError("buffer is to small to hold the requested read");
            }

            lock (_receivedData)
            {
                //if there is some data, copy the requested amount into the buffer
                readData = _receivedData.GetRange(offset, count);
                _receivedData.RemoveRange(offset, count);

                if (readData != null)
                {
                    Array.Copy(readData.ToArray(), buffer, readData.Count);
                }
            }

            return readData.Count;
        }

        /// <summary>
        /// Concrete implementation of the IOPort interface method <see cref="ProteusCore.Data.IO.IOPort.Write"/>
        /// </summary>
        /// <param name="buffer">an array of data that will be written</param>
        /// <param name="offset">an offset into the array that the data will start the write from</param>
        /// <param name="count">number of bytes to write.</param>
        void IIOPort.Write(byte[] buffer, int offset, int count)
        {
            if (_specficDevice != null)
            {
                var reportPayloadLength = Report.PayloadSize(_specficDevice.OutputReportLength);
                //workout the number of full reports there will be
                var number_of_reports = (count - offset) / reportPayloadLength;
                
                var bufferPos = offset;

                //send the full reports
                for (var i = 0; i < number_of_reports; i++)
                {
                    _fileReport(buffer, bufferPos, reportPayloadLength);
                    bufferPos += reportPayloadLength;
                }

                //send anything that is left
                if (bufferPos < buffer.Length)
                {
                    _fileReport(buffer, bufferPos, buffer.Length - bufferPos);
                }
            }
        }

        /// <summary>
        /// Concrete implementation of the IOPort interface method <see cref="ProteusCore.Data.IO.IOPort.DiscardInBuffer"/>
        /// </summary>
        void IIOPort.DiscardInBuffer()
        {
            lock (_lock)
            {
                _receivedData.Clear();
            }
        }

        /// <summary>
        /// Concrete implementation of the IOPort interface method <see cref="ProteusCore.Data.IO.IOPort.DiscardOutBuffer"/>
        /// </summary>
        void IIOPort.DiscardOutBuffer()
        {
            //nothing to do 
        }

        /// <summary>
        /// Concrete implementation of the IOPort interface Property <see cref="ProteusCore.Data.IO.IOPort.BytesAvailable"/>
        /// </summary>
        int IIOPort.BytesAvailable
        {
            get
            {
                return _receivedData.Count;
            }
        }

        /// <summary>
        /// Concrete implementation of the IOPort interface Property <see cref="ProteusCore.Data.IO.IOPort.IsOpen"/>
        /// </summary>
        bool IIOPort.IsOpen
        {
            get 
            {
                //if there is a specific device present then the port is open.
                return _specficDevice != null;
            }
        }

        /// <summary>
        /// Name that corresponds with the connection string used for this port
        /// </summary>
        string IIOPort.Name
        {
            get
            {
                return _devicePath;       
            }
        }

        string IIOPort.ProductString
        {
            get
            {
                return _productString;
            }
        }

        string IIOPort.ManufacturerString
        {
            get
            {
                return _manufacturerString;
            }
        }

        string IIOPort.SerialNumberString
        {
            get
            {
                return _serialNumberString;
            }
        }

        HIDAttributes IIOPort.HidAttrs
        {
            get
            {
                return _hidAttributes;
            }

        }

        int IIOPort.InputReportLength
        {
            get
            {
                return _specficDevice.InputReportLength;
            }
        }


        int IIOPort.OutputReportLength
        {
            get
            {
                return _specficDevice.OutputReportLength;
            }
        }

        #endregion
    }
}
