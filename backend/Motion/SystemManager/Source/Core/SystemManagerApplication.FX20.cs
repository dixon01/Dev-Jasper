// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SystemManagerApplication.FX20.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the SystemManagerApplication type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.SystemManager.Core
{
    using System.Threading;

    using Gorba.Common.SystemManagement.Core;

    /// <summary>
    /// The system manager main application.
    /// This is a wrapper around <see cref="SystemManagementControllerBase"/> that sets up
    /// all the necessary stuff needed by System Manager.
    /// </summary>
    public partial class SystemManagerApplication
    {
        private Mutex singleInstanceMutex;

        /// <summary>
        /// Check if the application should start.
        /// </summary>
        /// <param name="systemManagerOptions">
        /// The options.
        /// </param>
        /// <returns>
        /// True if this is the first instance of this application, otherwise false.
        /// </returns>
        public bool ShouldStart(SystemManagerOptions systemManagerOptions)
        {
            if (systemManagerOptions.ForceStart)
            {
                return true;
            }

            bool firstInstance;
            this.singleInstanceMutex = new Mutex(false, "SystemManager", out firstInstance);
            if (!firstInstance)
            {
                Logger.Error("System Manager already running");
                return false;
            }

            return true;
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <filterpriority>2</filterpriority>
        public void Dispose()
        {
            if (this.singleInstanceMutex == null)
            {
                return;
            }

            this.singleInstanceMutex.Close();
        }
    }
}
