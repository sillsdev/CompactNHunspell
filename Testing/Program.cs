//------------------------------------------------------------------------------
// <copyright 
//  file="Program.cs" 
//  company="enckse">
//  Copyright (c) All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
namespace CompactNHunspell.Tests
{
    using System;
    using CompactNHunspell;

    /// <summary>
    /// The entry-point class for this application.
    /// </summary>
    public static class Program
    {
        /// <summary>
        /// The entry-point class for this application.
        /// </summary>
        /// <param name='args'>Command-line arguments</param>
        public static void Main(string[] args)
        {
            bool error = false;
            if (args == null || args.Length != 2)
            {
                Console.WriteLine("Arguments for testing not sent. The arguments are full paths to the aff and dict files (in that order)");
                error = true;   
            }
            else
            {
                try
                {
                    // Load on construction
                    using (NHunspellWrapper wrap = new NHunspellWrapper(args[0], args[1]))
                    {
                        Test(wrap);
                    }

                    // Load after construction
                    using (NHunspellWrapper wrap = new NHunspellWrapper())
                    {
                        wrap.Load(args[0], args[1]);
                        Test(wrap);
                    }

                    // Never load
                    using (NHunspellWrapper wrap = new NHunspellWrapper())
                    {
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
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                    error = true;
                }
            }

            if (error)
            {
                System.Environment.Exit(1);
            }
        }

        /// <summary>
        /// Run the wrapper through some tests
        /// </summary>
        /// <param name='wrap'>Wrapper to test</param>
        private static void Test(NHunspellWrapper wrap)
        {
            if (wrap.Spell("notaword"))
            {
                throw new Exception("notaword should not be considered spelled correctly");
            }

            // It was cached, no change
            wrap.Add("notaword");
            if (wrap.Spell("notaword"))
            {
                throw new Exception("notaword should not be considered spelled correctly");
            }

            if (!wrap.Spell("word"))
            {
                throw new Exception("word should be considered spelled correctly");
            }

            // Set as valid and test after
            wrap.Add("notaword2");
            if (!wrap.Spell("notaword2"))
            {
                throw new Exception("notaword2 should be considered spelled correctly");
            }

            if (wrap.IsDisposed)
            {
                throw new Exception("Wrapper shouldn't be disposed yet");
            }

            string test = string.Empty;
            wrap.LogAction = (x, y) => { test = "done"; };
            wrap.LogAction.Invoke(typeof(Program), "done");
            if (test != "done")
            {
                throw new Exception("Unable to change logger, value was " + test);
            }

            test = null;
            wrap.LogAction = null;
            wrap.LogAction.Invoke(typeof(Program), "done");
            if (test != null)
            {
                throw new Exception("Value should not have been set");
            }
        }
    }
}
