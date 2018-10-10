// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Program.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Infomedia.SharpDXRendererTest
{
    using System;
    using System.Windows.Forms;

    using Gorba.Common.Medi.Core;
    using Gorba.Common.Medi.Core.Config;

    using NLog;

    /// <summary>
    /// Main Program.
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
            AppDomain.CurrentDomain.UnhandledException += CurrentDomainOnUnhandledException;
            var configurator = new FileConfigurator("medi.config");
            MessageDispatcher.Instance.Configure(configurator);

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            var renderer = new Renderer();
            renderer.Run();
        }

        private static void CurrentDomainOnUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Logger.FatalException("Unhandled exception", (Exception)e.ExceptionObject);
            LogManager.Flush();
        }
    }
}
