
// Software Copyright (c) 2013 by Hydrix Pty. Ltd.
//
// This material is protected by copyright law. It is unlawful
// to copy it.
//
// This document contains confidential information. It is not to be
// disclosed or used except in accordance with applicable contracts
// or agreements.

using System;
using System.Collections.Generic;

namespace DataLinkLayer.IO
{
    /// <summary>
    /// Interface used to encapsulate the behavior of a communication channel or IOPort.
    /// </summary>
    public interface IIOPort
    {
        /// <summary>
        /// name of the underlying port
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Close the IOPort.
        /// </summary>
        /// <remarks>
        /// This will clear any event listeners and any buffers.
        /// </remarks>
        void Close();

        /// <summary>
        /// Open the IOPort. Once opened the port can be read from 
        /// and written to.
        /// </summary>
        void Open();

        /// <summary>
        /// Read data from the IOPort if there is any available.
        /// </summary>
        /// <param name="buffer">A byte array to store the data in</param>
        /// <param name="offset">is an zero based offset into "buffer" to start placing the data</param>
        /// <param name="count">Number of bytes to read.</param>
        /// <returns>number of bytes read.</returns>
        int Read(byte[] buffer, int offset, int count);

        /// <summary>
        /// Write some bytes to the IOPort.
        /// </summary>
        /// <param name="buffer">a byte array containing the data to be sent</param>
        /// <param name="offset">is an zero based offset into "buffer" to start writing the data from.</param>
        /// <param name="count">Number of bytes from "buffer" starting at offset to write</param>
        void Write(byte[] buffer, int offset, int count);

        /// <summary>
        /// Event to notify listeners asynchronously of data being received.
        /// </summary>
        event PortDataReceivedEventHandler DataReceived;

        /// <summary>
        /// Event to notify listeners asynchronously of an error occurring in the underlying port.
        /// </summary>
        event EventHandler PortDeviceError;

        /// <summary>
        /// Clear the Port's input data buffer.
        /// </summary>
        void DiscardInBuffer();

        /// <summary>
        /// Clear the Port's output data buffer
        /// </summary>
        void DiscardOutBuffer();

        /// <summary>
        /// Property defining the number of bytes that are currently available.
        /// </summary>
        int BytesAvailable { get; }

        /// <summary>
        /// Property used to determine if a port is open to reading and writing.
        /// </summary>
        bool IsOpen { get; }

        string ProductString { get; }

        string ManufacturerString { get; }

        string SerialNumberString { get; }

        HIDAttributes HidAttrs { get; }

        int InputReportLength { get; }

        int OutputReportLength { get; }

    }

    /// <summary>
    /// Class to encapsulate data that needs to be communicated when a data received event
    /// is triggered.
    /// </summary>
    public class PortDataReceivedEventArgs : EventArgs
    {
        /// <summary>
        /// Constructor for a normal data received event.
        /// </summary>
        /// <param name="byteCount">is the number of bytes that have been received.</param>
        public PortDataReceivedEventArgs(int byteCount)
        {
            Error = PortDataReceivedErrors.DataReceived_OK;
            numBytesReceived = byteCount;
        }

        /// <summary>
        /// Constructor used to inform the listeners of an error occurring whilst receiving data.
        /// </summary>
        /// <param name="error"></param>
        public PortDataReceivedEventArgs(PortDataReceivedErrors error)
        {
            Error = error;
            numBytesReceived = 0;
        }

        /// <summary>
        /// Enum representing the type of error that has occurred.
        /// </summary>
        public PortDataReceivedErrors Error { get; private set; }

        /// <summary>
        /// Number of bytes that have been received for this event.
        /// </summary>
        public int numBytesReceived { get; set; }
    }

    /// <summary>
    /// enum describing the different types of errors that could occur
    /// whilst receiving data.
    /// </summary>
    public enum PortDataReceivedErrors
    {
        DataReceived_OK = 0,
        DeviceDisconnected,
        Max,
    }

    /// <summary>
    /// Delegate used to notify DataReceived listeners when a data received event occurs.
    /// </summary>
    /// <param name="sender">The originator of the event</param>
    /// <param name="args">event specific data</param>
    public delegate void PortDataReceivedEventHandler(object sender, PortDataReceivedEventArgs args);

    /// <summary>
    /// Holds HID Attributes
    /// </summary>
    public class HIDAttributes
    {
        #region Constructors

        public HIDAttributes()
        {
            m_Size = DataSize();
        }

        #endregion Constructors

        #region protected Data

        protected Int32 m_Size;
        protected UInt16 m_VendorID;
        protected UInt16 m_ProductID;
        protected UInt16 m_VersionNumber;

        #endregion protected Data

        #region Public Data

        /// <summary>

        public Int32 Size
        {
            get
            {
                return m_Size;
            }
        }

        public UInt16 VendorID
        {
            get
            {
                return m_VendorID;
            }
        }

        public UInt16 ProductID
        {
            get
            {
                return m_ProductID;
            }
        }

        public UInt16 VersionNumber
        {
            get
            {
                return m_VersionNumber;
            }
        }

        #endregion

        #region Public Methods

        public int DataSize()
        {
            return (sizeof(Int32) + (sizeof(UInt16) * 3));
        }

        public byte[] ToArray()
        {
            var data = new List<byte>();

            data.AddRange(BitConverter.GetBytes(m_Size));
            data.AddRange(BitConverter.GetBytes(m_VendorID));
            data.AddRange(BitConverter.GetBytes(m_ProductID));
            data.AddRange(BitConverter.GetBytes(m_VersionNumber));

            return data.ToArray();
        }

        public bool FromArray(byte[] data, int offset)
        {
            var IsValid = false;
            if (data.Length >= DataSize())
            {
                var fieldPosition = offset;

                m_Size = BitConverter.ToInt32(data, fieldPosition);
                fieldPosition += sizeof(Int32);

                m_VendorID = BitConverter.ToUInt16(data, fieldPosition);
                fieldPosition += sizeof(UInt16);

                m_ProductID = BitConverter.ToUInt16(data, fieldPosition);
                fieldPosition += sizeof(UInt16);

                m_VersionNumber = BitConverter.ToUInt16(data, fieldPosition);
                fieldPosition += sizeof(UInt16);

                IsValid = true;
            }
            return IsValid;
        }


        #endregion Public Methods
    }

}
