// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CompositionBootstrapper.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the CompositionBootstrapper type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Common.Wpf.Framework.Startup
{
    using System;
    using System.ComponentModel.Composition.Hosting;
    using System.ComponentModel.Composition.Primitives;
    using System.Linq;
    using System.Reflection;

    using Gorba.Center.Common.Wpf.Framework.Controllers;

    using Microsoft.Practices.ServiceLocation;
    using Microsoft.Practices.Unity;

    using NLog;

    /// <summary>
    /// Defines a <see cref="BootstrapperBase"/> that uses composition to build the
    /// <see cref="IApplicationController"/>.
    /// </summary>
    public class CompositionBootstrapper : BootstrapperBase
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private readonly Assembly[] assemblies;

        private CompositionContainer compositionContainer;

        private ComposablePartCatalog catalog;

        /// <summary>
        /// Initializes a new instance of the <see cref="CompositionBootstrapper"/> class.
        /// </summary>
        /// <param name="compositionBatch">The composition batch.</param>
        /// <param name="assemblies">The assemblies.</param>
        public CompositionBootstrapper(CompositionBatch compositionBatch, params Assembly[] assemblies)
        {
            if (assemblies == null)
            {
                throw new ArgumentNullException("assemblies");
            }

            if (assemblies.Length == 0)
            {
                throw new ArgumentOutOfRangeException(
                    "assemblies", "The list of assemblies can't be empty. Please specify at least one assembly.");
            }

            this.CompositionBatch = compositionBatch ?? new CompositionBatch();
            this.assemblies = assemblies;
        }

        /// <summary>
        /// Gets the composition batch.
        /// </summary>
        protected CompositionBatch CompositionBatch { get; private set; }

        /// <summary>
        /// Checks if the operating system meets the requirements of the application.
        /// </summary>
        /// <returns>
        /// <c>true</c> if the application meets the requirements; <c>false</c> otherwise.
        /// </returns>
        public override bool CheckOsRequirements()
        {
            return true;
        }

        /// <summary>
        /// Checks if an assembly is compatible with the application.
        /// </summary>
        /// <param name="assembly">
        /// The assembly to check.
        /// </param>
        /// <returns>
        /// <c>true</c> if the assembly is compatible; <c>false</c> otherwise.
        /// </returns>
        public override bool CheckAssemblyCompatibility(Assembly assembly)
        {
            return true;
        }

        /// <summary>
        /// Startup procedure which returns a <see cref="BootstrapperResult&lt;TController, TState&gt;"/> object.
        /// It composes the <see cref="IApplicationController"/> using the specified assemblies as
        /// <see cref="AssemblyCatalog"/>s.
        /// If not <c>null</c>, the given <see cref="CompositionBatch"/> is passed to the
        /// <see cref="CompositionContainer"/>.
        /// </summary>
        /// <typeparam name="TController">The type of the controller.</typeparam>
        /// <typeparam name="TState">The type of the state.</typeparam>
        /// <returns>
        /// The result of the operation.
        /// </returns>
        protected override BootstrapperResult<TController, TState> BootstrapCore<TController, TState>()
        {
            try
            {
                var assemblyCatalogs = this.assemblies.Select(assembly => new AssemblyCatalog(assembly));
                this.catalog = new AggregateCatalog(assemblyCatalogs);
                this.compositionContainer = new CompositionContainer(this.catalog);

                var bootstrapperResult = new BootstrapperResult<TController, TState>();
                this.compositionContainer.Compose(this.CompositionBatch);
                this.OnComposed(this.compositionContainer);

                bootstrapperResult.Controller = this.compositionContainer.GetExportedValue<TController>();
                bootstrapperResult.State = this.compositionContainer.GetExportedValue<TState>();
                var dictionaries = this.compositionContainer.GetExportedValues<IResourceDictionary>().ToList();
                dictionaries.ForEach(dictionary => bootstrapperResult.ResourceDictionaries.Add(dictionary));

                return bootstrapperResult;
            }
            catch (ReflectionTypeLoadException reflectionException)
            {
                foreach (var loaderException in reflectionException.LoaderExceptions)
                {
                    Logger.Error(loaderException, "Error loading a type.");
                }

                throw new ApplicationException(
                    "Error during composition. See the previous logged LoaderExceptions for details.",
                    reflectionException);
            }
            catch (Exception exception)
            {
                throw new ApplicationException("Error during composition", exception);
            }
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
            var unityContainer = ServiceLocator.Current.GetInstance<IUnityContainer>();
            unityContainer.RegisterInstance(this.compositionContainer);
            unityContainer.RegisterInstance(this.catalog);
        }

        /// <summary>
        /// Invoked after composition.
        /// </summary>
        /// <param name="compositionContainer">
        /// The composition container.
        /// </param>
        protected virtual void OnComposed(CompositionContainer compositionContainer)
        {
        }
    }
}