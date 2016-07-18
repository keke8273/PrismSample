using System;
using System.Collections.Generic;
using DataLinkLayer.IO.Protocol;
using QBR.Infrastructure.Models.DataTypes;

namespace QBR.Infrastructure.Models
{
    /// <summary>
    /// Singleton Factory class for building command frames to send to the Proteus meter
    /// </summary>
    public sealed class FrameBuilder
    {
        #region Constructors

        /// <summary>
        /// Constructor used to construct the singleton instance
        /// </summary>
        private FrameBuilder()
        {
            //generate a random starting point for the sequence number so that it will gaurantee that even
            //if the application is restarted two consecutive commands of the same type
            //won't be treated as a duplicate.
            var randgen = new Random();
            _sequenceNumber = Convert.ToUInt16(randgen.Next(UInt16.MaxValue - 1));
        }

        /// <summary>
        /// Initializes the <see cref="FrameBuilder"/> class.
        /// </summary>
        static FrameBuilder()
        {
        }

        #endregion Constructors

        #region Private data

        /// <summary>
        /// field backing the Singleton Instance property
        /// </summary>
        private static FrameBuilder _instance = new FrameBuilder();

        /// <summary>
        /// The current sequence number of the frame being constructed.
        /// </summary>
        private UInt16 _sequenceNumber;

        /// <summary>
        /// Property used to get the next sequence number. This will wraps the _sequence number
        /// field around to 0 when the value reaches UInt16.Max
        /// </summary>
        /// <value>
        /// The _next sequence number.
        /// </value>
        private UInt32 _nextSequenceNumber
        {
            get
            {
                if (_sequenceNumber == UInt16.MaxValue)
                {
                    _sequenceNumber = 0;
                }
                else
                {
                    _sequenceNumber++;
                }
                return _sequenceNumber;
            }
        }

        #endregion Private data

        #region public data

        /// <summary>
        /// Singleton instance property for this class
        /// </summary>
        /// <value>
        /// The instance.
        /// </value>
        public static FrameBuilder Instance
        {
            get { return _instance; }
        }

        #endregion public data

        #region Functions

        /// <summary>
        /// construct a list of frames with no result filter
        /// </summary>
        /// <param name="itemNumber">The item number.</param>
        /// <returns>
        /// List of frames used to get the nth patient result
        /// </returns>
        public List<IFrame> Build_GetNthPatientResult(int itemNumber)
        {
            return Build_GetNthPatientResult(itemNumber, null);
        }

        /// <summary>
        /// Build_s the get NTH patient result.
        /// </summary>
        /// <param name="itemNumber">The item number.</param>
        /// <param name="filter">The filter.</param>
        /// <returns></returns>
        public List<IFrame> Build_GetNthPatientResult(int itemNumber, ResultFilter filter)
        {
            var frames = new List<IFrame>();
            var targetFilter = filter;

            if (targetFilter == null)
            {
                targetFilter = ResultFilter.DisabledFilter;
            }

            frames.Add(Build_PayloadedCommand(EFrameType.SetPatientResultIterator, targetFilter));

            for (var i = 0; i < itemNumber; i++)
            {
                frames.Add(Build_SimpleCommand(EFrameType.GetNextPatientResult));
            }

            return frames;
        }

        /// <summary>
        /// Build_s the get NTH LQC result.
        /// </summary>
        /// <param name="itemNumber">The item number.</param>
        /// <returns></returns>
        public List<IFrame> Build_GetNthLQCResult(int itemNumber)
        {
            return Build_GetNthLQCResult(itemNumber, null);
        }

        /// <summary>
        /// Build_s the get NTH LQC result.
        /// </summary>
        /// <param name="itemNumber">The item number.</param>
        /// <param name="filter">The filter.</param>
        /// <returns></returns>
        public List<IFrame> Build_GetNthLQCResult(int itemNumber, ResultFilter filter)
        {
            var frames = new List<IFrame>();
            var targetFilter = filter;

            if (targetFilter == null)
            {
                targetFilter = ResultFilter.DisabledFilter;
            }

            frames.Add(Build_PayloadedCommand(EFrameType.SetLQCResultIterator, targetFilter));

            for (var i = 0; i < itemNumber; i++)
            {
                frames.Add(Build_SimpleCommand(EFrameType.GetNextLQCResult));
            }

            return frames;
        }

        /// <summary>
        /// construct a list of frames with no result filter
        /// </summary>
        /// <param name="itemNumber">The item number.</param>
        /// <returns>
        /// List of frames used to get the nth patient result
        /// </returns>
        public List<IFrame> Build_GetNthFaultRecord(int itemNumber)
        {
            return Build_GetNthFaultRecord(itemNumber, null);
        }

        /// <summary>
        /// Build_s the get NTH fault record.
        /// </summary>
        /// <param name="itemNumber">The item number.</param>
        /// <param name="filter">The filter.</param>
        /// <returns></returns>
        public List<IFrame> Build_GetNthFaultRecord(int itemNumber, ResultFilter filter)
        {
            var frames = new List<IFrame>();
            var targetFilter = filter;

            if (targetFilter == null)
            {
                targetFilter = ResultFilter.DisabledFilter;
            }

            frames.Add(Build_PayloadedCommand(EFrameType.SetFaultEntryRecordIterator, targetFilter));

            for (var i = 0; i < itemNumber; i++)
            {
                frames.Add(Build_SimpleCommand(EFrameType.GetNextFaultRecordEntry));
            }

            return frames;
        }

        /// <summary>
        /// Build_s the get NTH operator identifier.
        /// </summary>
        /// <param name="itemNumber">The item number.</param>
        /// <returns></returns>
        public List<IFrame> Build_GetNthOperatorID(int itemNumber)
        {
            return Build_GetNthOperatorID(itemNumber, null);
        }

        /// <summary>
        /// Build_s the get NTH operator identifier.
        /// </summary>
        /// <param name="itemNumber">The item number.</param>
        /// <param name="filter">The filter.</param>
        /// <returns></returns>
        public List<IFrame> Build_GetNthOperatorID(int itemNumber, OperatorIDFilter filter)
        {
            var frames = new List<IFrame>();

            var targetFilter = filter;

            if (targetFilter == null)
            {
                targetFilter = OperatorIDFilter.DisabledFilter;
            }

            frames.Add(Build_PayloadedCommand(EFrameType.SetOperatorListIterator, targetFilter));

            for (var i = 0; i < itemNumber; i++)
            {
                var getOperatorID = Build_SimpleCommand(EFrameType.GetNextOperatorID);
                frames.Add(getOperatorID);
            }

            return frames;
        }

        /// <summary>
        /// Build_s the get NTH transient result.
        /// </summary>
        /// <param name="itemNumber">The item number.</param>
        /// <returns></returns>
        public List<IFrame> Build_GetNthTransientResult(int itemNumber)
        {
            return Build_GetNthTransientResult(itemNumber, null);
        }

        /// <summary>
        /// Build_s the get NTH transient result.
        /// </summary>
        /// <param name="itemNumber">The item number.</param>
        /// <param name="filter">The filter.</param>
        /// <returns></returns>
        public List<IFrame> Build_GetNthTransientResult(int itemNumber, ResultFilter filter)
        {
            var frames = new List<IFrame>();
            var targetFilter = filter;

            if (targetFilter == null)
            {
                targetFilter = ResultFilter.DisabledFilter;
            }

            frames.Add(Build_PayloadedCommand(EFrameType.SetTransientResultIterator, targetFilter));

            for (var i = 0; i < itemNumber; i++)
            {
                frames.Add(Build_SimpleCommand(EFrameType.GetNextTransientResult));
            }

            return frames;
        }

        /// <summary>
        /// Build_s the get Rubix transient header.
        /// </summary>
        /// <param name="filter">The filter.</param>
        /// <returns></returns>
        public List<IFrame> Build_GetRubixTransientHeader(RubixResultFilter filter)
        {
            var frames = new List<IFrame>();
            var targetFilter = filter;

            frames.Add(Build_PayloadedCommand(EFrameType.SetTransientResultIterator, targetFilter));

            frames.Add(Build_SimpleCommand(EFrameType.GetNextRubixTransientHeader));

            return frames;
        }


        /// <summary>
        /// Build_s the get Rubix transient.
        /// </summary>
        /// <param name="transientHeader">The transient header.</param>
        /// <returns></returns>
        public IEnumerable<IFrame> Build_GetRubixTransient(RubixTransientHeader transientHeader)
        {
            var frames = new List<IFrame>();

            for (var i = 0; i < transientHeader.FileInformation.FileSize; i += RubixTransientChunk.DataLength)
            {
                var remainingSize = transientHeader.FileInformation.FileSize - i;
                frames.Add(Build_PayloadedCommand(EFrameType.GetNextRubixTransientChunk, new RubixTransientChunkHeader
                    {
                        ProtocolVersion = 0,
                        AccessionNumber = transientHeader.AccessionNumber,
                        Offset = (UInt32)i,
                        RequestedSize = (UInt16)Math.Min(RubixTransientChunk.DataLength, remainingSize)
                    }));
            }

            return frames;
        }

        /// <summary>
        /// Build an IFrame that contains a payload
        /// </summary>
        /// <param name="type">The type of IFrame to build</param>
        /// <param name="dataObject">The object that defines the payload</param>
        /// <returns>
        /// An IFrame of the specified type that encapsulates the dataObject as payload data
        /// </returns>
        public IFrame Build_PayloadedCommand(EFrameType type, ABaseCommsData dataObject)
        {
            return new IFrame(type, _nextSequenceNumber, dataObject.ToArray());
        }

        /// <summary>
        /// Builds a simple IFrame with no payload
        /// </summary>
        /// <param name="type">The type of IFrame to build</param>
        /// <returns>
        /// An IFrame of the specified type
        /// </returns>
        public IFrame Build_SimpleCommand(EFrameType type)
        {
            return new IFrame(type, _nextSequenceNumber);
        }

        #endregion Functions

    }
}
