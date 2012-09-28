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
    using System.Text;
    using System.Runtime.InteropServices;

    internal class HunspellWinx64 : BaseHunspell
    {
        [DllImport("Hunspellx64.dll")]
        public static extern void HunspellFree(IntPtr handle);

        [DllImport("Hunspellx64.dll")]
        public static extern IntPtr HunspellInit([MarshalAs(UnmanagedType.LPArray)] byte[] affixData,
            IntPtr affixDataSize,
            [MarshalAs(UnmanagedType.LPArray)] byte[] dictionaryData,
            IntPtr dictionaryDataSize,
            string key);

        [DllImport("Hunspellx64.dll")]
        public static extern bool HunspellSpell(IntPtr handle, [MarshalAs(UnmanagedType.LPWStr)] string word);

        protected override void Free(IntPtr handle)
        {
            HunspellFree(handle);
        }

        protected override IntPtr Init(byte[] affixData, IntPtr affixDataSize, byte[] dictionaryData, IntPtr dictionaryDataSize, string key)
        {
            return HunspellInit(affixData, affixDataSize, dictionaryData, dictionaryDataSize, key);
        }

        protected override bool Spell(IntPtr handle, string word)
        {
            return HunspellSpell(handle, word);
        }
    }
}
