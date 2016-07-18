
// Software Copyright (c) 2013 by Hydrix Pty. Ltd.
//
// This material is protected by copyright law. It is unlawful
// to copy it.
//
// This document contains confidential information. It is not to be
// disclosed or used except in accordance with applicable contracts
// or agreements.

using System;

namespace DataLinkLayer.Utils
{
    /// <summary>
    /// Interface for the Publisher component of an  observer interface
    /// </summary>
    public interface IPublisher
    {
        /// <summary>
        /// Used to register a Subscriber with a Publisher
        /// </summary>
        /// <typeparam name="TSubscriber">The type of the subscriber.</typeparam>
        /// <param name="subscriber">The subscriber.</param>
        void RegisterSubscriber<TSubscriber>(TSubscriber subscriber) where TSubscriber : ISubscriber;

        /// <summary>
        /// Used to unregister a Subscriber from a Publisher
        /// </summary>
        /// <typeparam name="TSubscriber">The type of the subscriber.</typeparam>
        /// <param name="subscriber">The subscriber.</param>
        void UnregisterSubscriber<TSubscriber>(TSubscriber subscriber) where TSubscriber : ISubscriber;

        /// <summary>
        /// Used to notify all Subscribers that an event has occurred
        /// </summary>
        /// <param name="args"> A <see cref="System.EventArgs"/> containing event-specific data </param>
        void NotifySubscribers(EventArgs args);
    }

    /// <summary>
    /// Interface for the Subscriber component of an observer interface
    /// </summary>
    public interface ISubscriber
    {
        /// <summary>
        /// Called by the Publisher when an an event is being posted to the Subscriber
        /// </summary>
        /// <param name="sender"> A <see cref="System.object"/> where the event originated </param>
        /// <param name="args"> A <see cref="System.EventArgs"/> containing event-specific data </param>
        void OnNotification(Object sender, EventArgs args);
    }
}
