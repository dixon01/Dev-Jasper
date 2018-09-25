// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IApplicationController.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the IApplicationController type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Common.Wpf.Framework.Controllers
{
    using System;

    /// <summary>
    /// Defines the controller for the application lifecycle.
    /// This controller is the first to be started and the last to be shutdown.
    /// It &quot;delegates&quot; the control to other controllers (for instance, window controllers), orchestrating.
    /// </summary>
    public interface IApplicationController
    {
        /// <summary>
        /// Occurs when the application shutdown is requested by the controller.
        /// </summary>
        event EventHandler ShutdownCompleted;

        /// <summary>
        /// Initializes this controller.
        /// </summary>
        void Initialize();

        /// <summary>
        /// Runs the controller logic until completed or until the <see cref="Shutdown"/>.
        /// </summary>
        void Run();

        /// <summary>
        /// Requests the shutdown of this controller.
        /// </summary>
        void Shutdown();
    }
}
