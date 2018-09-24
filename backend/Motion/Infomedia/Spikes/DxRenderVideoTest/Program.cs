// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Program.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace DxRenderVideoTest
{
    using System;

    using Gorba.Common.Medi.Core;
    using Gorba.Common.Medi.Core.Config;

    using Microsoft.Samples.DirectX.UtilityToolkit;

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
        /// <returns>
        /// The return code.
        /// </returns>
        [STAThread]
        public static int Main()
        {
            System.Windows.Forms.Application.EnableVisualStyles();

            var configurator = new FileConfigurator("medi.config");
            MessageDispatcher.Instance.Configure(configurator);

            using (var sampleFramework = new Framework())
            {
                var sample = new Renderer(sampleFramework);
                try
                {
                    // Initialize
                    sample.InitializeApplication();

                    // Initialize the sample framework and create the desired window and Direct3D 
                    // device for the application. Calling each of these functions is optional, but they
                    // allow you to set several options which control the behavior of the sampleFramework.
                    // Parse the command line, handle the default hotkeys, and show msgboxes
                    sampleFramework.Initialize(true, true, true);

                    sample.CreateWindow();

                    // Pass control to the sample framework for handling the message pump and 
                    // dispatching render calls. The sample framework will call your FrameMove 
                    // and FrameRender callback when there is idle time between handling window messages.
                    sampleFramework.MainLoop();
                }
                catch (Exception e)
                {
                    Logger.FatalException("Error in DirectX", e);

#if DEBUG
                    // In debug mode show this error (maybe - depending on settings)
                    sampleFramework.DisplayErrorMessage(e);
#endif

                    // Ignore any exceptions here, they would have been handled by other areas
                    return (sampleFramework.ExitCode == 0) ? 1 : sampleFramework.ExitCode; // Return an error code here
                }

                // Perform any application-level cleanup here. Direct3D device resources are released within the
                // appropriate callback functions and therefore don't require any cleanup code here.
                return sampleFramework.ExitCode;
            }
        }
    }
}
