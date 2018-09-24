// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Program.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the Program type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Tfs.Tools.QualityScreenProvider
{
    using System;
    using System.Threading;

    using Gorba.Motion.Protran.Core;

    /// <summary>
    /// The main program.
    /// </summary>
    public class Program
    {
        private static readonly AutoResetEvent StopWait = new AutoResetEvent(false);

        private static ProtranCtrl protranCtrl;

        /// <summary>
        /// The main application's method.
        /// This starts everything.
        /// </summary>
        /// <param name="args">List of all the required arguments
        /// coming from the command line.</param>
        public static void Main(string[] args)
        {
            protranCtrl = new ProtranCtrl();
            protranCtrl.Stopped += ProtranCtrlOnStopped;

            protranCtrl.Initialize();
            protranCtrl.Start();

            var consoleReader = new Thread(ReadConsole) { IsBackground = true };
            consoleReader.Start();

            StopWait.WaitOne();

            DisposeAll();
        }

        private static void ProtranCtrlOnStopped(object sender, EventArgs eventArgs)
        {
            StopWait.Set();
        }

        private static void ReadConsole()
        {
            while (Console.ReadLine() != "q")
            {
            }

            DisposeAll();
        }

        private static void DisposeAll()
        {
            protranCtrl.Stop();
            protranCtrl.Dispose();
        }
    }
}
