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
	using System.Diagnostics;
	using System.Text;
	using System.Collections.Generic;
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
		
		
		public void Init(string affFile, string dictFile)
		{
			process = new Process();
			process.StartInfo.UseShellExecute = false;
			process.StartInfo.RedirectStandardInput = true;
			process.StartInfo.FileName = "hunspell";
			process.StartInfo.RedirectStandardOutput = true;
			process.Start();
			process.OutputDataReceived += (sender, e) => 
			{
				lock (output)
				{
					output.Add(((DataReceivedEventArgs)e).Data);
				}
			};
			
			process.BeginOutputReadLine();
		}
		
		public bool Spell(string word)
		{
			if(this.process == null)
			{
				throw new InvalidOperationException("Not initialized");
			}
			
			process.StandardInput.WriteLine(word);
			bool isDone = false;
			bool isPass = false;
			while (!isDone){
				System.Threading.Thread.Sleep(100);
			lock (output)
			{
					foreach (var item in output.ToArray())
					{
						if(string.IsNullOrEmpty(item))
						{
							output.Clear();
							isDone = true;
						}
						else
						{
							isPass = !item.StartsWith("&");
						}
					}
			}
			}
			
			return isPass;
		}
		
		public void Free()
		{
			if(this.process != null){
				process.Kill();
			}
		}
	}
}

