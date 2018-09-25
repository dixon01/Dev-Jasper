// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Program.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the Program type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Utility.PerformanceLogger
{
    using System;
    using System.Diagnostics;
    using System.Threading;

    using Gorba.Common.Utility.PerformanceLogger.Component;

    /// <summary>
    /// Test logic for the program.
    /// </summary>
    internal class Program
    {
        /// <summary>
        /// The iterations.
        /// </summary>
        public const int Iterations = 1000;

        private static readonly Random Random = new Random();

        /// <summary>
        /// The main method.
        /// </summary>
        /// <param name="args">
        /// The args.
        /// </param>
        internal static void Main(string[] args)
        {
            int sessionId;
            if (args.Length < 1 || string.IsNullOrEmpty(args[0]) || !int.TryParse(args[0], out sessionId))
            {
                sessionId = 1;
            }

            Console.WriteLine("Starting performance logger test for session {0}...");
            for (int i = 1; i <= Iterations; i++)
            {
                Extensions.LogBegin(i);
                var sleep = Random.Next(500);
                Thread.Sleep(sleep);
                Extensions.LogEnd(i);
            }

            Thread.Sleep(100);
            var quit = false;
            while (!quit)
            {
                Console.WriteLine("Type q to quit");
                var read = Console.ReadLine();
                Debug.Assert(read != null, "The string read shoudn't be null.");
                quit = read.Equals("q");
            }
        }
    }
}