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
    internal abstract class BaseHunspell
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
            if (this.handle == IntPtr.Zero)
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
            this.handle = this.InitInstance(affFile, dictFile);
        }
  
        /// <summary>
        /// Free this instance.
        /// </summary>
        public void Free()
        {
            if (this.handle != IntPtr.Zero)
            {
                this.Free(this.handle);
            }
        }
        
        /// <summary>
        /// Windows initialize routine
        /// </summary>
        /// <returns>
        /// IntPtr to the instance
        /// </returns>
        /// <param name='affFile'>
        /// Aff file.
        /// </param>
        /// <param name='dictFile'>
        /// Dict file.
        /// </param>
        protected IntPtr WindowsInit(string affFile, string dictFile)
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
            
            return this.DataInvoke(affixData, new IntPtr(affixData.Length), dictionaryData, new IntPtr(dictionaryData.Length));
        }
        
        /// <summary>
        /// Provides an invoke init with data streams instead of file names
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
        /// Inits the instance.
        /// </summary>
        /// <returns>
        /// The instance.
        /// </returns>
        /// <param name='affFile'>
        /// Aff file.
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
    }
}
