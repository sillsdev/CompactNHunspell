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
    using System.Runtime.InteropServices;
    
    /// <summary>
    /// Hunspell Linux.
    /// </summary>
    internal class HunspellLinux : BaseHunspell
    {
        /// <summary>
        /// Lib Hunspell 1.3
        /// </summary>
        private const string LibHunspell = "libhunspell-1.3.so.0";

        /// <inheritdoc />
        protected override BaseHunspell.FreeHandle FreeResource
        {
            get { return Hunspell_destroy; }
        }

        /// <inheritdoc />
        protected override BaseHunspell.CreateHandle CreateResource
        {
            get { return Hunspell_create; }
        }

        /// <inheritdoc />
        protected override BaseHunspell.SpellCheck CheckSpelling
        {
            get { return Hunspell_spell; }
        }

        /// <inheritdoc />
        protected override BaseHunspell.AddWord AddDictionaryWord
        {
            get { return Hunspell_add; }
        }

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
    }
}
