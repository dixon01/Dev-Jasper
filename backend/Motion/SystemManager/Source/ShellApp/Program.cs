// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Program.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the Program type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.SystemManager.ShellApp
{
    using System;
    using System.IO;
    using System.Windows.Forms;

    using Gorba.Common.SystemManagement.Host;
    using Gorba.Common.Utility.Compatibility;
    using Gorba.Motion.SystemManager.Core;

    using NLog;

    /// <summary>
    /// The main program.
    /// </summary>
    public static class Program
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        /// <param name="args">
        /// The arguments.
        /// </param>
        [STAThread]
        public static void Main(string[] args)
        {
            try
            {
                var entryAssemblyLocation = ApplicationHelper.GetEntryAssemblyLocation();
                if (entryAssemblyLocation != null)
                {
                    // ReSharper disable once AssignNullToNotNullAttribute
                    Environment.CurrentDirectory = Path.GetDirectoryName(entryAssemblyLocation);
                }
            }
            catch (Exception ex)
            {
                Logger.Info(ex, "Exception, The current directory of the SM shell cannot not change!");
            }

            LaunchHelper.PrepareLaunch<SystemManagerShellOptions>(args);

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            new ApplicationHost<SystemManagerShellApplication>(args).Run("SystemManager");
        }
    }
}
