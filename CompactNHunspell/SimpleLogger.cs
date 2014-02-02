//------------------------------------------------------------------------------
// <copyright 
//  file="SimpleLogger.cs" 
//  company="enckse">
//  Copyright (c) All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
namespace Common
{
    using System;
    using System.IO;
 
    /// <summary>
    /// Provides a buffer-backed (StringBuilder) logger
    /// </summary>
    internal class SimpleLogger
    {
        /// <summary>
        /// Flush the buffer to disk when it hits this length
        /// </summary>
        private const int FlushAt = 8192;

        /// <summary>
        /// Indicates if the buffer is in use for logging
        /// </summary>
        private bool useBuffer;

        /// <summary>
        /// Buffer file to save to
        /// </summary>
        private string bufferFile;

        /// <summary>
        /// Buffer diagnostic messages
        /// </summary>
        private System.Text.StringBuilder buffer = new System.Text.StringBuilder();

        /// <summary>
        /// Indicates if verbose console logging should be on
        /// </summary>
        private bool verbose;

        /// <summary>
        /// Initializes a new instance of the <see cref="Common.SimpleLogger"/> class.
        /// </summary>
        public SimpleLogger()
        {
            this.verbose = false;
            this.useBuffer = false;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Common.SimpleLogger"/> class.
        /// </summary>
        /// <param name='verboseOption'>Configuration key value to get a value indicating if verbose (console) logging is on (when true)</param>
        /// <param name='traceFileSetting'>Configuration key value to get a path to write a trace file to (appends to)</param>
        public SimpleLogger(string verboseOption, string traceFileSetting)
        {
            var verboseSetting = System.Configuration.ConfigurationManager.AppSettings[verboseOption];
            if (!string.IsNullOrEmpty(verboseSetting))
            {
                // This is being eaten, a failure case will just disable verbose logging
                bool.TryParse(verboseSetting, out this.verbose);
            }

            this.bufferFile = System.Configuration.ConfigurationManager.AppSettings[traceFileSetting];
            this.WriteDiagnostics(this.bufferFile);
            if (!string.IsNullOrEmpty(this.bufferFile))
            {
                this.useBuffer = true;
                this.WriteMessage(typeof(SimpleLogger), "Trace stream initialized");
            }
        }

        /// <summary>
        /// Write a trace message using a given type
        /// </summary>
        /// <param name='type'>Type requesting the write</param>
        /// <param name='message'>Trace/debug message</param>
        public void WriteMessage(Type type, string message)
        {
            var msg = CreateMessage(type, message);
            this.WriteDiagnostics(msg);
            if (this.useBuffer)
            {
                this.buffer.AppendLine(msg);
                this.FlushBuffer(false);
            }
        }

        /// <summary>
        /// Flush the buffer to disk
        /// </summary>
        public void Flush()
        {
            this.FlushBuffer(true);
        }

        /// <summary>
        /// Create a diagnostic message
        /// </summary>
        /// <param name='type'>Type requesting the write</param>
        /// <param name='message'>Trace/debug message</param>
        /// <returns>Diagnostic message to output</returns>
        private static string CreateMessage(Type type, string message)
        {
            return DateTime.Now.ToString("u") + " : " + type.Name + " -> " + message;
        }
   
        /// <summary>
        /// Write a trace message using a given type to the console (verbose diagnostics)
        /// </summary>
        /// <param name='message'>Trace/debug message</param>
        private void WriteDiagnostics(string message)
        {
            if (this.verbose)
            {
                System.Console.WriteLine(message);
            }
        }

        /// <summary>
        /// Flush the internal buffer to file
        /// </summary>
        /// <param name='force'>Force output writing</param>
        private void FlushBuffer(bool force)
        {
            try
            {
                if (this.useBuffer)
                {
                    if (this.buffer.Length > FlushAt || force)
                    {
                        this.buffer.AppendLine(CreateMessage(this.GetType(), "Flushing buffer"));
                        File.AppendAllText(this.bufferFile, this.buffer.ToString());
                        this.buffer.Length = 0;
                        this.buffer.Capacity = 0;
                    }
                }
            }
            catch
            {
                // Can't write to the diagnostic output file, send something to verbose and keep going
                this.WriteDiagnostics("Unable to flush buffer");
            }
        }
    }
}
