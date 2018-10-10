// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IWindowViewModel.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the IWindowViewModel type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Common.Wpf.Framework.ViewModels
{
    using System;
    using System.ComponentModel;

    /// <summary>
    /// Defines the a ViewModel for windows.
    /// </summary>
    public interface IWindowViewModel : IDisplayViewModel, INotifyPropertyChanged
    {
        /// <summary>
        /// Occurs when the window is created.
        /// </summary>
        event EventHandler Created;

        /// <summary>
        /// Occurs when the window is closed.
        /// </summary>
        event EventHandler Closed;

        /// <summary>
        /// Occurs when the window is closing.
        /// </summary>
        event CancelEventHandler Closing;

        /// <summary>
        /// Closes the window.
        /// </summary>
        void Close();

        /// <summary>
        /// Shows the window and returns without waiting for the newly opened window to close.
        /// </summary>
        void Show();
    }
}