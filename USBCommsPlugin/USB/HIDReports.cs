using System;

namespace USBCommsPlugin.USB
{
    /// <summary>
    /// Base class for report types. Simply wraps a byte buffer. This can be used for the output report
    /// </summary>
    public class Report
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="oDev">Constructing device</param>
        /// <param name="dev"></param>
        /// <param name="reportSize"></param>
        public Report(HIDDevice dev, int reportSize)
        {
            ConstructingDevice = dev;
            m_arrBuffer = new byte[reportSize];
        }

        #region protected Data

        /// <summary>Buffer for raw report bytes</summary>
        protected byte[] m_arrBuffer;
        
        /// <summary>Length of the report</summary>
        protected int m_nLength;

        #endregion protected Data

        #region Public Data

        /// <summary>
        /// The device that this report belongs to
        /// </summary>
        public readonly HIDDevice ConstructingDevice;

        /// <summary>
        /// Accessor for the raw byte buffer
        /// </summary>
        public byte[] Buffer
        {
            get { return m_arrBuffer; }
        }

        /// <summary>
        /// Accessor for the buffer length
        /// </summary>
        public int BufferLength
        {
            get
            {
                return m_nLength;
            }
        }
        
        #endregion

        #region Public Methods

        /// <summary>
        /// Sets the raw byte array.
        /// </summary>
        /// <param name="arrBytes">Raw report bytes</param>
        public virtual void SetBuffer(byte[] arrBytes, int offset, int count)
        {
            if (count > m_arrBuffer.Length)
            {
                throw new HIDDeviceException("There was a problem copying the source data into the report");
            }

            //prep the buffer
            Array.Clear(m_arrBuffer, 0, m_arrBuffer.Length);
            //put the report tag in the first byte.
            m_arrBuffer[0] = 0x01;

            Array.Copy(arrBytes, offset, m_arrBuffer, 1, count);
            
            m_nLength = m_arrBuffer.Length;
        }

        /// <summary>
        /// Given a report size calculate the maximum payload the report can carry
        /// </summary>
        /// <param name="reportSize">The size of the report to be constructed</param>
        /// <returns>The size of the payload field in bytes</returns>
        public static int PayloadSize(int reportSize)
        {
            return reportSize - 1;
        }

        #endregion Public Methods
    }

    /// <summary>
    /// Simple class for incomming USB Reports
    /// </summary>
    public class InputReport : Report
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="dev">The device the report was received from</param>
        /// <param name="Report_size">The size in bytes of the report</param>
        public InputReport(HIDDevice dev, int Report_size)
            : base(dev, Report_size)
        { }

        /// <summary>
        /// Override of the helper method for setting the underlying data buffer
        /// </summary>
        /// <param name="arrBytes">The data that the underlying buffer is to be set to</param>
        /// <param name="offset">The offset into the arrBytes array that the data starts</param>
        /// <param name="count">The number of bytes fomr arrBytes to use</param>
        public override void SetBuffer(byte[] arrBytes, int offset, int count)
        {
            if (count > m_arrBuffer.Length)
            {
                throw new HIDDeviceException("There was a problem copying the source data into the report");
            }

            //prep the buffer
            Array.Clear(m_arrBuffer, 0, m_arrBuffer.Length);

            Array.Copy(arrBytes, offset, m_arrBuffer, 0, count);

            m_nLength = m_arrBuffer.Length;            
        }
    }

}
