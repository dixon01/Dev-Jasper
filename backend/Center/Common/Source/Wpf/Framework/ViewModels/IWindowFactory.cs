// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IWindowFactory.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the IWindowFactory type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Common.Wpf.Framework.ViewModels
{
    using Gorba.Center.Common.Wpf.Framework.Views;

    /// <summary>
    /// Defines a factory to create windows.
    /// </summary>
    public interface IWindowFactory
    {
        /// <summary>
        /// Creates a window binding its context to the specified view model.
        /// </summary>
        /// <param name="viewModel">The view model.</param>
        /// <returns>A new window with the given ViewModel as DataContext.</returns>
        IWindowView Create(IWindowViewModel viewModel);
    }
}
