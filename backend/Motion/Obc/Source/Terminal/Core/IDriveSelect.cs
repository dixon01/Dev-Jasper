// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IDriveSelect.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the IDriveSelect type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Obc.Terminal.Core
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// The drive selection field interface.
    /// </summary>
    public interface IDriveSelect : IMainField
    {
        /// <summary>
        /// The drive confirmed event.
        /// </summary>
        event EventHandler<DriveSelectedEventArgs> DriveConfirmed;

        /// <summary>
        /// Initialization of this field.
        /// </summary>
        /// <param name = "caption">
        /// The caption.
        /// </param>
        /// <param name = "items">
        /// The drives to select from.
        /// </param>
        /// <param name = "isDrivingSchoolActive">
        /// if true the driving school button will be selected
        /// </param>
        /// <param name = "isAdditionalActive">
        /// if true the additional drive button will be selected
        /// </param>
        /// <param name="focusIndex">
        /// Index of the focused drive.
        /// </param>
        void Init(
            string caption,
            List<string> items,
            bool isDrivingSchoolActive,
            bool isAdditionalActive,
            int focusIndex);
    }
}