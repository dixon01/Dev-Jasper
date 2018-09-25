// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UpdateApplication.cs" company="Luminator LTG">
//   Copyright © 2011-2017 LuminatorLTG. All rights reserved.
// </copyright>
// <summary>
//   Defines the UpdateApplication type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Update.Core
{
    using System;
    using System.Threading;

    using CommandLineParser;

    using Gorba.Common.SystemManagement.Host;
    using Gorba.Motion.Update.Core.SelfUpdate;

    /// <summary>
    /// The update application.
    /// </summary>
    public class UpdateApplication : ApplicationBase
    {
        private readonly ManualResetEvent runWait = new ManualResetEvent(false);

        private IUpdateController updateController;

        /// <summary>
        /// Implementation of the <see cref="ApplicationBase.Run"/> method.
        /// This method should not return until after <see cref="ApplicationBase.Stop"/> was called.
        /// Implementing classes should either override <see cref="ApplicationBase.DoRun()"/> or this method.
        /// </summary>
        /// <param name="args">
        /// The command line arguments.
        /// </param>
        protected override void DoRun(string[] args)
        {
            this.updateController = CreateUpdateController(args);
            this.updateController.Start(this);

            this.SetRunning();

            this.runWait.WaitOne();
        }

        /// <summary>
        /// Implementation of the <see cref="ApplicationBase.Stop"/> method.
        /// This method should stop whatever is running in <see cref="ApplicationBase.DoRun()"/>.
        /// </summary>
        protected override void DoStop()
        {
            try
            {
                this.updateController.Stop();
                this.updateController = null;
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Couldn't stop the update controller");
            }

            this.runWait.Set();
        }

        private static IUpdateController CreateUpdateController(string[] args)
        {
            if (args.Length > 0)
            {
                var parser = new CommandLineParser();
                var options = new CommandLineOptions();
                parser.ExtractArgumentAttributes(options);
                parser.ParseCommandLine(args);

                if (!string.IsNullOrEmpty(options.InstallFile) && !string.IsNullOrEmpty(options.TargetPath))
                {
                    return new SelfUpdateController(options.InstallFile, options.TargetPath, options.WaitForExit);
                }
            }

            return new UpdateController();
        }
    }
}
