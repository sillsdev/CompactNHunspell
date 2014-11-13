//------------------------------------------------------------------------------
// <copyright 
//  file="ISpeller.cs" 
//  company="enckse">
//  Copyright (c) All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
namespace CompactNHunspell
{
    /// <summary>
    /// Speller definition
    /// </summary>
    public interface ISpeller
    {
        /// <summary>
        /// Initialize the specified affix File and dict file to the instance.
        /// </summary>
        /// <param name='affFile'>
        /// Affix file to load
        /// </param>
        /// <param name='dictFile'>
        /// Dict file to load
        /// </param>
        void Init(string affFile, string dictFile);

        /// <summary>
        /// Check if the word is spelled correctly
        /// </summary>
        /// <param name='word'>
        /// Word to spell check.
        /// </param>
        /// <exception cref='InvalidOperationException'>
        /// Is thrown when an operation cannot be performed due to not initialized spell engine
        /// </exception>
        /// <returns>True if the word is spelled correctly</returns>
        bool Spell(string word);

        /// <summary>
        /// Add the specified word to the internal dictionary
        /// </summary>
        /// <param name='word'>
        /// Word to add to the dictionary.
        /// </param>
        void Add(string word);

        /// <summary>
        /// Free instance.
        /// </summary>
        void Free();
    }
}