
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
using System.IO;
using System.Text;

namespace DataLinkLayer.Diagnostics
{
    /// <summary>
    /// Class providing diagnostic output functionality
    /// </summary>
    /// <remarks>
    ///     <para>
    ///     This class uses the Debug and Trace classes in the framework to
    ///     implement diagnostic services in the form of diagnostic message logging.
    ///     This class provides the functions through which a client is to log
    ///     messages and also the switches through which the diagnostic level is
    ///     controlled. These switches are set in the application config file
    ///     and thus diagnostic thresholds can be adjusted without the need to
    ///     recompile. Of course any diagnostic output is dependent on TRACE and/or
    ///     DEBUG being defined at build-time.
    ///     </para>
    /// </remarks>
    public static class Logger
    {
        #region Constructors

        /// <summary>
        /// Static class constructor for the Diagnostics class
        /// </summary>
        /// <remarks>
        ///     <para>
        ///     Create the various trace / boolean switches and, if required
        ///     create the diagnostic trace listener
        ///     </para>
        /// </remarks>
        static Logger()
        {
            // Initialize the Trace switches (the switch setting is determined by the app config file)

            CntrlSwitch = new TraceSwitch("CTRL", "Controller Trace Switch");
            AppSwitch = new TraceSwitch("APP", "Application Trace Switch");
            BDMSwitch = new TraceSwitch("BDM", "BDM Trace Switch");
            IOSwitch = new TraceSwitch("COMS", "IO Trace Switch");
            ConfigurationSwitch = new TraceSwitch("CNFG", "Configuration Trace Switch");
            TestSwitch = new TraceSwitch("TEST", "Test Trace Switch");
            UsbSwitch = new TraceSwitch("USB", "Usb Trace Switch");

            // Do we trace stack or not?
            StackSwtich = new TraceSwitch("Stack", "Stack Trace Switch");

            _lock = new Object();

            var TraceWindowSwitch = new BooleanSwitch("DiagWindow", "Trace Window Switch");
            var TraceFileSwitch = new BooleanSwitch("TraceFile", "Trace File Switch");

            // Only construct the trace file window if the switch is setup to do so
            string dir = null;
            if (TraceFileSwitch.Enabled == true)
            {
                // Create a sub-dir for the log file(s) to go into
                dir = Path.Combine(@".\", "logfiles");

                if (Directory.Exists(dir) == false)
                {
                    try
                    {
                        Directory.CreateDirectory(dir);
                    }
                    catch (Exception ex)
                    {
                        // Failed to created our logfile directory
                        LogException(AppSwitch, ex, "Logfile directory creation has failed");
                    }
                }

                TextWriterTraceListener fileTracer = null;
                if (Directory.Exists(dir) == true)
                {
                    try
                    {
                        var filename = string.Format("{0:yyyy}{1:MM}{2:dd}{3:HH}{4:mm}{5:ss}.log",
                                                            DateTime.Now,
                                                            DateTime.Now,
                                                            DateTime.Now,
                                                            DateTime.Now,
                                                            DateTime.Now,
                                                            DateTime.Now);

                        Stream theFile = File.Create(Path.Combine(dir, filename));
                        fileTracer = new TextWriterTraceListener(theFile);
                    }
                    catch (Exception ex)
                    {
                        // Failed to created our logfile directory
                        LogException(AppSwitch, ex, "Logfile creation has failed");
                    }
                }

                if (fileTracer != null)
                {
                    Trace.Listeners.Add(fileTracer);
                }
            }

            Trace.AutoFlush = true;
        }

        #endregion

        #region Private Data
        /// <summary>
        /// A lock for synchronizing access
        /// </summary>
        static private object _lock;

        #endregion

        #region Private Methods

        /// <summary>
        /// Formats a message ready for diagnostic output
        /// </summary>
        /// <remarks>
        ///     <para>
        ///     The message format is : Timestamp  Component name  Tracelevel  Message
        ///     </para>
        /// </remarks>
        /// <returns>
        ///     A <see cref="System.String"/> containing the formatted message
        /// </returns>
        /// <param name="ts"> A <see cref="System.Diagnostics.TraceSwitch"/> which controls trace output for this message </param>
        /// <param name="tl"> A <see cref="System.Diagnostics.TraceLevel"/> which controls trace output for this message </param>
        /// <param name="message"> A <see cref="System.String"/> containing the message </param>
        /// <exception cref="ArgumentNullException"> If <paramref name="message"/> is <c>null</c>.</exception>
        static private string _formatMessage(TraceSwitch ts, TraceLevel tl, string message)
        {
            string formattedMessage = null;

            if (message == null)
            {
                {
                    throw (new ArgumentNullException("message"));
                }
            }

            // Check that the specified switch is active as the requested level
            if (ts.Level >= tl)
            {
                var dt = DateTime.Now;
                formattedMessage = dt.ToString("HH:mm:ss:fff") + "," + ts.DisplayName + "," + tl.ToString() + "," + message;

                if (StackSwtich.Level != TraceLevel.Off)
                {
                    var methodName = "--";
                    var lineNumber = "--";
                    var filename = "--";
                    var traceStr = "--";

                    // Obtain a stack trace and retrieve the frame that applies to the
                    // application-level routine that made the call to 'LogXXX'
                    var trace = new StackTrace(true);
                    var frame = trace.GetFrame(2);

                    methodName = frame.GetMethod().Name;

                    filename = Path.GetFileName(frame.GetFileName());
                    lineNumber = frame.GetFileLineNumber().ToString();

                    traceStr = Environment.NewLine;
                    
                    traceStr += string.Format("\tTrace : {0}\t{1}:{2}", methodName, filename, lineNumber);
                    formattedMessage += traceStr;
                }
            }

            return (formattedMessage);
        }

        #endregion

        #region Public Data

        /// <summary>
        /// Controls the Trace output for the Controller.
        /// </summary>
        static public TraceSwitch CntrlSwitch;

        /// <summary>
        /// Controls the Trace output for the main Application.
        /// </summary>
        static public TraceSwitch AppSwitch;

        /// <summary>
        /// Controls the Trace output for the Business Domain.
        /// </summary>
        static public TraceSwitch BDMSwitch;

        /// <summary>
        /// Controls the trace output for the IO component.
        /// </summary>
        static public TraceSwitch IOSwitch;

        /// <summary>
        /// Controls the trace output for the Configuration component.
        /// </summary>
        static public TraceSwitch ConfigurationSwitch;
        
        /// <summary>
        /// Controls the trace output for the Test components
        /// </summary>
        static public TraceSwitch TestSwitch;

        /// <summary>
        /// Control the USB logging domain
        /// </summary>
        static public TraceSwitch UsbSwitch;

        /// <summary>
        /// Controls the output of stack information. If set the Method , filename and linenumber
        /// will be output for each call to the Log methods.
        /// </summary>
        static public TraceSwitch StackSwtich;

        #endregion // Public Data

        #region Public Methods

        /// <summary>
        /// Routine through which a client logs a trace message
        /// </summary>
        /// <remarks>
        ///     <para>
        ///     </para>
        /// </remarks>
        /// <param name="ts"> A <see cref="System.Diagnostics.TraceSwitch"/> which controls trace output for this message </param>
        /// <param name="tl"> A <see cref="System.Diagnostics.TraceLevel"/> which controls trace output for this message </param>
        /// <param name="message"> A <see cref="System.string"/> containing the message </param>
        /// <exception cref="ArgumentNullException"> If <paramref name="message"/> is <c>null</c>.</exception>
        [Conditional("TRACE")]
        static public void LogMessage(TraceSwitch ts, TraceLevel tl, string message)
        {
            if (message != null)
            {
                // Check that the specified switch is active as the requested level
                if (ts.Level >= tl)
                {
                    // We are good to trace!
                    Trace.WriteLineIf(ts.Level >= tl, _formatMessage(ts, tl, message));
                }
            }
            else
            {
                throw (new ArgumentNullException("message"));
            }
        }

        /// <summary>
        /// Routine through which a client logs an exception
        /// </summary>
        /// <remarks>
        ///     <para>
        ///     </para>
        /// </remarks>
        /// <param name="ts"> A <see cref="System.Diagnostics.TraceSwitch"/> which controls trace output for this message </param>
        /// <param name="tl"> A <see cref="System.Diagnostics.TraceLevel"/> which controls trace output for this message </param>
        /// <param name="message"> A <see cref="System.string"/> containing the message </param>
        /// <exception cref="ArgumentNullException"> If <paramref name="message"/> is <c>null</c>.</exception>
        [Conditional("TRACE")]
        static public void LogException(TraceSwitch ts, Exception ex, string optionalMessage)
        {
            if (ex != null)
            {
                var sb = new StringBuilder();

                sb.Append(Environment.NewLine);
                sb.Append("***** EXCEPTION *****");
                sb.Append(Environment.NewLine);

                if (string.IsNullOrEmpty(optionalMessage) == false)
                {
                    sb.Append(optionalMessage);
                    sb.Append(Environment.NewLine);
                }

                // Obtain a stack trace object for the exception (this is far more than
                // is contained in the stack trace property of the Exception object!)
                var trace = new StackTrace(ex, true);
                    
                sb.Append(string.Format("Message:{0}{1}Stack Trace:{2}",
                                            ex.Message,
                                            Environment.NewLine,
                                            trace.ToString()));

                sb.Append("***** END EXCEPTION *****");

                Trace.WriteLine(_formatMessage(ts, TraceLevel.Error, sb.ToString()));
            }
            else
            {
                throw (new ArgumentNullException("ex"));
            }
        }

        /// <summary>
        /// Stop all diagnostic logging
        /// </summary>
        public static void Stop()
        {
            Trace.Close();
        }

        #endregion
    }
}
