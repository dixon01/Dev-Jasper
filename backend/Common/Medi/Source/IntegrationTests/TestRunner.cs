// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TestRunner.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the TestRunner type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Medi.IntegrationTests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;

    using NLog;

    /// <summary>
    /// Class that runs all <see cref="IIntegrationTest"/>s in this solution.
    /// </summary>
    public class TestRunner
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private readonly List<IIntegrationTest> tests;

        /// <summary>
        /// Initializes a new instance of the <see cref="TestRunner"/> class.
        /// Loads all tests
        /// </summary>
        /// <param name="typeNames">
        /// All (short) type names to be executed, if empty or null, all tests will be executed.
        /// </param>
        public TestRunner(params string[] typeNames)
        {
            var asm = typeof(IIntegrationTest).Assembly;
            this.tests =
                new List<IIntegrationTest>(
                    asm.GetTypes().Where(t => !t.IsAbstract && typeof(IIntegrationTest).IsAssignableFrom(t)).Select(
                        Activator.CreateInstance).Cast<IIntegrationTest>().Where(i => i.Enabled));

            if (typeNames != null && typeNames.Length > 0)
            {
                this.tests.RemoveAll(t => !typeNames.Contains(t.GetType().Name));
            }
        }

        /// <summary>
        /// Runs all integration tests.
        /// </summary>
        /// <returns>
        /// true if all tests were successful.
        /// </returns>
        public bool Run()
        {
            Logger.Info("Running {0} tests", this.tests.Count);

            var failed = new List<IIntegrationTest>();
            int success = 0;
            foreach (IIntegrationTest test in this.tests)
            {
                if (RunTest(test))
                {
                    success++;
                }
                else
                {
                    failed.Add(test);
                }
            }

            Logger.Info("{0} out of {1} tests successful", success, this.tests.Count);
            if (failed.Count > 0)
            {
                Logger.Warn("{0} tests failed:", failed.Count);
                foreach (var test in failed)
                {
                    Logger.Warn(" - {0}", test.GetType().Name);
                }
            }

            LogManager.Flush();
            return success == this.tests.Count;
        }

        private static bool RunTest(IIntegrationTest test)
        {
            bool success = false;
            var thread = new Thread(() => success = RunTestOnThread(test));
            thread.Start();
            if (!thread.Join(test.Timeout))
            {
                Logger.Error("Execution thread of {0} timed out, aborting it", test.GetType().Name);
                thread.Abort();
                return false;
            }

            return success;
        }

        private static bool RunTestOnThread(IIntegrationTest test)
        {
            var testName = test.GetType().Name;
            Logger.Info("Running {0}", testName);
            try
            {
                test.Setup();
            }
            catch (Exception ex)
            {
                Logger.ErrorException("Exception in setup of " + testName, ex);
                return false;
            }

            try
            {
                test.Run();
            }
            catch (Exception ex)
            {
                Logger.ErrorException("Exception running the test " + testName, ex);
                return false;
            }

            try
            {
                test.Teardown();
            }
            catch (Exception ex)
            {
                Logger.ErrorException("Exception in teardown of " + testName, ex);
                return false;
            }

            Logger.Info("Successfully ran {0}", testName);
            return true;
        }
    }
}