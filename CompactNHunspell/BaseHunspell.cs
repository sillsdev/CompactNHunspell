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
    /// Base hunspell implementation for spelling requests
    /// </summary>
    /// <exception cref='InvalidOperationException'>
    /// Is thrown when an operation cannot be performed because the speller is not initialized
    /// </exception>
    internal abstract class BaseHunspell : IHunspell
    {
        /// <summary>
        /// The handle to the process
        /// </summary>
        private IntPtr handle = IntPtr.Zero;
  
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
            if (handle == IntPtr.Zero)
            {
                throw new InvalidOperationException("Library not initialized");
            }

            return this.Spell(this.handle, word);
        }
  
        /// <summary>
        /// Init the specified affFile and dictFile to the instance.
        /// </summary>
        /// <param name='affFile'>
        /// Aff file to load
        /// </param>
        /// <param name='dictFile'>
        /// Dict file to load
        /// </param>
        public void Init(string affFile, string dictFile)
        {
            byte[] affixData;
            using (FileStream stream = File.OpenRead(affFile))
            {
                using (var reader = new BinaryReader(stream))
                {
                    affixData = reader.ReadBytes((int)stream.Length);
                }
            }

            byte[] dictionaryData;
            using (FileStream stream = File.OpenRead(dictFile))
            {
                using (var reader = new BinaryReader(stream))
                {
                    dictionaryData = reader.ReadBytes((int)stream.Length);
                }
            }
            
            this.handle = this.Init(affixData, new IntPtr(affixData.Length), dictionaryData, new IntPtr(dictionaryData.Length), null);
        }
  
        /// <summary>
        /// Free this instance.
        /// </summary>
        public void Free()
        {
            if (handle != IntPtr.Zero)
            {
                this.Free(this.handle);
            }
        }
  
        /// <summary>
        /// Free the specified handle.
        /// </summary>
        /// <param name='handle'>
        /// Handle to free.
        /// </param>
        protected abstract void Free(IntPtr handle);
  
        /// <summary>
        /// Init the specified affixData, affixDataSize, dictionaryData, dictionaryDataSize and key to the instance
        /// </summary>
        /// <param name='affixData'>
        /// Affix data.
        /// </param>
        /// <param name='affixDataSize'>
        /// Affix data size.
        /// </param>
        /// <param name='dictionaryData'>
        /// Dictionary data.
        /// </param>
        /// <param name='dictionaryDataSize'>
        /// Dictionary data size.
        /// </param>
        /// <param name='key'>
        /// Key (if encrypted)
        /// </param>
        /// <returns>Instance pointer</returns>
        protected abstract IntPtr Init([MarshalAs(UnmanagedType.LPArray)] byte[] affixData, IntPtr affixDataSize, [MarshalAs(UnmanagedType.LPArray)] byte[] dictionaryData, IntPtr dictionaryDataSize, string key);
  
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
        protected abstract bool Spell(IntPtr handle, [MarshalAs(UnmanagedType.LPWStr)] string word);
    }
}
