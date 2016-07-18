
// Software Copyright (c) 2013 by Hydrix Pty. Ltd.
//
// This material is protected by copyright law. It is unlawful
// to copy it.
//
// This document contains confidential information. It is not to be
// disclosed or used except in accordance with applicable contracts
// or agreements.

using DataLinkLayer.IO.Protocol;

namespace DataLinkLayer.IO
{
    /// <summary>
    /// A base class for all application layer response data handlers.
    /// </summary>
    public abstract class AResponseHandler
    {
        #region Constructor

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <remarks>
        /// This will set the application layer timeout to the default value defined in the ProteusProtocol class
        /// this can be overridden in a child subclass
        /// </remarks>
        public AResponseHandler()
        {
            Data = null;
            ResponseTimeoutInterval = DeviceProtocol.RESPONSE_TIMEOUT;
        }

        #endregion Constructor

        #region Public data

        /// <summary>
        /// Flag used to indicate if there has been a complete response.
        /// </summary>
        public virtual bool Complete { get; protected set; }

        /// <summary>
        /// Get the data received by the handler the type of data is dependent on 
        /// the response handler type
        /// </summary>
        public object Data { get; protected set; }

        /// <summary>
        /// The response Timeout interval to be used with this response handler
        /// </summary>
        public int ResponseTimeoutInterval { get; protected set; }

        #endregion Public data

        #region Public methods

        /// <summary>
        /// Handle and IFrame command in the appropriate fashion
        /// </summary>
        /// <param name="command">The IFrame containing the data to be processed</param>
        /// <returns>True if the command frame was handled otherwise false</returns>
        public abstract bool HandleResponse(IFrame command);

        /// <summary>
        /// Reset the public state
        /// </summary>
        public virtual void Reset()
        {
            Complete = false;
            Data = null;
            ResponseTimeoutInterval = DeviceProtocol.RESPONSE_TIMEOUT;
        }

        #endregion Public methods
    }
}
