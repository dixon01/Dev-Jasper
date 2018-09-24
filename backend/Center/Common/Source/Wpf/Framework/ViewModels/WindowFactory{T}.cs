// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WindowFactory{T}.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the WindowFactory type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Common.Wpf.Framework.ViewModels
{
    using System;

    using Gorba.Center.Common.Wpf.Framework.Views;

    /// <summary>
    /// Base window factory.
    /// </summary>
    /// <typeparam name="T">The type of the window.</typeparam>
    public abstract class WindowFactory<T> : IWindowFactory
        where T : IWindowView
    {
        /// <summary>
        /// Creates a window binding its context to the specified view model.
        /// </summary>
        /// <param name="viewModel">The view model.</param>
        /// <returns>
        /// A new window with the given ViewModel as DataContext.
        /// </returns>
        public virtual IWindowView Create(IWindowViewModel viewModel)
        {
            try
            {
                var window = Activator.CreateInstance<T>();
                window.DataContext = viewModel;
                return window;
            }
            catch (Exception exception)
            {
                throw new ApplicationException(
                    string.Format(
                        "Error while creating a new window of type {0}."
                        + " This default implementation only works if a default constructor is defined.",
                        typeof(T).FullName),
                    exception);
            }
        }
    }
}
