
// Software Copyright (c) 2013 by Hydrix Pty. Ltd.
//
// This material is protected by copyright law. It is unlawful
// to copy it.
//
// This document contains confidential information. It is not to be
// disclosed or used except in accordance with applicable contracts
// or agreements.

using System;
using System.Diagnostics;
using DataLinkLayer.Diagnostics;
using DataLinkLayer.IO;
using DataLinkLayer.IO.Protocol;
using QBR.Infrastructure.Models.DataTypes;

namespace QBR.Infrastructure.Models.ResponseHandlers
{
    /// <summary>
    /// Simple response handler
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class SimpleResponseHandler<T>: AResponseHandler
        where T : ABaseCommsData
    {
        /// <summary>
        /// Handle and IFrame command in the appropriate fashion
        /// </summary>
        /// <param name="command">The IFrame containing the data to be processed</param>
        /// <returns>
        /// True if the command frame was handled otherwise false
        /// </returns>
        public override bool HandleResponse(IFrame command)
        {
            var handled = false;

            var responseData = Activator.CreateInstance<T>();
            responseData.FromArray(command.Payload, 0);

            if (responseData.IsValid)
            {
                Data = responseData;

                Logger.LogMessage(Logger.TestSwitch,
                                  TraceLevel.Info,
                                  string.Format("\n**** RECEIVED *****\n{0}\n*******************", Data.ToString()));

                Complete = true;
                handled = true;
            }
            
            return handled;
        }
    }
}
