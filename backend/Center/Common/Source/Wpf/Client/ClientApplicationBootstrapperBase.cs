// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ClientApplicationBootstrapperBase.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the ClientApplicationBootstrapperBase type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Common.Wpf.Client
{
    using System.ComponentModel.Composition.Hosting;
    using System.Diagnostics.CodeAnalysis;
    using System.Reflection;

    using Gorba.Center.Common.Wpf.Client.Controllers;
    using Gorba.Center.Common.Wpf.Framework.Startup;

    using Microsoft.Practices.ServiceLocation;
    using Microsoft.Practices.Unity;

    /// <summary>
    /// Base class for all bootstrapper classes of applications using this client library.
    /// </summary>
    [SuppressMessage(
        "Microsoft.StyleCop.CSharp.ReadabilityRules",
        "SA1100:DoNotPrefixCallsWithBaseUnlessLocalImplementationExists",
        Justification = "It seems a bug in the current version of StyleCop.")]
    public abstract class ClientApplicationBootstrapperBase : CompositionBootstrapper
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ClientApplicationBootstrapperBase"/> class.
        /// </summary>
        /// <param name="compositionBatch">
        /// The composition batch.
        /// </param>
        /// <param name="assemblies">
        /// The assemblies.
        /// </param>
        protected ClientApplicationBootstrapperBase(CompositionBatch compositionBatch, params Assembly[] assemblies)
            : base(compositionBatch, assemblies)
        {
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
            container.RegisterInstance((IConnectedApplicationState)result.State);
            container.RegisterInstance((IClientApplicationController)result.Controller);
        }
    }
}
