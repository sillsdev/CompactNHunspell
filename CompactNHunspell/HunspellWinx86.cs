//------------------------------------------------------------------------------
// <copyright 
//  file="HunspellWinx86.cs" 
//  company="enckse">
//  Copyright (c) All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
namespace CompactNHunspell
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.InteropServices;
 
    /// <summary>
    /// Hunspell Windows x86
    /// </summary>
    internal class HunspellWinx86 : BaseHunspell
    {
        /// <summary>
        /// Library for the reference assembly
        /// </summary>
        private const string Library = "Hunspellx86.dll";

        /// <inheritdoc />
        protected override BaseHunspell.FreeHandle FreeResource
        {
            get { return HunspellFree; }
        }

        /// <inheritdoc />
        protected override BaseHunspell.CreateHandle CreateResource
        {
            get { return (x, y) => this.Initialize(x, y, HunspellInit); }
        }

        /// <inheritdoc />
        protected override BaseHunspell.SpellCheck CheckSpelling
        {
            get { return HunspellSpell; }
        }

        /// <inheritdoc />
        protected override BaseHunspell.AddWord AddDictionaryWord
        {
            get { return HunspellAdd; }
        }

        /// <summary>
        /// Hunspell free.
        /// </summary>
        /// <param name='handle'>
        /// Handle to release.
        /// </param>
        [DllImport(Library)]
        public static extern void HunspellFree(IntPtr handle);
        
        /// <summary>
        /// Initializes Hunspell.
        /// </summary>
        /// <returns>
        /// Pointer to the instance
        /// </returns>
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
        /// Key if encrypted.
        /// </param>
        [DllImport(Library)]
        public static extern IntPtr HunspellInit([MarshalAs(UnmanagedType.LPArray)] byte[] affixData, IntPtr affixDataSize, [MarshalAs(UnmanagedType.LPArray)] byte[] dictionaryData, IntPtr dictionaryDataSize, [MarshalAs(UnmanagedType.LPWStr)] string key);
        
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
        [DllImport(Library)]
        public static extern bool HunspellSpell(IntPtr handle, [MarshalAs(UnmanagedType.LPWStr)] string word);
        
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
        [DllImport(Library)]
        public static extern bool HunspellAdd(IntPtr handle, [MarshalAs(UnmanagedType.LPWStr)] string word);
    }
}
