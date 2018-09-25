// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Program.cs" company="Gorba AG">
//   Copyright © 2011-2012 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Protran.AbuDhabiMerge
{
    using System;
    using System.Windows.Forms;
    using System.Windows.Forms.VisualStyles;

    using NLog;

    /// <summary>
    /// Main class for this application
    /// </summary>
    public static class Program
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        public static void Main()
        {
            AppDomain.CurrentDomain.UnhandledException += (s, e) =>
            {
                var ex = e.ExceptionObject as Exception;
                if (ex != null)
                {
                    Logger.Fatal("Unhandled Exception; terminating={0}, Cause {1}", e.IsTerminating, ex);
                }

                LogManager.Flush();
            };

            if (VisualStyleRenderer.IsSupported)
            {
                Application.EnableVisualStyles();
            }

            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new AbuDhabiMergeMainForm());
        }
    }
}
