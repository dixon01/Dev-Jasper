// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IStageController.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the IStageController type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Common.Wpf.Framework.Controllers
{
    /// <summary>
    /// Defines a controller for stages.
    /// </summary>
    public interface IStageController
    {
        /// <summary>
        /// Gets a value indicating whether this controller is active.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this controller is active; otherwise, <c>false</c>.
        /// </value>
        bool IsActive { get; }

        /// <summary>
        /// Activates this controller.
        /// </summary>
        void Activate();

        /// <summary>
        /// Deactivates this controller.
        /// </summary>
        void Deactivate();
    }
}