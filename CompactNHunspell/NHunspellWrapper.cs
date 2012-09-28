//------------------------------------------------------------------------------
// <copyright 
//  file="NHunspellWrapper.cs" 
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

    public class NHunspellWrapper : IDisposable
    {
        private IHunspell speller;

        public NHunspellWrapper(string affFile, string dictFile)
        {
            Load(affFile, dictFile);
        }
		
        public bool IsDisposed
        {
            get
            {
                return this.speller == null;
            }
        }

        public void Load(string affFile, string dictFile)
        {
            affFile = Path.GetFullPath(affFile);
            if (!File.Exists(affFile))
            {
                throw new FileNotFoundException("AFF File not found: " + affFile);
            }

            dictFile = Path.GetFullPath(dictFile);
            if (!File.Exists(dictFile))
            {
                throw new FileNotFoundException("DIC File not found: " + dictFile);
            }
			
			if(System.Environment.OSVersion.Platform == PlatformID.Unix){
				this.speller = new HunspellLinux();
			}else{
            if (IntPtr.Size == 4)
            {
                this.speller = new HunspellWinx86();
            }
            else
            {
                this.speller = new HunspellWinx64();
            }
			}
			
			if(this.speller != null){
            this.speller.Init(affFile, dictFile);
			}
        }


        public bool Spell(string word)
        {
            if (this.speller == null)
            {
                throw new InvalidOperationException("Speller not initialized");
            }

            return this.speller.Spell(word);
        }

        public void Dispose()
        {
            if (!this.IsDisposed)
            {
                if (this.speller != null)
                {
                    this.speller.Free();
                    this.speller = null;
                }
            }
        }
    }
}