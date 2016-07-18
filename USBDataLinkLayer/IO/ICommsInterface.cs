
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
using DataLinkLayer.Utils;

namespace DataLinkLayer.IO
{
    /// <summary>
    /// This interface defines the behavior for a class wishing to manage the allocation and notification
    /// of device communication interfaces. The publisher interface is to be used to notify clients 
    /// of a connection/removal event.
    /// </summary>
    public interface ICommsInterface: IPublisher, IDisposable
    {

        /// <summary>
        /// Gets the serial numbers of all the currently connected devices.
        /// </summary>
        /// <returns>List of strings containing the connection strings of the connected devices</returns>
        List<string> GetAvailablePorts();

        /// <summary>
        /// check if a connection string is valid
        /// </summary>
        /// <param name="connectionString">a string used to connect to the device</param>
        /// <returns>true is the connection string is valid otherwise false.</returns>
        bool IsValidPortName(string connectionString);

        /// <summary>
        /// Gets the IIOPort associated with the serial number
        /// </summary>
        /// <param name="serialNo">The connection string of the particular IIOport to get</param>
        /// <returns>The IIOPort identified by the serialNo string otherwise null.</returns>
        IIOPort GetPort(string connectionString);

    }

    /// <summary>
    /// An Event argument class passed when a connection event occurs. It will indicate the device
    /// for which the connection event occured and has a flag that will be true for a new device
    /// connection or false for a disconnection/removal.
    /// </summary>
    public class CommsInterfaceEventArgs : EventArgs
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="connectionString">connectionString of the device the event applies to</param>
        /// <param name="deviceConnected">True is if a new device coneccted otherwise false</param>
        public CommsInterfaceEventArgs(string connectionString, bool deviceConnected)
        {
            ConnectionString = connectionString;
            DeviceConnected = deviceConnected;
        }

        /// <summary>
        /// Connection string of the device the event applies to.
        /// </summary>
        public string ConnectionString { get; private set; }

        /// <summary>
        /// Indicator used to determine if this event is a new device connection or if it is a device removal.
        /// True indicates a new device connection, false indicates the device was removed.
        /// </summary>
        public bool DeviceConnected { get; private set; }
    }
}
