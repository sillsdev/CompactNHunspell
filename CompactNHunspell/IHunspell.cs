//------------------------------------------------------------------------------
// <copyright 
//  file="IHunspell.cs" 
//  company="enckse">
//  Copyright (c) All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
namespace CompactNHunspell
{
	/// <summary>
	/// Interface for making hunspell requests.
	/// </summary>
	internal interface IHunspell
	{
		/// <summary>
		/// Init the instance with aff and dict files
		/// </summary>
		/// <param name='affFile'>
		/// Aff file for input
		/// </param>
		/// <param name='dictFile'>
		/// Dict file for input
		/// </param>
		void Init(string affFile, string dictFile);
		
		/// <summary>
		/// Check if the word is spelled correctly
		/// </summary>
		/// <param name='word'>
		/// True if the word is spelled correctly
		/// </param>
		bool Spell(string word);
		
		/// <summary>
		/// Free this instance resources
		/// </summary>
		void Free();
	}
}

