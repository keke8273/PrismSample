
// Software Copyright (c) 2013 by Hydrix Pty. Ltd.
//
// This material is protected by copyright law. It is unlawful
// to copy it.
//
// This document contains confidential information. It is not to be
// disclosed or used except in accordance with applicable contracts
// or agreements.

using System;

namespace DataLinkLayer.IO.CommsCntrl
{
    /// <summary>
    /// Datalink layer event arguments class used for reporting progress and the completion of a transaction to the application layer.
    /// </summary>
    public class DatalinkEventArgs : EventArgs
    {
        /// <summary>
        /// Constructor 
        /// </summary>
        /// <param name="type">Type of the event that has occured</param>
        public DatalinkEventArgs(DatalinkLayerEventType type)
        {
            EventType = type;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="type">Type of event that has occured</param>
        /// <param name="frame">the frame associated with the event</param>
        public DatalinkEventArgs(DatalinkLayerEventType type, object data)
            : this(type)
        {
            ReceivedData = data;
        }

        /// <summary>
        /// Gets the Event type 
        /// </summary>
        public DatalinkLayerEventType EventType { get; private set; }

        /// <summary>
        /// The data that was received
        /// </summary>
        public object ReceivedData { get; private set; }
    }

    /// <summary>
    /// Enum defining the types of events that can occur.
    /// </summary>
    public enum DatalinkLayerEventType
    {
        DataSent,
        CommsError,
        DataReceived,
        Cancelled,
        Progress
    }
}
