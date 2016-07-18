using System.Collections.Generic;
using System.ComponentModel;
using DataLinkLayer.IO.Protocol;

namespace DataLinkLayer.IO
{
    public interface ICommsWorker
    {
        /// <summary>
        /// Event used for reporting progress
        /// </summary>
        event ProgressChangedEventHandler ProgressChanged;

        /// <summary>
        /// Event used to report the completion of the work
        /// </summary>
        event RunWorkerCompletedEventHandler RunWorkerCompleted;

        /// <summary>
        /// Flag indicating if the work is to be cancelled
        /// </summary>
        bool CancellationPending { get; }

        /// <summary>
        /// Will be true when there is some work being performed otherwise it will be false
        /// </summary>
        bool IsBusy { get; }

        /// <summary>
        /// flag indicating if the worker is stopped.
        /// </summary>
        bool Stopped { get; }

        /// <summary>
        /// Send a list of IFrame commands and wait for a response to each command. 
        /// </summary>
        /// <param name="commands">List of IFrames to be send</param>
        /// <param name="handler">The application layer handler that will process and aggregate the responses</param>
        /// <remarks>It is valid for the handler to be null in which case no response is expected.</remarks>
        void Start(List<IFrame> commands, AResponseHandler handler);

        /// <summary>
        /// Send the a single IFrame command and wait for responses to be received.
        /// </summary>
        /// <remarks>The worker doesn't not guarantee the completeness of the response beyond the completeness
        /// of the frames received. A check should be made on the handler when the worker is complete to make
        /// sure the entire expected response was received.</remarks>
        /// <param name="command">The single command to be sent</param>
        /// <param name="handler">The application layer handler that will process and aggregate the responses</param>
        void Start(IFrame command, AResponseHandler handler);

        /// <summary>
        /// Start the worker with a list of commands to send with no responses expected.
        /// </summary>
        /// <param name="frames">List of IFrame to be sent</param>
        void Start(IEnumerable<IFrame> frames);

        /// <summary>
        /// Sends the specified command and does not wait for a response.
        /// </summary>
        /// <param name="command">the command to be communicated</param>
        void Start(IFrame command);

        /// <summary>
        /// Stop the worker immediately. This will cause any current work to be cancelled
        /// </summary>
        void Stop();

        /// <summary>
        /// Implementation of the IDisposable interface
        /// </summary>
        void Dispose();
    }
}