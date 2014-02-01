//------------------------------------------------------------------------------
// <copyright 
//  file="BaseHunspell.cs" 
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
    /// Base Hunspell implementation for spelling requests
    /// </summary>
    /// <exception cref='InvalidOperationException'>
    /// Is thrown when an operation cannot be performed because the speller is not initialized
    /// </exception>
    internal abstract class BaseHunspell
    {
        /// <summary>
        /// The handle to the process
        /// </summary>
        private IntPtr handle = IntPtr.Zero;

        /// <summary>
        /// Gets or sets a trace function for diagnostic output
        /// </summary> 
        public Action<Type, string> TraceFunction { get; set; }

        /// <summary>
        /// Check if the word is spelled correctly
        /// </summary>
        /// <param name='word'>
        /// Word to spell check.
        /// </param>
        /// <exception cref='InvalidOperationException'>
        /// Is thrown when an operation cannot be performed due to not initialized spell engine
        /// </exception>
        /// <returns>True if the word is spelled correctly</returns>
        public bool Spell(string word)
        {
            this.WriteTrace("Spell");
            this.WriteTrace(word);
            if (this.handle == IntPtr.Zero)
            {
                this.WriteTrace("Not initialized");
                throw new InvalidOperationException("Library not initialized");
            }

            this.WriteTrace("Checking");
            return this.Spell(this.handle, word);
        }
  
        /// <summary>
        /// Initialize the specified affix File and dict file to the instance.
        /// </summary>
        /// <param name='affFile'>
        /// Affix file to load
        /// </param>
        /// <param name='dictFile'>
        /// Dict file to load
        /// </param>
        public void Init(string affFile, string dictFile)
        {    
            this.WriteTrace("Init");
            this.WriteTrace(affFile);
            this.WriteTrace(dictFile);
            this.handle = this.InitInstance(affFile, dictFile);
            this.WriteTrace("Init is complete");
        }
  
        /// <summary>
        /// Free this instance.
        /// </summary>
        public void Free()
        {
            this.WriteTrace("Free");
            if (this.handle != IntPtr.Zero)
            {
                this.WriteTrace("Freeing handle");
                this.Free(this.handle);
            }
            
            this.WriteTrace("Free is complete");
        }
        
        /// <summary>
        /// Add the specified word to the internal dictionary
        /// </summary>
        /// <param name='word'>
        /// Word to add to the dictionary.
        /// </param>
        public void Add(string word)
        {
            this.WriteTrace("Add");
            this.WriteTrace(word);
            if (this.handle != IntPtr.Zero)
            {
                this.WriteTrace("Adding word");
                this.AddWord(this.handle, word);
            }
            
            this.WriteTrace("Add is complete");
        }
        
        /// <summary>
        /// Adds the word to the dictionary
        /// </summary>
        /// <param name='pointer'>
        /// Pointer to the instance
        /// </param>
        /// <param name='word'>
        /// Word to add
        /// </param>
        protected abstract void AddWord(IntPtr pointer, string word);
        
        /// <summary>
        /// Windows initialize routine
        /// </summary>
        /// <returns>
        /// Pointer to the instance
        /// </returns>
        /// <param name='affFile'>
        /// Affix file.
        /// </param>
        /// <param name='dictFile'>
        /// Dict file.
        /// </param>
        protected IntPtr WindowsInit(string affFile, string dictFile)
        {
            this.WriteTrace("WindowsInit");
            this.WriteTrace(affFile);
            this.WriteTrace(dictFile);
            byte[] affixData;
            using (FileStream stream = File.OpenRead(affFile))
            {
                using (var reader = new BinaryReader(stream))
                {
                    affixData = reader.ReadBytes((int)stream.Length);
                }
            }

            this.WriteTrace("Read affFile successfully");
            byte[] dictionaryData;
            using (FileStream stream = File.OpenRead(dictFile))
            {
                using (var reader = new BinaryReader(stream))
                {
                    dictionaryData = reader.ReadBytes((int)stream.Length);
                }
            }
            
            this.WriteTrace("Read dictFile successfully");
            return this.DataInvoke(affixData, new IntPtr(affixData.Length), dictionaryData, new IntPtr(dictionaryData.Length));
        }
        
        /// <summary>
        /// Provides an invoke initializing with data streams instead of file names
        /// </summary>
        /// <returns>
        /// The instance pointer
        /// </returns>
        /// <param name='affixData'>
        /// Affix data.
        /// </param>
        /// <param name='affixSize'>
        /// Affix size.
        /// </param>
        /// <param name='dictData'>
        /// Dictionary data.
        /// </param>
        /// <param name='dictSize'>
        /// Dictionary size.
        /// </param>
        protected virtual IntPtr DataInvoke(byte[] affixData, IntPtr affixSize, byte[] dictData, IntPtr dictSize)
        {
            this.WriteTrace("DataInvoke");
            return IntPtr.Zero;
        }
  
        /// <summary>
        /// Free the specified handle.
        /// </summary>
        /// <param name='handle'>
        /// Handle to free.
        /// </param>
        protected abstract void Free(IntPtr handle);
  
        /// <summary>
        /// Initializes the instance.
        /// </summary>
        /// <returns>
        /// The instance.
        /// </returns>
        /// <param name='affFile'>
        /// Affix file.
        /// </param>
        /// <param name='dictFile'>
        /// Dict file.
        /// </param>
        protected abstract IntPtr InitInstance(string affFile, string dictFile);
  
        /// <summary>
        /// Spell check the word
        /// </summary>
        /// <param name='handle'>
        /// Handle to use
        /// </param>
        /// <param name='word'>
        /// Word to check
        /// </param>
        /// <returns>True if the word is properly spelled</returns>
        protected abstract bool Spell(IntPtr handle, string word);

        /// <summary>
        /// Write a trace message using the speller type
        /// </summary>
        /// <param name='message'>Trace/debug message</param>
        protected void WriteTraceMessage(string message)
        {
            this.WriteTrace(this.GetType(), message);
        }

        /// <summary>
        /// Write a trace message using a given type
        /// </summary>
        /// <param name='type'>Type requesting the write</param>
        /// <param name='message'>Trace/debug message</param>
        private void WriteTrace(Type type, string message)
        {
            if (this.TraceFunction != null)
            {
                this.TraceFunction(type, message);
            }
        }

        /// <summary>
        /// Write a trace message as base
        /// </summary>
        /// <param name='message'>Trace/debug message</param>
        private void WriteTrace(string message)
        {
            this.WriteTrace(typeof(BaseHunspell), message);
        }
    }
}
