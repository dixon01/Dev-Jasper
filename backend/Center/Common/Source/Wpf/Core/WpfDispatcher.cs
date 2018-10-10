// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WpfDispatcher.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   A Wrapper for the WPF Dispatcher type
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Common.Wpf.Core
{
    using System;
    using System.Windows.Threading;

    /// <summary>
    /// Implementation of the <see cref="IDispatcher"/> wrapping the WPF <see cref="Dispatcher"/>.
    /// </summary>
    public class WpfDispatcher : IDispatcher
    {
        private readonly Dispatcher dispatcher;

        /// <summary>
        /// Initializes a new instance of the <see cref="WpfDispatcher"/> class.
        /// </summary>
        public WpfDispatcher()
            : this(Dispatcher.CurrentDispatcher)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="WpfDispatcher"/> class.
        /// </summary>
        /// <param name="dispatcher">The dispatcher.</param>
        public WpfDispatcher(Dispatcher dispatcher)
        {
            this.dispatcher = dispatcher;
        }

        /// <summary>
        /// Dispatches the specified action to the thread.
        /// </summary>
        /// <param name="actionToInvoke">The action to invoke.</param>
        public void Dispatch(Action actionToInvoke)
        {
            if (!this.CanDispatch())
            {
                actionToInvoke();
            }
            else
            {
                this.dispatcher.Invoke(DispatcherPriority.Normal, actionToInvoke);
            }
        }

        /// <summary>
        /// Checks whether the thread invoking the method.
        /// </summary>
        /// <returns><c>true</c> if it is possible to dispatch directly; otherwise, <c>false</c>.</returns>
        public bool CanDispatch()
        {
            return !this.dispatcher.CheckAccess();
        }
    }
}