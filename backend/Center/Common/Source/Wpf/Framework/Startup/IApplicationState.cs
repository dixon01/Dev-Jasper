// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IApplicationState.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   The IApplicationState.cs
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Common.Wpf.Framework.Startup
{
    using Gorba.Center.Common.Wpf.Framework.Model;

    /// <summary>
    /// Defines the state of the application.
    /// </summary>
    public interface IApplicationState : IDirty
    {
        /// <summary>
        /// Gets or sets the application options.
        /// </summary>
        ApplicationOptions Options { get; set; }

        /// <summary>
        /// Gets the display context.
        /// </summary>
        DisplayContext DisplayContext { get; }
    }
}