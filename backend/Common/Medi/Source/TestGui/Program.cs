// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Program.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the Program type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Medi.TestGui
{
    using System;
    using System.Windows.Forms;

    using NLog;

    /// <summary>
    /// Main program.
    /// </summary>
    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        /// <param name="args">
        /// The command line arguments.
        /// </param>
        [STAThread]
        internal static void Main(string[] args)
        {
            AppDomain.CurrentDomain.UnhandledException +=
                (s, e) =>
                LogManager.GetCurrentClassLogger().FatalException(
                    "Unhandled exception, terminating=" + e.IsTerminating, (Exception)e.ExceptionObject);

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            var mainForm = new MainForm();
            if (args.Length == 1)
            {
                mainForm.LoadConfig(args[0]);
            }

            Application.Run(mainForm);
        }
    }
}
