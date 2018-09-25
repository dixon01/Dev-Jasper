// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IButtonBar.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the IButtonBar type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Obc.Terminal.Core
{
    using System;

    /// <summary>
    ///   The interface for the button bar
    /// </summary>
    public interface IButtonBar
    {
        /// <summary>
        /// The button click event.
        /// </summary>
        event EventHandler<CommandEventArgs> ButtonClick;
    }
}