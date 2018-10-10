// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AdminBootstrapper.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Bootstrapper for icenter.admin.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Admin.Core
{
    using System;
    using System.ComponentModel.Composition;
    using System.ComponentModel.Composition.Hosting;
    using System.Diagnostics.CodeAnalysis;
    using System.Reflection;
    using System.Runtime.Serialization;
    using System.Windows.Threading;

    using Gorba.Center.Common.Wpf.Client;
    using Gorba.Center.Common.Wpf.Core;
    using Gorba.Center.Common.Wpf.Framework.Startup;

    using Microsoft.Practices.ServiceLocation;
    using Microsoft.Practices.Unity;

    using NLog;

    /// <summary>
    /// Bootstrapper for icenter.admin.
    /// </summary>
    [Export]
    [DataContract]
    [SuppressMessage(
        "Microsoft.StyleCop.CSharp.ReadabilityRules",
        "SA1100:DoNotPrefixCallsWithBaseUnlessLocalImplementationExists",
        Justification = "It seems a bug in the current version of StyleCop.")]
    public class AdminBootstrapper : ClientApplicationBootstrapperBase
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Initializes a new instance of the <see cref="AdminBootstrapper"/> class.
        /// </summary>
        /// <param name="compositionBatch">The composition batch.</param>
        /// <param name="assemblies">The assemblies.</param>
        public AdminBootstrapper(CompositionBatch compositionBatch, params Assembly[] assemblies)
            : base(compositionBatch, assemblies)
        {
        }

        /// <summary>
        /// Checks if the operating system meets the requirements of the application.
        /// </summary>
        /// <returns>
        /// <c>true</c> if the application meets the requirements; <c>false</c> otherwise.
        /// </returns>
        public override bool CheckOsRequirements()
        {
            try
            {
                var operatingSystem = this.GetOsVersionInfo();
                Logger.Info("Operating system: {0}", operatingSystem);
            }
            catch (Exception exception)
            {
                Logger.Warn(exception, "Can't retrieve the information about the Operating System");
            }

            var dotNetVersion = this.GetClrVersion();
            Logger.Info("Current CLR: {0}", dotNetVersion);
            if (dotNetVersion.Major >= 4)
            {
                return true;
            }

            Logger.Error(".NET framework 4.5 or higher is required.");
            return false;
        }

        /// <summary>
        /// Initializes the service locator.
        /// </summary>
        /// <typeparam name="TController">The type of the controller.</typeparam>
        /// <typeparam name="TState">The type of the state.</typeparam>
        /// <param name="result">The result.</param>
        protected override void AddRegistrationsToServiceLocator<TController, TState>(
            BootstrapperResult<TController, TState> result)
        {
            base.AddRegistrationsToServiceLocator(result);
            var container = ServiceLocator.Current.GetInstance<IUnityContainer>();
            var dispatcher = new WpfDispatcher(Dispatcher.CurrentDispatcher);
            container.RegisterInstance<IDispatcher>(dispatcher);
            container.RegisterInstance(result.State);
        }
    }
}