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
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    
    /// <summary>
    /// Hunspell linux.
    /// </summary>
    /// <exception cref='InvalidOperationException'>
    /// Is thrown when an operation cannot be performed because the process is not initialized
    /// </exception>
    internal class HunspellLinux : IHunspell
    {
        /* *
         * This is not the proper way to call hunspell
         * Need to work with the libhunspell in the future
         * */
        
        /// <summary>
        /// The output from the process
        /// </summary>
        private IList<string> output = new List<string>();
        
        /// <summary>
        /// The process in which hunspell is running.
        /// </summary>
        private Process process = null;
        
        /// <summary>
        /// Init the specified affFile and dictFile to the process instance
        /// </summary>
        /// <param name='affFile'>
        /// Aff file.
        /// </param>
        /// <param name='dictFile'>
        /// Dict file.
        /// </param>
        public void Init(string affFile, string dictFile)
        {
            this.process = new Process();
            this.process.StartInfo.UseShellExecute = false;
            this.process.StartInfo.RedirectStandardInput = true;
            this.process.StartInfo.FileName = "hunspell";
            this.process.StartInfo.RedirectStandardOutput = true;
            this.process.Start();
            this.process.OutputDataReceived += (sender, e) => 
            {
                lock (this.output)
                {
                    this.output.Add(((DataReceivedEventArgs)e).Data);
                }
            };
            
            this.process.BeginOutputReadLine();
        }
        
        /// <summary>
        /// Check the spelling of a word
        /// </summary>
        /// <param name='word'>
        /// Word to check
        /// </param>
        /// <exception cref='InvalidOperationException'>
        /// Is thrown when an operation cannot be performed.
        /// </exception>
        /// <returns>True if the word is spelled properly</returns>
        public bool Spell(string word)
        {
            if (this.process == null)
            {
                throw new InvalidOperationException("Not initialized");
            }
            
            this.process.StandardInput.WriteLine(word);
            bool done = false;
            bool spelledCorrectly = false;
            while (!done)
            {
                System.Threading.Thread.Sleep(100);
                lock (this.output)
                {
                    foreach (var item in this.output.ToArray())
                    {
                        if (string.IsNullOrEmpty(item))
                        {
                            this.output.Clear();
                            done = true;
                        }
                        else
                        {
                            spelledCorrectly = !item.StartsWith("&");
                        }
                    }
                }
            }
            
            return spelledCorrectly;
        }
        
        /// <summary>
        /// Free this instance.
        /// </summary>
        public void Free()
        {
            if (this.process != null)
            {
                this.process.Kill();
            }
        }
    }
}