using System;
using System.Linq;
using System.Collections.Generic;
using System.Windows.Forms;

namespace CompactFramework35Test
{
    using Gorba.Common.Utility.Compatibility;

    using NLog;

    static class Program
    {
        private static readonly Logger Logger = LogManager.GetLogger("Program");

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [MTAThread]
        static void Main()
        {
            var version = ApplicationHelper.GetApplicationFileVersion();
            Logger.Info("Starting test application {0}", version);

            Application.Run(new Form1());
        }
    }
}