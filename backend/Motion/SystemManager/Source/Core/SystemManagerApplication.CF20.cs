// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SystemManagerApplication.CF20.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the SystemManagerApplication type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.SystemManager.Core
{
    /// <summary>
    /// The system manager main application.
    /// This is a wrapper around <see cref="SystemManagementController"/> that sets up
    /// all the necessary stuff needed by System Manager.
    /// </summary>
    public partial class SystemManagerApplication
    {
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
            return true;
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <filterpriority>2</filterpriority>
        public void Dispose()
        {
        }
    }
}
