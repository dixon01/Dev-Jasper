// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ShutdownCatcher.CF20.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the ShutdownCatcher type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.SystemManagement.Client
{
    using System.Windows.Forms;

    /// <summary>
    /// Class that catches the shutdown event from Windows (using a dedicated form).
    /// </summary>
    public partial class ShutdownCatcher
    {
        private bool running;

        /// <summary>
        /// Starts this object.
        /// </summary>
        /// <param name="context">
        /// The application context or null if none is available.
        /// </param>
        public void Start(ApplicationContext context)
        {
            if (this.running)
            {
                return;
            }

            Logger.Debug("Starting");
            this.running = true;

            Logger.Debug("Started");
        }

        /// <summary>
        /// Stops this object.
        /// </summary>
        public void Stop()
        {
            if (!this.running)
            {
                return;
            }

            Logger.Debug("Stopping");
            this.running = false;

            Logger.Debug("Stopped");
        }
    }
}