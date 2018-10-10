// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IUpdateController.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the IUpdateController type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Update.Core
{
    using System;

    /// <summary>
    /// Interface for update application controllers.
    /// </summary>
    public interface IUpdateController : IDisposable
    {
        /// <summary>
        /// Starts the update controller
        /// </summary>
        /// <param name="application">
        /// The <see cref="UpdateApplication"/> starting this controller.
        /// </param>
        void Start(UpdateApplication application);

        /// <summary>
        /// Stops the update controller
        /// </summary>
        void Stop();
    }
}