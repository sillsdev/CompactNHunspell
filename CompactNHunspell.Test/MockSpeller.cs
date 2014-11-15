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
        public Log TraceFunction { get; set; }

        /// <inheritdoc />
        public void Init(string affFile, string dictFile)
        {
            this.Log("Init");
            Assert.AreEqual(AffText, System.IO.File.ReadAllText(affFile));
            Assert.AreEqual(DictText, System.IO.File.ReadAllText(dictFile));
        }

        /// <inheritdoc />
        public bool Spell(string word)
        {
            this.Log("Spell");
            return this.words.Contains(word);
        }

        /// <inheritdoc />
        public void Add(string word)
        {
            this.Log("Add");
            if (!this.words.Contains(word))
            {
                this.words.Add(word);
            }
        }

        /// <inheritdoc />
        public void Free()
        {
            this.Log("Free");
        }

        /// <summary>
        /// Log a message
        /// </summary>
        /// <param name="message">Message to log</param>
        private void Log(string message)
        {
            if (this.TraceFunction != null)
            {
                this.TraceFunction(typeof(MockSpeller), message);
            }
        }
    }
}