// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IStatusField.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the IStatusField type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Obc.Terminal.Core
{
    using System;

    /// <summary>
    /// The status field on the top right side
    /// </summary>
    public interface IStatusField
    {
        /// <summary>
        ///   The event when the status field is touched from the user
        /// </summary>
        event EventHandler Click;

        /// <summary>
        ///   Adds a text to be shown.
        /// </summary>
        /// <param name = "message">
        /// The message.
        /// </param>
        void SetMessage(string message);
    }
}