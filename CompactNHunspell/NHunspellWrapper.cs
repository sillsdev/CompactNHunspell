//------------------------------------------------------------------------------
// <copyright 
//  file="NHunspellWrapper.cs" 
//  company="enckse">
//  Copyright (c) All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
namespace CompactNHunspell
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Runtime.InteropServices;
 
    /// <summary>
    /// NHunspell wrapper.
    /// </summary>
    /// <exception cref='FileNotFoundException'>
    /// Is thrown when a file path argument specifies a file that does not exist.
    /// </exception>
    /// <exception cref='InvalidOperationException'>
    /// Is thrown when an operation cannot be performed.
    /// </exception>
    public class NHunspellWrapper : IDisposable
    {
        /// <summary>
        /// Flush the buffer to disk when it hits this length
        /// </summary>
        private const int FlushAt = 8192;

        /// <summary>
        /// The cached words and their status
        /// </summary>
        private IDictionary<string, bool> cachedWords = new Dictionary<string, bool>(StringComparer.CurrentCulture);
        
        /// <summary>
        /// The speller for spell checking
        /// </summary>
        private BaseHunspell speller;

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
        /// Initializes a new instance of the <see cref="CompactNHunspell.NHunspellWrapper"/> class.
        /// </summary>
        /// <param name='affFile'>
        /// Affix file.
        /// </param>
        /// <param name='dictFile'>
        /// Dict file.
        /// </param>
        public NHunspellWrapper(string affFile, string dictFile)
        {
            var verboseSetting = System.Configuration.ConfigurationManager.AppSettings["CompactNHunspell.Verbose"];
            if (!string.IsNullOrEmpty(verboseSetting))
            {
                // This is being eaten, a failure case will just disable verbose logging
                bool.TryParse(verboseSetting, out this.verbose);
            }

            this.bufferFile = System.Configuration.ConfigurationManager.AppSettings["CompactNHunspell.TraceFile"];
            this.WriteDiagnostics(this.bufferFile);
            if (!string.IsNullOrEmpty(this.bufferFile))
            {
                this.useBuffer = true;
                this.WriteMessage("Trace stream initialized");
            }

            this.Load(affFile, dictFile);
        }
        
        /// <summary>
        /// Gets a value indicating whether this instance is disposed.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is disposed; otherwise, <c>false</c>.
        /// </value>
        public bool IsDisposed
        {
            get
            {
                this.WriteMessage("IsDisposed");
                return this.speller == null;
            }
        }
  
        /// <summary>
        /// Load the specified affixFile and dictFile.
        /// </summary>
        /// <param name='affFile'>
        /// Affix file.
        /// </param>
        /// <param name='dictFile'>
        /// Dict file.
        /// </param>
        /// <exception cref='FileNotFoundException'>
        /// Is thrown when a file path argument specifies a file that does not exist.
        /// </exception>
        public void Load(string affFile, string dictFile)
        {
            this.WriteMessage("Load");
            this.WriteMessage(affFile);
            this.WriteMessage(dictFile);
            affFile = Path.GetFullPath(affFile);
            if (!File.Exists(affFile))
            {
            this.WriteMessage("affFile not found");
                throw new FileNotFoundException("AFF File not found: " + affFile);
            }

            dictFile = Path.GetFullPath(dictFile);
            if (!File.Exists(dictFile))
            {
            this.WriteMessage("dictFile not found");
                throw new FileNotFoundException("DIC File not found: " + dictFile);
            }
            
            this.WriteMessage("Files found, creating spell check instance");
            if (System.Environment.OSVersion.Platform == PlatformID.Unix) 
            {
                this.speller = new HunspellLinux();
            }
            else
            {
                if (IntPtr.Size == 4)
                {
                    this.speller = new HunspellWinx86();
                }
                else
                {
                    this.speller = new HunspellWinx64();
                }
            }

            this.WriteMessage("Spell check instance created");
            if (this.speller != null)
            {
                // Attach the trace function
                this.speller.TraceFunction = (x, y) => { this.WriteMessage(x, y); };            
                this.speller.Init(affFile, dictFile);
            }

            this.WriteMessage("Load is complete");
        }

        /// <summary>
        /// Checks if a word is spelled correctly
        /// </summary>
        /// <param name='word'>
        /// Word to check.
        /// </param>
        /// <exception cref='InvalidOperationException'>
        /// Is thrown when an operation cannot be performed.
        /// </exception>
        /// <returns>True if the word is spelled correctly</returns>
        public bool Spell(string word)
        {
            this.WriteMessage("Spell");
            this.WriteMessage(word);
            if (this.speller == null)
            {
                this.WriteMessage("No spell check instance");
                throw new InvalidOperationException("Speller not initialized");
            }
            
            this.WriteMessage("Checking cache");
            if (!this.cachedWords.ContainsKey(word))
            {
                this.WriteMessage("Not cached");
                var result = this.speller.Spell(word);
                this.cachedWords[word] = result;
            }
         
            this.WriteMessage("Spell is complete");
            return this.cachedWords[word];
        }
  
        /// <summary>
        /// Releases all resource used by the <see cref="CompactNHunspell.NHunspellWrapper"/> object.
        /// </summary>
        /// <remarks>
        /// Call <see cref="Dispose"/> when you are finished using the <see cref="CompactNHunspell.NHunspellWrapper"/>.
        /// The <see cref="Dispose"/> method leaves the <see cref="CompactNHunspell.NHunspellWrapper"/> in an unusable
        /// state. After calling <see cref="Dispose"/>, you must release all references to the
        /// <see cref="CompactNHunspell.NHunspellWrapper"/> so the garbage collector can reclaim the memory that the
        /// <see cref="CompactNHunspell.NHunspellWrapper"/> was occupying.
        /// </remarks>
        public void Dispose()
        {
            this.WriteMessage("Disposing");
            if (!this.IsDisposed)
            {
                if (this.speller != null)
                {
                    this.speller.Free();
                    this.speller = null;
                }

                this.WriteMessage("About to close output writer");
                this.FlushBuffer(true);
            }
        }
        
        /// <summary>
        /// Add the specified word to the dictionary
        /// </summary>
        /// <param name='word'>
        /// Word to add
        /// </param>
        /// <exception cref='InvalidOperationException'>
        /// Is thrown when an operation cannot be performed.
        /// </exception>
        public void Add(string word)
        {
            this.WriteMessage("Add");
            this.WriteMessage(word);
            if (this.speller == null)
            {
                this.WriteMessage("No spell check instance");
                throw new InvalidOperationException("Speller not initialized");
            }
            
            this.speller.Add(word);
            this.WriteMessage("Add is complete");
        }

        /// <summary>
        /// Create a diagnostic message
        /// </summary>
        /// <param name='type'>Type requesting the write</param>
        /// <param name='message'>Trace/debug message</param>
        /// <returns>Diagnostic message to output</returns>
        private static string CreateMessage(Type type, string message)
        {
            return type.Name + " -> " + message;
        }

        /// <summary>
        /// Write a trace message using a given type
        /// </summary>
        /// <param name='type'>Type requesting the write</param>
        /// <param name='message'>Trace/debug message</param>
        private void WriteMessage(Type type, string message)
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
        /// Write a trace message using a given type
        /// </summary>
        /// <param name='message'>Trace/debug message</param>
        private void WriteMessage(string message)
        {
            this.WriteMessage(typeof(NHunspellWrapper), message);
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
