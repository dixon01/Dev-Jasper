// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ComponentApplicationController.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the ComponentApplicationController type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.SystemManagement.Core.Applications
{
    using System;
    using System.Reflection;

    using Gorba.Common.Configuration.SystemManager.Application;
    using Gorba.Common.SystemManagement.Client;
    using Gorba.Common.SystemManagement.ServiceModel;

    /// <summary>
    /// Application controller that handles a component (object implementing
    /// <see cref="IApplication"/>).
    /// </summary>
    public partial class ComponentApplicationController : ApplicationControllerBase
    {
        private readonly ComponentConfig config;

        private IApplication component;

        private AppDomain appDomain;

        /// <summary>
        /// Initializes a new instance of the <see cref="ComponentApplicationController"/> class.
        /// </summary>
        /// <param name="config">
        /// The config.
        /// </param>
        /// <param name="manager">
        /// The manager that created this controller.
        /// </param>
        public ComponentApplicationController(ComponentConfig config, ApplicationManager manager)
            : base(config, manager)
        {
            this.config = config;
        }

        /// <summary>
        /// Gets the main file path (i.e. the <see cref="ComponentConfig.LibraryPath"/>).
        /// </summary>
        protected override string FilePath
        {
            get
            {
                return this.config.LibraryPath;
            }
        }

        /// <summary>
        /// Implementation of the launch of the application.
        /// </summary>
        protected override void DoLaunch()
        {
            if (this.component != null)
            {
                this.Logger.Warn("Can't launch twice");
                return;
            }

            if (!this.config.UseAppDomain)
            {
                this.component = this.CreateLocalInstance();
            }
            else
            {
                this.component = this.CreateInstanceInNewAppDomain();
            }

            this.State = ApplicationState.Launching;
            this.component.Configure(this.config.Name);
            this.component.Start();
        }

        /// <summary>
        /// Implementation of the exit of the application.
        /// </summary>
        /// <param name="reason">
        /// The reason.
        /// </param>
        /// <param name="explanation">
        /// The explanation.
        /// </param>
        protected override void DoExit(ApplicationReason reason, string explanation)
        {
            var app = this.component;
            this.component = null;
            if (app == null)
            {
                this.Logger.Warn("Can't exit, component wasn't launched before");
                return;
            }

            app.Stop();
            this.SetState(ApplicationState.Exited, ApplicationReason.Requested, string.Empty);

            var appDom = this.appDomain;
            this.appDomain = null;
            if (appDom != null)
            {
                AppDomain.Unload(appDom);
            }
        }

        private IApplication CreateLocalInstance()
        {
            var asm = Assembly.LoadFrom(this.config.LibraryPath);
            var type = asm.GetType(this.config.ClassName, true);
            return (IApplication)Activator.CreateInstance(type);
        }
    }
}