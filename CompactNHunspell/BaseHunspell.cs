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
    using System.IO;
    
    /// <summary>
    /// Base Hunspell implementation for spelling requests
    /// </summary>
    /// <exception cref='InvalidOperationException'>
    /// Is thrown when an operation cannot be performed because the speller is not initialized
    /// </exception>
    internal abstract class BaseHunspell : ISpeller
    {
        /// <summary>
        /// The handle to the process
        /// </summary>
        private IntPtr handle = IntPtr.Zero;

        /// <summary>
        /// Hunspell destroy/free/cleanup
        /// </summary>
        /// <param name='handle'>
        /// Handle to release.
        /// </param>
        protected delegate void FreeHandle(IntPtr handle);

        /// <summary>
        /// Create an instance
        /// </summary>
        /// <returns>Instance pointer</returns>
        /// <param name='affFile'>
        /// Affix data.
        /// </param>
        /// <param name='dictFile'>
        /// Dictionary data.
        /// </param>
        protected delegate IntPtr CreateHandle(string affFile, string dictFile);

        /// <summary>
        /// Initializes an instance.
        /// </summary>
        /// <returns>
        /// Pointer to the instance
        /// </returns>
        /// <param name='affData'>
        /// Affix data.
        /// </param>
        /// <param name='affPointer'>
        /// Affix data size.
        /// </param>
        /// <param name='dictData'>
        /// Dictionary data.
        /// </param>
        /// <param name='dictPointer'>
        /// Dictionary data size.
        /// </param>
        /// <param name='key'>
        /// Key if encrypted.
        /// </param>
        protected delegate IntPtr InitHandle(byte[] affData, IntPtr affPointer, byte[] dictData, IntPtr dictPointer, string key);

        /// <summary>
        /// Check the spelling of a word
        /// </summary>
        /// <returns>
        /// True if the word is spelled correctly
        /// </returns>
        /// <param name='handle'>
        /// Instance handle
        /// </param>
        /// <param name='word'>
        /// Word to check
        /// </param>
        protected delegate bool SpellCheck(IntPtr handle, string word);

        /// <summary>
        /// Add the word to the instance
        /// </summary>
        /// <param name='handle'>
        /// Instance handle
        /// </param>
        /// <param name='word'>
        /// Word to add
        /// </param>
        /// <returns>
        /// True if word is added
        /// </returns>
        protected delegate bool AddWord(IntPtr handle, string word);
        
        /// <summary>
        /// Gets or sets a trace function for diagnostic output
        /// </summary> 
        internal Action<Type, string> TraceFunction { get; set; }

        /// <summary>
        /// Gets the call to free the resource
        /// </summary>
        protected abstract FreeHandle FreeResource { get; }

        /// <summary>
        /// Gets the call to create the resource
        /// </summary>
        protected abstract CreateHandle CreateResource { get; }

        /// <summary>
        /// Gets the call to check spelling for a word
        /// </summary>
        protected abstract SpellCheck CheckSpelling { get; }

        /// <summary>
        /// Gets the call to add a word to the dictionary
        /// </summary>
        protected abstract AddWord AddDictionaryWord { get; }

        /// <inheritdoc />
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
            return this.CheckSpelling(this.handle, word);
        }
  
        /// <inheritdoc />
        public void Init(string affFile, string dictFile)
        {    
            this.WriteTrace("Init");
            this.WriteTrace(affFile);
            this.WriteTrace(dictFile);
            this.handle = this.CreateResource(affFile, dictFile);
            this.WriteTrace("Init is complete");
        }
  
        /// <inheritdoc />
        public void Free()
        {
            this.WriteTrace("Free");
            if (this.handle != IntPtr.Zero)
            {
                this.WriteTrace("Freeing handle");
                this.WriteTrace(this.handle.ToString());
                this.FreeResource(this.handle);
                this.WriteTrace("Done freeing handle");
            }
            
            this.WriteTrace("Free is complete");
        }
        
        /// <inheritdoc />
        public void Add(string word)
        {
            this.WriteTrace("Add");
            this.WriteTrace(word);
            if (this.handle != IntPtr.Zero)
            {
                this.WriteTrace("Adding word");
                bool wasAdded = this.AddDictionaryWord(this.handle, word);
                this.WriteTrace("Word was added? " + wasAdded);
            }
            
            this.WriteTrace("Add is complete");
        }
        
        /// <summary>
        /// Initialize routine
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
        /// <param name="handleInit">Handle initialize call</param>
        protected IntPtr Initialize(string affFile, string dictFile, InitHandle handleInit)
        {
            this.WriteTrace("Initialize");
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
            var affixDataPointer = new IntPtr(affixData.Length);
            this.WriteTrace(affixDataPointer.ToString());
            var dictDataPointer = new IntPtr(dictionaryData.Length);
            this.WriteTrace(dictDataPointer.ToString());
            return handleInit(affixData, affixDataPointer, dictionaryData, dictDataPointer, null);
        }

        /// <summary>
        /// Write a trace message as base
        /// </summary>
        /// <param name='message'>Trace/debug message</param>
        private void WriteTrace(string message)
        {
            if (this.TraceFunction != null)
            {
                this.TraceFunction(this.GetType(), message);
            }
        }
    }
}
