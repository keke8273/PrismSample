using System.Runtime.Serialization;

namespace DataLinkLayer.IO.Protocol
{
    /// <summary>
    /// Static class encapsulating the values that form the basic rules for communicating with an
    /// proteus device.
    /// </summary>
    /// 
    public static class DeviceProtocol
    {
        /// <summary>
        /// The maximum number of times to try and send a command to the device
        /// </summary>
        public static readonly int MAX_RETRIES = 3;

        /// <summary>
        /// The maximum number of duplicate frames to receive in a row before assuming an error
        /// </summary>
        public static readonly int MAX_DUPLICATES = 3;

        /// <summary>
        /// The maximum number of invalid frames to receive in a row before assuming an error.
        /// </summary>
        public static readonly int MAX_INVALID = 3;

        /// <summary>
        /// The timeout value used when waiting for an ack
        /// </summary>
        public static readonly int ACK_TIMEOUT = 100;

        /// <summary>
        /// Timeout used to determine how long the device has to respond. This is the default application 
        /// level response timeout and must be bigger than the ACK_TIMEOUT
        /// </summary>
        public static readonly int RESPONSE_TIMEOUT = 500;

        /// <summary>
        /// The private backing field for the Read_Block_Size property
        /// </summary>
        private static int _read_block_size = 63;

        /// <summary>
        /// Flag used to indicate if the read block size has been set
        /// </summary>
        private static bool _read_set = false;

        /// <summary>
        /// The number of bytes that should be read in a block of data received on the comms channel
        /// </summary>
        /// <remarks>This has a default value of 63 bytes but can be set once at some later time eg. when 
        /// a USB device connects and the report size is determined</remarks>
        public static int Read_Block_Size 
        {
            get
            {
                return _read_block_size;
            }
            set
            {
                if (!_read_set)
                {
                    _read_block_size = value;
                    _read_set = true;
                }
            }
        }
        
        /// <summary>
        /// The private backing field for the Write_Block_Size property
        /// </summary>
        private static int _write_block_size = 63;

        /// <summary>
        /// Flag used to indicate if the write block size has been set
        /// </summary>
        private static bool _write_set = false;

        /// <summary>
        /// The number of bytes that should be written in one write to the comms channel.
        /// </summary>
        /// <remarks>This has a default value of 63 bytes but can be set once at some later time eg. when 
        /// a USB device connects and the report size is determined</remarks>        
        public static int Write_Block_Size
        {
            get
            {
                return _write_block_size;
            }
            set
            {
                if (!_write_set)
                {
                    _write_block_size = value;
                    _write_set = true;
                }
            }
        }

    }

    /// <summary>
    /// Enum Describing the different frame type used in the application
    /// </summary>
    [DataContract]
    public enum EFrameType : ushort
    {
        //Configuration
        SetOperatorConfig           = 0x0100,
        SetMeterConfig              = 0x0101,
        SetOperatorListIterator     = 0x0102,
        AddOperatorID               = 0x0112,
        RemoveOperatorID            = 0x0113,
        ClearOperatorList           = 0x0114,
        SetDateAndTime              = 0x0120,
        ClearStripVialCache         = 0x0130,
        GetOperatorConfig           = 0x0180,
        GetMeterConfig              = 0x0181,
        GetNextOperatorID           = 0x0191,
        GetDateAndTime              = 0x01A0,
        ReplyOperatorConfig         = 0x01C0,
        ReplyMeterConfig            = 0x01C1,
        ReplyOperatorID             = 0x01D1,
        ReplyDateAndTime            = 0x01E0,


        //TestResults
        ReplyPatientTestResult      = 0x0200,
        ReplyLQCTestResult          = 0x0201,
        ReplyTransientResultHeader  = 0x0202,
        ReplyTransientResult        = 0x0203,
        SetPatientResultIterator    = 0x0210,
        SetLQCResultIterator        = 0x0211,
        SetTransientResultIterator  = 0x0212,
        ClearPatientTestResults     = 0x0220,
        ClearLQCTestResults         = 0x0221,
        ClearTransientResults       = 0x0222,
        [EnumMember]
        SetPatientResultAccessionNumber = 0x0230,
        [EnumMember]
        SetLQCResultAccessionNumber = 0x0231,
        [EnumMember]
        SetTransientResultAccessionNumber = 0x232,
        GetPatientResultStatus      = 0x0240,
        GetLQCResultStatus          = 0x0241,
        GetTransientResultStatus    = 0x0242,
        ReplyPatientResultStatus    = 0x0250,
        ReplyLQCResultStatus        = 0x0251,
        ReplyTransientResultStatus  = 0x0252,
        GetNextPatientResult        = 0x0290,
        GetNextLQCResult            = 0x0291,
        GetNextTransientResult      = 0x0292,
        InsertPatientTestResult     = 0x0f00,
        InsertLQCTestResult         = 0x0f01,
        InsertTransientRecordHeader = 0x0f02,

        // Rubix Transient
        GetNextRubixTransientHeader = 0x02A0,
        GetNextRubixTransientChunk  = 0x02A1,
        ReplyRubixTransientHeader   = 0x0208,
        ReplyRubixTransientChunk    = 0x0209,

        //Diagnostic Frames
        ReplyFaultRecord            = 0x0300,
        ReplyBuildInfo              = 0x0301,
        ReplyUsageCounter           = 0x0302,
        SetFaultEntryRecordIterator = 0x0310,
        ClearFaultLog               = 0x0320,
        [EnumMember]
        SetFaultLogAccessionNumber  = 0x0330,
        GetFaultRecordStatus        = 0x0340,
        ReplyFaultRecordStatus      = 0x0350,
        GetNextFaultRecordEntry     = 0x0390,
        GetBuildInfo                = 0x0391,
        GetUsageCounter             = 0x0392,
        InsertFaultRecord           = 0x0f03,

        //Software update frames
        PrepForFWUpgrade            = 0x0400,
        ApplicationImage            = 0x0401,

        //Proteus Engineering Log
        ReplyEngRecord              = 0x0400,
        SetEngLogRecordIterator     = 0x0410,
        ClearEngLog                 = 0x0420,
        [EnumMember]
        SetEngLogAccessionNumber    = 0x0430,
        GetEngRecordStatus          = 0x0440,
        ReplyEngRecordStatus        = 0x0450,
        GetNextEngRecordEntry       = 0x0490,
        InsertEngRecord             = 0x0f04,

        //Rubix Engineering Log
        GetNextEngLog               = 0x09A0,
        GetNextEngLogChunk          = 0x09A1,
        ReplyEngLog                 = 0x0908,
        ReplyEngLogChunk            = 0x0909,
        SetEngLogIterator           = 0x0912,
        ClearEngLogs                = 0x0930,

        //System Frames
        DoConfirm                   = 0x1000,
        NaK                         = 0x2000,
        Ack                         = 0x4000,
     
        //Misc
        DoCancel                    = 0x0500,
        DoLogin                     = 0x0600,
        Unknown                     = 0xFFFF,
    }

    /// <summary>
    /// Enum describing the different type of devices
    /// </summary>
    public enum EDeviceType : ushort
    {
        Proteus,
        Rubix
    }

    /// <summary>
    /// enum describing the status of an Proteus
    /// </summary>
    public enum EProteusStatus : ushort
    {
        Ready = 0x0000,
        Busy = 0x0001,
        Error = 0x0002,
        NotAuthorised = 0x0003,
        Invalid = 0xFFFF
    }

}
