// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IBusy.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Common.Wpf.Framework
{
    /// <summary>
    /// The Busy interface.
    /// </summary>
    public interface IBusy
    {
        /// <summary>
        /// Gets or sets a value indicating whether is busy.
        /// </summary>
        bool IsBusy { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether is busy indeterminate.
        /// </summary>
        bool IsBusyIndeterminate { get; set; }

        /// <summary>
        /// Gets or sets the busy content text format.
        /// </summary>
        string BusyContentTextFormat { get; set; }

        /// <summary>
        /// Gets the busy content text.
        /// </summary>
        string BusyContentText { get; }

        /// <summary>
        /// Gets or sets the current busy progress text.
        /// </summary>
        string CurrentBusyProgressText { get; set; }

        /// <summary>
        /// Gets or sets the current busy progress.
        /// </summary>
        double CurrentBusyProgress { get; set; }

        /// <summary>
        /// Gets or sets the total busy progress.
        /// </summary>
        double TotalBusyProgress { get; set; }

        /// <summary>
        /// Clears all properties to their default value.
        /// </summary>
        void ClearBusy();
    }
}
