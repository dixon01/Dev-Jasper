// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SynchronizableControllerBase.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the SynchronizableControllerBase type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Common.Wpf.Framework.Controllers
{
    using System;
    using System.Windows.Threading;

    /// <summary>
    /// Controller base class that provides a dispatcher to synchronize async events onto the UI thread.
    /// </summary>
    public abstract class SynchronizableControllerBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SynchronizableControllerBase"/> class.
        /// </summary>
        protected SynchronizableControllerBase()
        {
            this.Dispatcher = Dispatcher.CurrentDispatcher;
        }

        /// <summary>
        /// Gets the UI dispatcher that can be used to execute code on the UI thread.
        /// </summary>
        public Dispatcher Dispatcher { get; private set; }

        /// <summary>
        /// Starts a new action on the UI thread.
        /// </summary>
        /// <param name="action">
        /// The action to perform on the UI thread.
        /// </param>
        protected void StartNew(Action action)
        {
            this.Dispatcher.BeginInvoke(action, DispatcherPriority.DataBind);
        }
    }
}
