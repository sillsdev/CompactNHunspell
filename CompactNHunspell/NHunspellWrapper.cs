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
    using System.Linq;
    using Common;

    /// <summary>
    /// Log a message
    /// </summary>
    /// <param name="type">Logging type</param>
    /// <param name="message">Message to log</param>
    public delegate void Log(Type type, string message);
        
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
        /// App configuration key for this library
        /// </summary>
        private const string SettingsKey = "CompactNHunspell";

        /// <summary>
        /// Default cache size to use (if not set)
        /// </summary>
        private const int DefaultCacheSize = int.MaxValue;

        /// <summary>
        /// Value that indicates if the cache is disabled
        /// </summary>
        private const int CacheDisabled = 1;

        /// <summary>
        /// The cached words and their status
        /// </summary>
        private IDictionary<string, bool> cachedWords = new Dictionary<string, bool>(StringComparer.CurrentCulture);
        
        /// <summary>
        /// The speller for spell checking
        /// </summary>
        private ISpeller speller;

        /// <summary>
        /// Simple buffer-backed logger
        /// </summary>
        private SimpleLogger logger = new SimpleLogger();

        /// <summary>
        /// Given log action (use instead of the logger action)
        /// </summary>
        private Log logAction = null;

        /// <summary>
        /// Backing field to given an override type for instancing the underlying spell checker
        /// </summary>
        private string overridenType = null;

        /// <summary>
        /// Cache size at which the internal cache is entirely cleared and reset
        /// </summary>
        private int cacheSize = DefaultCacheSize;

        /// <summary>
        /// Indicates if the instance is disposed
        /// </summary>
        private bool disposed = false;

        /// <summary>
        /// Initializes a new instance of the <see cref="CompactNHunspell.NHunspellWrapper"/> class.
        /// <remarks>Load must be called when using this constructor</remarks>
        /// </summary>
        public NHunspellWrapper()
        {
            this.logger = new SimpleLogger(SettingsKey);
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
           : this(affFile, dictFile, null)
        {
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
        /// <param name='overrideType'>Override type</param>
        /// <param name='logAction'>Logging action</param>
        public NHunspellWrapper(string affFile, string dictFile, string overrideType, Log logAction)
           : this()
        {
            if (logAction != null)
            {
                this.LogAction = logAction;
            }
 
            this.overridenType = overrideType;
            this.Load(affFile, dictFile);
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
        /// <param name='overrideType'>Override type</param>
        public NHunspellWrapper(string affFile, string dictFile, string overrideType)
            : this(affFile, dictFile, overrideType, null)
        {
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
                return this.speller == null && this.disposed;
            }
        }

        /// <summary>
        /// Gets or sets the size at which the internal cache will automatically be cleared. Defaults to int.MaxValue
        /// <remarks>Set this value to 0 to disable cache flushing entirely</remarks>
        /// <remarks>Set this value to 1 to disable the use of the cache at all</remarks>
        /// </summary>
        public int CacheSize
        {
            get
            {
                return this.cacheSize;
            }

            set
            {
                if (value > 0)
                {
                    this.cacheSize = value;
                }
                else
                {
                    this.cacheSize = 0;
                }
            }
        }

        /// <summary>
        /// Gets or sets an override type name to use to create the underlying spell checker
        /// </summary>
        public string OverrideType 
        {
            get
            {
                if (string.IsNullOrEmpty(this.overridenType))
                {
                    return System.Configuration.ConfigurationManager.AppSettings[string.Format("{0}.{1}", SettingsKey, "OverrideType")];
                }
                else
                {
                    return this.overridenType;
                }
            }

            set
            {
                this.overridenType = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether to disable the cache or not
        /// </summary>
        public bool DisableCache
        {
            get
            {
                return this.CacheSize == CacheDisabled;
            }

            set
            {
                if (value)
                {
                    this.CacheSize = CacheDisabled;
                }
                else
                {
                    this.CacheSize = DefaultCacheSize;
                }
            }
        }

        /// <summary>
        /// Gets or sets the logging action to use
        /// </summary>
        public Log LogAction
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
            var overrideType = this.OverrideType;
            if (string.IsNullOrEmpty(overrideType))
            {
                if (System.Environment.OSVersion.Platform == PlatformID.Unix || System.Environment.OSVersion.Platform == PlatformID.MacOSX)
                {
                    overrideType = typeof(HunspellLinux).FullName;
                }
                else
                {
                    if (IntPtr.Size == 4)
                    {
                        overrideType = typeof(HunspellWinx86).FullName;
                    }
                    else
                    {
                        overrideType = typeof(HunspellWinx64).FullName;
                    }
                }
            }

            this.WriteMessage("Using type: " + overrideType);
            Type instanceType = Type.GetType(overrideType);
            if (instanceType == null || !instanceType.GetInterfaces().Contains(typeof(ISpeller)))
            {
                throw new ArgumentException("Invalid Hunspell instance type");
            }

            this.speller = (ISpeller)Activator.CreateInstance(instanceType);
            this.WriteMessage("Spell check instance created");
            if (this.speller != null)
            {
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
            
            // Dump the cache once it goes over the cache size (or if the cache size is 1, always dump the cache)
            if (this.CacheSize > 0 && (this.cachedWords.Count > this.CacheSize || this.CacheSize == CacheDisabled))
            {
                this.WriteMessage("Dumping the cache");
                this.Clear();
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
            this.Dispose(true);
            GC.SuppressFinalize(this);
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
        /// Dispose of the instance
        /// </summary>
        /// <param name="disposing">True when called from Dispose()</param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.WriteMessage("Disposing");
                if (!this.disposed)
                {
                    if (this.speller != null)
                    {
                        this.speller.Free();
                        this.speller = null;
                    }

                    this.WriteMessage("About to close output writer");
                    this.logger.Flush();
                    this.disposed = true;
                }
            }
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
