// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IUpdateComponent.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the IUpdateComponent type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Update.ServiceModel.Common
{
    using System;

    /// <summary>
    /// Base interface for <see cref="IUpdateSink"/> and <see cref="IUpdateSource"/>.
    /// </summary>
    public interface IUpdateComponent
    {
        /// <summary>
        /// Event that is fired when <see cref="IsAvailable"/> changes.
        /// </summary>
        event EventHandler IsAvailableChanged;

        /// <summary>
        /// Gets a value indicating whether the repository is available
        /// (e.g. the USB stick is plugged in or the FTP server is reachable).
        /// </summary>
        bool IsAvailable { get; }

        /// <summary>
        /// Gets the unique name of this part.
        /// </summary>
        string Name { get; }
    }
}