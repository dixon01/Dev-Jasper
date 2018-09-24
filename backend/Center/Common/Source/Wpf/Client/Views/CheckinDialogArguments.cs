// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CheckinDialogArguments.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   The check in dialog arguments.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Common.Wpf.Client.Views
{
    using System;

    using Gorba.Center.Common.Wpf.Client.Controllers;

    /// <summary>
    /// The check in dialog arguments.
    /// </summary>
    public class CheckinDialogArguments
    {
        /// <summary>
        /// Gets or sets a value indicating whether skip is allowed.
        /// </summary>
        public bool Skippable { get; set; }

        /// <summary>
        /// Gets or sets the on check in completed.
        /// </summary>
        public Action<CheckinTrapResult> OnCheckinCompleted { get; set; }
    }
}