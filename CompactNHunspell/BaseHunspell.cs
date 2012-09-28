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
    using System.Linq;
    using System.Text;
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
        private IntPtr handle = IntPtr.Zero;

        public bool Spell(string word)
        {
            if (handle == IntPtr.Zero)
            {
                throw new InvalidOperationException("Library not initialized");
            }

            return this.Spell(this.handle, word);
        }

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

        public void Free()
        {
            if (handle != IntPtr.Zero)
            {
                this.Free(this.handle);
            }
        }

        protected abstract void Free(IntPtr handle);

        protected abstract IntPtr Init([MarshalAs(UnmanagedType.LPArray)] byte[] affixData,
            IntPtr affixDataSize,
            [MarshalAs(UnmanagedType.LPArray)] byte[] dictionaryData,
            IntPtr dictionaryDataSize,
            string key);

        protected abstract bool Spell(IntPtr handle, [MarshalAs(UnmanagedType.LPWStr)] string word);

    }
}
