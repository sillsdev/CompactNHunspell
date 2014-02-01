//------------------------------------------------------------------------------
// <copyright 
//  file="HunspellLinux.cs" 
//  company="enckse">
//  Copyright (c) All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
namespace CompactNHunspell
{
    using System;
    using System.IO;
    using System.Runtime.InteropServices;
    
    /// <summary>
    /// Hunspell Linux.
    /// </summary>
    /// <exception cref='InvalidOperationException'>
    /// Is thrown when an operation cannot be performed because the process is not initialized
    /// </exception>
    internal class HunspellLinux : BaseHunspell
    {
        /// <summary>
        /// Lib Hunspell 1.3
        /// </summary>
        private const string LibHunspell = "libhunspell-1.3.so.0";

        /// <summary>
        /// Hunspell free.
        /// </summary>
        /// <param name='handle'>
        /// Handle to release.
        /// </param>
        [DllImport(LibHunspell)]
        public static extern void Hunspell_destroy(IntPtr handle);
  
        /// <summary>
        /// Hunspell construction
        /// </summary>
        /// <returns>Instance pointer</returns>
        /// <param name='affixData'>
        /// Affix data.
        /// </param>
        /// <param name='dictionaryData'>
        /// Dictionary data.
        /// </param>
        [DllImport(LibHunspell)]
        public static extern IntPtr Hunspell_create(string affixData, string dictionaryData);
  
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
        [DllImport(LibHunspell)]
        public static extern bool Hunspell_spell(IntPtr handle, string word);
       
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
        [DllImport(LibHunspell)]
        public static extern bool Hunspell_add(IntPtr handle, string word);
        
        /// <summary>
        /// Free the specified handle.
        /// </summary>
        /// <param name='handle'>
        /// Handle to free.
        /// </param>
        protected override void Free(IntPtr handle)
        {
            this.WriteTraceMessage("Freeing pointer");
            this.WriteTraceMessage(handle.ToString());
            Hunspell_destroy(handle);
            this.WriteTraceMessage("Freeing pointer completed");
        }
        
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
        protected override IntPtr InitInstance(string affFile, string dictFile)
        {
            this.WriteTraceMessage("Creating the instance");
            this.WriteTraceMessage(affFile);
            this.WriteTraceMessage(dictFile);
            return Hunspell_create(affFile, dictFile);
        }
  
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
        protected override bool Spell(IntPtr handle, string word)
        {
            this.WriteTraceMessage("Performing spell check");
            this.WriteTraceMessage(handle.ToString());
            this.WriteTraceMessage(word);
            return Hunspell_spell(handle, word);
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
        protected override void AddWord(IntPtr pointer, string word)
        {
            this.WriteTraceMessage("Adding word");
            this.WriteTraceMessage(pointer.ToString());
            this.WriteTraceMessage(word);
            Hunspell_add(pointer, word);
            this.WriteTraceMessage("Word added");
        }
    }
}
