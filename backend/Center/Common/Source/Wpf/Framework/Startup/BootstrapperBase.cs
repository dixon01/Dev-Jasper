// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BootstrapperBase.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the BootstrapperBase type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Common.Wpf.Framework.Startup
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using System.Security;
    using System.Security.Permissions;
    using System.Threading.Tasks;

    using Gorba.Center.Common.Wpf.Framework.Controllers;

    using Microsoft.Practices.ServiceLocation;
    using Microsoft.Practices.Unity;

    /// <summary>
    /// Defines the component responsible to execute operations needed at startup.
    /// </summary>
    public abstract class BootstrapperBase
    {
        /// <summary>
        /// Startup procedure which returns a <see cref="BootstrapperResult&lt;TController, TState&gt;"/> object.
        /// </summary>
        /// <typeparam name="TController">The type of the controller.</typeparam>
        /// <typeparam name="TState">The type of the state.</typeparam>
        /// <returns>
        /// The result of the operation.
        /// </returns>
        public virtual BootstrapperResult<TController, TState> Bootstrap<TController, TState>()
            where TController : IApplicationController where TState : IApplicationState
        {
            var unityContainer = new UnityContainer();
            ServiceLocator.SetLocatorProvider(() => new UnityServiceLocator(unityContainer));
            var result = this.BootstrapCore<TController, TState>();
            this.InitializeServiceLocator(result);
            return result;
        }

        /// <summary>
        /// Checks if the <paramref name="path"/> has the needed <paramref name="permissions"/>.
        ///  </summary>
        /// <param name="path">
        /// The path to check.
        /// </param>
        /// <param name="permissions">
        /// The permissions needed.
        /// </param>
        /// <returns>
        /// List of failed permissions.
        /// </returns>
        public virtual IEnumerable<FileIOPermissionAccess> GetFailingPathPermissions(
            string path, params FileIOPermissionAccess[] permissions)
        {
            if (string.IsNullOrEmpty(path))
            {
                throw new ArgumentNullException("path");
            }

            if (permissions == null)
            {
                throw new ArgumentNullException("permissions");
            }

            if (permissions.Length == 0)
            {
                throw new ArgumentOutOfRangeException(
                    "permissions", "The list of permissions can't be empty. Please specify at least one permission.");
            }

            var result = new List<FileIOPermissionAccess>();

            foreach (var permission in permissions)
            {
                var fileIoPermission = new FileIOPermission(permission, path);
                var permissionSet = new PermissionSet(PermissionState.None);
                permissionSet.AddPermission(fileIoPermission);
                if (!permissionSet.IsSubsetOf(AppDomain.CurrentDomain.PermissionSet))
                {
                    result.Add(permission);
                }
            }

            return result;
        }

        /// <summary>
        /// Checks if the operating system meets the requirements of the application.
        /// </summary>
        /// <returns>
        /// <c>true</c> if the application meets the requirements; <c>false</c> otherwise.
        /// </returns>
        public abstract bool CheckOsRequirements();

        /// <summary>
        /// Checks if an assembly is compatible with the application.
        /// </summary>
        /// <param name="assembly">
        /// The assembly to check.
        /// </param>
        /// <returns>
        /// <c>true</c> if the assembly is compatible; <c>false</c> otherwise.
        /// </returns>
        public abstract bool CheckAssemblyCompatibility(Assembly assembly);

        /// <summary>
        /// Startup procedure which returns a <see cref="BootstrapperResult&lt;TController, TState&gt;"/> object.
        /// </summary>
        /// <typeparam name="TController">The type of the controller.</typeparam>
        /// <typeparam name="TState">The type of the state.</typeparam>
        /// <returns>
        /// The result of the operation.
        /// </returns>
        protected abstract BootstrapperResult<TController, TState> BootstrapCore<TController, TState>()
            where TController : IApplicationController
            where TState : IApplicationState;

        /// <summary>
        /// Initializes the service locator.
        /// </summary>
        /// <typeparam name="TController">The type of the controller.</typeparam>
        /// <typeparam name="TState">The type of the state.</typeparam>
        /// <param name="result">The result.</param>
        /// <remarks>
        /// The method MUST ensure that the <see cref="ServiceLocator"/> is initialized
        /// through its <see cref="ServiceLocator.SetLocatorProvider"/> method.
        /// The base implementation also registers the <see cref="TaskScheduler.Default"/> to the container.
        /// </remarks>
        protected virtual void AddRegistrationsToServiceLocator<TController, TState>(
            BootstrapperResult<TController, TState> result) where TController : IApplicationController
            where TState : IApplicationState
        {
            // Nothing to add here
        }

        /// <summary>
        /// Gets the operating system information
        /// </summary>
        /// <returns>
        /// The <see cref="OsInfo"/>.
        /// </returns>
        protected OsInfo GetOsVersionInfo()
        {
            var os = Environment.OSVersion;

            return new OsInfo
            {
                Version = os,
                Is64Bit = Environment.Is64BitOperatingSystem
            };
        }

        /// <summary>
        /// Gets the current runtime version.
        /// </summary>
        /// <returns>
        /// The current <see cref="Version"/>.
        /// </returns>
        protected Version GetClrVersion()
        {
            return Environment.Version;
        }

        private void InitializeServiceLocator<TController, TState>(BootstrapperResult<TController, TState> result)
            where TController : IApplicationController
            where TState : IApplicationState
        {
            var unityContainer = ServiceLocator.Current.GetInstance<IUnityContainer>();
            unityContainer.RegisterInstance(result.Controller);
            unityContainer.RegisterInstance<IApplicationController>(result.Controller);
            unityContainer.RegisterInstance(result.State);
            unityContainer.RegisterInstance<IApplicationState>(result.State);
            unityContainer.RegisterInstance(TaskScheduler.Default);
            Func<TaskScheduler> getCurrentSynchronizationContextTaskScheduler =
                TaskScheduler.FromCurrentSynchronizationContext;
            unityContainer.RegisterInstance(getCurrentSynchronizationContextTaskScheduler);
            this.AddRegistrationsToServiceLocator(result);
        }

        /// <summary>
        /// Defines version information of the operating system.
        /// </summary>
        protected class OsInfo
        {
            /// <summary>
            /// Gets or sets a value indicating whether the system is 64 bit.
            /// </summary>
            public bool Is64Bit { get; set; }

            /// <summary>
            /// Gets or sets the version including the service pack.
            /// </summary>
            public OperatingSystem Version { get; set; }

            /// <summary>
            /// Returns a <see cref="System.String"/> that represents this instance.
            /// </summary>
            /// <returns>
            /// A <see cref="System.String"/> that represents this instance.
            /// </returns>
            public override string ToString()
            {
                var bit = this.Is64Bit ? "64bit" : "32bit";
                return string.Format("{0} ({1})", this.Version.VersionString, bit);
            }
        }
    }
}
