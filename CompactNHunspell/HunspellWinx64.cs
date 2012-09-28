//------------------------------------------------------------------------------
// <copyright 
//  file="HunspellWinx64.cs" 
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
    /// Hunspell for windows x64
    /// </summary>
    internal class HunspellWinx64 : BaseHunspell
    {
        /// <summary>
        /// Hunspell free.
        /// </summary>
        /// <param name='handle'>
        /// Handle to release.
        /// </param>
        [DllImport("Hunspellx64.dll")]
        public static extern void HunspellFree(IntPtr handle);
  
        /// <summary>
        /// Init hunspell.
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
        [DllImport("Hunspellx64.dll")]
        public static extern IntPtr HunspellInit([MarshalAs(UnmanagedType.LPArray)] byte[] affixData, IntPtr affixDataSize, [MarshalAs(UnmanagedType.LPArray)] byte[] dictionaryData, IntPtr dictionaryDataSize, string key);
  
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
        [DllImport("Hunspellx64.dll")]
        public static extern bool HunspellSpell(IntPtr handle, [MarshalAs(UnmanagedType.LPWStr)] string word);
        
        /// <summary>
        /// Free the specified handle.
        /// </summary>
        /// <param name='handle'>
        /// Handle to free.
        /// </param>
        protected override void Free(IntPtr handle)
        {
            HunspellFree(handle);
        }
        
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
        protected override IntPtr InitInstance(string affFile, string dictFile)
        {
            return this.WindowsInit(affFile, dictFile);
        }
  
        /// <summary>
        /// Invoke the instance with data
        /// </summary>
        /// <returns>
        /// The instance pointer.
        /// </returns>
        /// <param name='affixData'>
        /// Affix data.
        /// </param>
        /// <param name='affixSize'>
        /// Affix size.
        /// </param>
        /// <param name='dictData'>
        /// Dict data.
        /// </param>
        /// <param name='dictSize'>
        /// Dict size.
        /// </param>
        protected override IntPtr DataInvoke(byte[] affixData, IntPtr affixSize, byte[] dictData, IntPtr dictSize)
        {
            return HunspellInit(affixData, affixSize, dictData, dictSize, null);
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
            return HunspellSpell(handle, word);
        }
    }
}