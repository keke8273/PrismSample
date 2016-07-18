using System;
using System.Runtime.InteropServices;

namespace USBCommsPlugin.USB
{
    /// <summary>
    /// exception class that is thrown by the HID interfaces
    /// </summary>
    public class HIDDeviceException: Exception
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="strMessage">The reason for the exception</param>
        public HIDDeviceException(string strMessage) : base(strMessage) { }

        public static HIDDeviceException GenerateWithWinError(string strMessage)
        {
            return new HIDDeviceException(string.Format("Msg:{0} WinEr:{1:X8}", strMessage, Marshal.GetLastWin32Error()));
        }

        public static HIDDeviceException GenerateError(string strMessage)
        {
            return new HIDDeviceException(string.Format("Msg:{0}", strMessage));
        }
    }
}
