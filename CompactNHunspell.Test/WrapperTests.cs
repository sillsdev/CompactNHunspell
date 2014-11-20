//------------------------------------------------------------------------------
// <copyright 
//  file="WrapperTests.cs" 
//  company="enckse">
//  Copyright (c) All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
namespace CompactNHunspell.Test
{
    using System;
    using NUnit.Framework;

    /// <summary>
    /// Wrapper testing
    /// </summary>
    [TestFixture]
    public class WrapperTests
    {
        /// <summary>
        /// Affix file for testing
        /// </summary>
        private string affixFile = string.Empty;

        /// <summary>
        /// Dictionary file
        /// </summary>
        private string dictFile = string.Empty;
        
        /// <summary>
        /// Executing test id
        /// </summary>
        private Guid id = Guid.Empty;

        /// <summary>
        /// Construction-based loading
        /// </summary>
        [Test]
        public void Construction()
        {
            // Load on construction
            using (NHunspellWrapper wrap = new NHunspellWrapper(this.affixFile, this.dictFile, typeof(MockSpeller).AssemblyQualifiedName))
            {
                Test(wrap);
            }
        }

        /// <summary>
        /// Loading files after creating the instance
        /// </summary>
        [Test]
        public void LoadPost()
        {
            // Load after construction
            using (NHunspellWrapper wrap = new NHunspellWrapper())
            {
                wrap.OverrideType = typeof(MockSpeller).AssemblyQualifiedName;
                wrap.Load(this.affixFile, this.dictFile);
                Test(wrap);
            }
        }

        /// <summary>
        /// Testing when not initialized
        /// </summary>
        [Test]
        public void NotInitialized()
        {
            // Never load
            using (NHunspellWrapper wrap = new NHunspellWrapper())
            {
                wrap.OverrideType = typeof(MockSpeller).AssemblyQualifiedName;
                try
                {
                    Test(wrap);
                }
                catch (InvalidOperationException inv)
                {
                    if (!inv.Message.Contains("Speller not initialized"))
                    {
                        throw;
                    }
                }
            }
        }

        /// <summary>
        /// Invalid type override
        /// </summary>
        [Test]
        public void InvalidType()
        {
            // Invalid type
            using (NHunspellWrapper wrap = new NHunspellWrapper())
            {
                wrap.OverrideType = "failure.type";
                try
                {
                    wrap.Load(this.affixFile, this.dictFile);
                    Test(wrap);
                }
                catch (ArgumentException argExp)
                {
                    if (!argExp.Message.Contains("Invalid Hunspell instance type"))
                    {
                        throw;
                    }
                }
            }
        }

        /// <summary>
        /// Trace function testing
        /// </summary>
        [Test]
        public void TraceFunction()
        {
            var list = new System.Collections.Generic.List<string>();
            using (NHunspellWrapper wrap = new NHunspellWrapper())
            {
                wrap.LogAction = (x, y) => 
                { 
                    if (x == typeof(MockSpeller)) 
                    { 
                        list.Add(y); 
                    } 
                };

                wrap.OverrideType = typeof(MockSpeller).AssemblyQualifiedName;
                wrap.Load(this.affixFile, this.dictFile);
                wrap.Spell("test");
                wrap.Add("test");
            }

            Assert.AreEqual(4, list.Count, string.Join(Environment.NewLine, list.ToArray()));
            Assert.IsTrue(list.Contains("Init"));
            Assert.IsTrue(list.Contains("Spell"));
            Assert.IsTrue(list.Contains("Add"));
            Assert.IsTrue(list.Contains("Free"));
        }

        /// <summary>
        /// Trace function attached during construction
        /// </summary>
        [Test]
        public void TraceFunctionAttached()
        {
            var list = new System.Collections.Generic.List<string>();
            Log log = (x, y) => 
            { 
                if (x == typeof(MockSpeller)) 
                { 
                    list.Add(y); 
                } 
            };

            using (NHunspellWrapper wrap = new NHunspellWrapper(this.affixFile, this.dictFile, typeof(MockSpeller).AssemblyQualifiedName, log))
            {
                wrap.Spell("test");
                wrap.Add("test");
            }

            Assert.AreEqual(4, list.Count, string.Join(Environment.NewLine, list.ToArray()));
            Assert.IsTrue(list.Contains("Init"));
            Assert.IsTrue(list.Contains("Spell"));
            Assert.IsTrue(list.Contains("Add"));
            Assert.IsTrue(list.Contains("Free"));
        }

        /// <summary>
        /// Testing setup
        /// </summary>
        [TestFixtureSetUp] 
        public void Init()
        {
            this.id = Guid.NewGuid();
            this.affixFile = this.id.ToString() + ".aff";
            this.dictFile = this.id.ToString() + ".dic";
            System.IO.File.WriteAllText(this.affixFile, MockSpeller.AffText);
            System.IO.File.WriteAllText(this.dictFile, MockSpeller.DictText);
        }

        /// <summary>
        /// Run the wrapper through some tests
        /// </summary>
        /// <param name='wrap'>Wrapper to test</param>
        private static void Test(NHunspellWrapper wrap)
        {
            if (wrap.Spell("notaword"))
            {
                Assert.Fail("notaword should not be considered spelled correctly");
            }

            // It was cached, no change
            wrap.Add("notaword");
            if (wrap.Spell("notaword"))
            {
                Assert.Fail("notaword should not be considered spelled correctly");
            }

            // Clearing the cache
            wrap.Clear();
            if (!wrap.Spell("notaword"))
            {
                Assert.Fail("notaword should be considered spelled correctly");
            }

            if (!wrap.Spell("word"))
            {
                Assert.Fail("word should be considered spelled correctly");
            }

            // Set as valid and test after
            wrap.Add("notaword2");
            if (!wrap.Spell("notaword2"))
            {
                Assert.Fail("notaword2 should be considered spelled correctly");
            }

            if (wrap.IsDisposed)
            {
                Assert.Fail("Wrapper shouldn't be disposed yet");
            }

            string test = string.Empty;
            wrap.LogAction = (x, y) => { test = "done"; };
            wrap.LogAction.Invoke(typeof(WrapperTests), "done");
            if (test != "done")
            {
                Assert.Fail("Unable to change logger, value was " + test);
            }

            test = null;
            wrap.LogAction = null;
            wrap.LogAction.Invoke(typeof(WrapperTests), "done");
            if (test != null)
            {
                Assert.Fail("Value should not have been set");
            }

            if (wrap.Spell("flushCacheTest"))
            {
                Assert.Fail("flushCacheTest should not be considered spelled correctly");
            }

            if (wrap.DisableCache)
            {
                Assert.Fail("Cache should not be disabled");
            }

            // Limit the cache size, check the word is still not valid
            wrap.CacheSize = 1;
            wrap.Add("flushCacheTest");
            if (!wrap.Spell("flushCacheTest"))
            {
                Assert.Fail("flushCacheTest should be considered spelled correctly");
            }

            if (!wrap.DisableCache)
            {
                Assert.Fail("Cache should be disabled");
            }

            wrap.DisableCache = false;
            if (wrap.DisableCache)
            {
                Assert.Fail("Cache should not be disabled");
            }

            wrap.DisableCache = true;
            if (!wrap.DisableCache)
            {
                Assert.Fail("Cache should be disabled");
            }
        }
    }
}
