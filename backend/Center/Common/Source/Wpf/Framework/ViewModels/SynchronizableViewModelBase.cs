// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SynchronizableViewModelBase.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the SynchronizableViewModelBase type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Common.Wpf.Framework.ViewModels
{
    using System;
    using System.Windows.Threading;

    using Gorba.Center.Common.Wpf.Core;

    /// <summary>
    /// View model base class that provides a dispatcher to synchronize async events onto the UI thread.
    /// </summary>
    public abstract class SynchronizableViewModelBase : ViewModelBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SynchronizableViewModelBase"/> class.
        /// </summary>
        protected SynchronizableViewModelBase()
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
            this.Dispatcher.Invoke(action, DispatcherPriority.DataBind);
        }
    }
}
