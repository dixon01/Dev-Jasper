// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CompositionExtensions.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the CompositionExtensions type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Common.Wpf.Framework.Extensions
{
    using System;
    using System.ComponentModel.Composition;
    using System.ComponentModel.Composition.Hosting;

    using Gorba.Center.Common.Wpf.Framework.Services;
    using Gorba.Center.Common.Wpf.Framework.Startup;

    using NLog;

    /// <summary>
    /// Defines extension methods for the composition in the <see cref="CompositionBootstrapper"/>.
    /// </summary>
    public static class CompositionExtensions
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Tries to export the value loaded from <see cref="ApplicationStateManager.Current"/>.
        /// </summary>
        /// <typeparam name="T">The type of the state.</typeparam>
        /// <typeparam name="TExport">The type for the export.</typeparam>
        /// <param name="compositionBatch">The composition batch.</param>
        /// <param name="application">The application.</param>
        /// <param name="name">The name.</param>
        /// <param name="knownTypes">The types of application specific option groups</param>
        /// <returns>
        /// <c>true</c> if the state was loaded from the <see cref="ApplicationStateManager.Current"/>; otherwise,
        /// <c>false</c>.
        /// </returns>
        public static bool TryExportApplicationStateManagerValue<T, TExport>(
            this CompositionBatch compositionBatch,
            string application,
            string name,
            Type[] knownTypes = null) where T : class, TExport
        {
            try
            {
                Logger.Info("Exported state named '{0}' for application '{1}'", name, application);
                var state = ApplicationStateManager.Current.Load<T>(application, name, knownTypes);
                compositionBatch.AddExportedValue<TExport>(state);
                Logger.Info("Exported state {0} named '{1}' for application '{2}'", state, name, application);
                return true;
            }
            catch (Exception exception)
            {
                Logger.Warn(exception, "Error while trying to load the state");
                return false;
            }
        }

        /// <summary>
        /// Tries to export the value loaded from <see cref="ApplicationStateManager.Current"/>.
        /// </summary>
        /// <typeparam name="T">The type of the state.</typeparam>
        /// <param name="compositionBatch">The composition batch.</param>
        /// <param name="application">The application.</param>
        /// <param name="name">The name.</param>
        /// <param name="knownTypes">The types of application specific option groups</param>
        /// <returns>
        /// <c>true</c> if the state was loaded from the <see cref="ApplicationStateManager.Current"/>; otherwise,
        /// <c>false</c>.
        /// </returns>
        public static bool TryExportApplicationStateManagerValue<T>(
            this CompositionBatch compositionBatch,
            string application,
            string name,
            Type[] knownTypes = null) where T : class
        {
            try
            {
                Logger.Info("Exported state named '{0}' for application '{1}'", name, application);
                var state = ApplicationStateManager.Current.Load<T>(application, name, knownTypes);
                compositionBatch.AddExportedValue(state);
                Logger.Info("Exported state {0} named '{1}' for application '{2}'", state, name, application);
                return true;
            }
            catch (Exception exception)
            {
                Logger.Warn(exception, "Error while trying to load the state");
                return false;
            }
        }
    }
}