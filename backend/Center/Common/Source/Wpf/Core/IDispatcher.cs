// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IDispatcher.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the IDispatcher type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Common.Wpf.Core
{
    using System;

    /// <summary>
    /// Defines an interface to a reusable action dispatcher.
    /// </summary>
    public interface IDispatcher
    {
        /// <summary>
        /// Dispatches the specified action to the thread.
        /// </summary>
        /// <param name="actionToInvoke">The action to invoke.</param>
        void Dispatch(Action actionToInvoke);
    }
}
