//------------------------------------------------------------------------------
// <copyright 
//  file="MockSpeller.cs" 
//  company="enckse">
//  Copyright (c) All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
namespace CompactNHunspell.Test
{
    using System.Collections.Generic;
    using NUnit.Framework;

    /// <summary>
    /// Mock speller
    /// </summary>
    public class MockSpeller : ISpeller
    {
        /// <summary>
        /// Testing definition for aff file
        /// </summary>
        public const string AffText = "aff-test";

        /// <summary>
        /// Testing definition for dictionary
        /// </summary>
        public const string DictText = "dict-test";

        /// <summary>
        /// Known words
        /// </summary>
        private HashSet<string> words = new HashSet<string>()
        {
            "word"
        };

        /// <inheritdoc />
        public void Init(string affFile, string dictFile)
        {
            Assert.AreEqual(AffText, System.IO.File.ReadAllText(affFile));
            Assert.AreEqual(DictText, System.IO.File.ReadAllText(dictFile));
        }

        /// <inheritdoc />
        public bool Spell(string word)
        {
            return this.words.Contains(word);
        }

        /// <inheritdoc />
        public void Add(string word)
        {
            if (!this.words.Contains(word))
            {
                this.words.Add(word);
            }
        }

        /// <inheritdoc />
        public void Free()
        {
        }
    }
}