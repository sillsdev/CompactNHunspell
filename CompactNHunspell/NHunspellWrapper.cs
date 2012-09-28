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
    /// Nhunspell wrapper.
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
        private IHunspell speller;

        /// <summary>
        /// Initializes a new instance of the <see cref="CompactNHunspell.NHunspellWrapper"/> class.
        /// </summary>
        /// <param name='affFile'>
        /// Aff file.
        /// </param>
        /// <param name='dictFile'>
        /// Dict file.
        /// </param>
        public NHunspellWrapper(string affFile, string dictFile)
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
                return this.speller == null;
            }
        }
  
        /// <summary>
        /// Load the specified affFile and dictFile.
        /// </summary>
        /// <param name='affFile'>
        /// Aff file.
        /// </param>
        /// <param name='dictFile'>
        /// Dict file.
        /// </param>
        /// <exception cref='FileNotFoundException'>
        /// Is thrown when a file path argument specifies a file that does not exist.
        /// </exception>
        public void Load(string affFile, string dictFile)
        {
            affFile = Path.GetFullPath(affFile);
            if (!File.Exists(affFile))
            {
                throw new FileNotFoundException("AFF File not found: " + affFile);
            }

            dictFile = Path.GetFullPath(dictFile);
            if (!File.Exists(dictFile))
            {
                throw new FileNotFoundException("DIC File not found: " + dictFile);
            }
            
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
            
            if (this.speller != null)
            {
                this.speller.Init(affFile, dictFile);
            }
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
            if (this.speller == null)
            {
                throw new InvalidOperationException("Speller not initialized");
            }
            
            if (!this.cachedWords.ContainsKey(word))
            {
                var result = this.speller.Spell(word);
                this.cachedWords[word] = result;
            }
         
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
            if (!this.IsDisposed)
            {
                if (this.speller != null)
                {
                    this.speller.Free();
                    this.speller = null;
                }
            }
        }
    }
}