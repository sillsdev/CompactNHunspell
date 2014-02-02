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
    using Common;
 
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
        /// The cached words and their status
        /// </summary>
        private IDictionary<string, bool> cachedWords = new Dictionary<string, bool>(StringComparer.CurrentCulture);
        
        /// <summary>
        /// The speller for spell checking
        /// </summary>
        private BaseHunspell speller;

        /// <summary>
        /// Simple buffer-backed logger
        /// </summary>
        private SimpleLogger logger = new SimpleLogger();

        /// <summary>
        /// Given log action (use instead of the logger action)
        /// </summary>
        private Action<Type, string> logAction = null;

        /// <summary>
        /// Initializes a new instance of the <see cref="CompactNHunspell.NHunspellWrapper"/> class.
        /// <remarks>Load must be called when using this constructor</remarks>
        /// </summary>
        public NHunspellWrapper()
        {
            this.logger = new SimpleLogger("CompactNHunspell");
        }

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
           : this()
        {
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
        /// Gets or sets the logging action to use
        /// </summary>
        public Action<Type, string> LogAction
        {
            get
            {
                var action = this.logAction;
                if (this.logger.Restricted || action == null)
                {
                    action = this.logger.WriteMessage;
                }
     
                return action;
            }

            set
            {
                this.logAction = value;
            }
        }

        /// <summary>
        /// Clear the underlying cache of previously checked words 
        /// </summary>
        public void Clear()
        {
            this.cachedWords.Clear();
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
                this.logger.Flush();
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
        /// Write a trace message using a given type
        /// </summary>
        /// <param name='type'>Type requesting the write</param>
        /// <param name='message'>Trace/debug message</param>
        private void WriteMessage(Type type, string message)
        {
            this.LogAction(type, message);
        }

        /// <summary>
        /// Write a trace message using a given type
        /// </summary>
        /// <param name='message'>Trace/debug message</param>
        private void WriteMessage(string message)
        {
            this.WriteMessage(typeof(NHunspellWrapper), message);
        }
    }
}
