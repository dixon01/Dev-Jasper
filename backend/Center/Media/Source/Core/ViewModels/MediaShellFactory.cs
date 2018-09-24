// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MediaShellFactory.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the MediaShellFactory type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Media.Core.ViewModels
{
    using System.ComponentModel.Composition;

    using Gorba.Center.Common.Wpf.Framework.ViewModels;
    using Gorba.Center.Common.Wpf.Views;
    using Gorba.Center.Media.Core.Views;

    /// <summary>
    /// Factory to create <see cref="MediaShellWindow"/> windows.
    /// </summary>
    [Export]
    public class MediaShellFactory : WindowFactory<IShellWindowView>
    {
        /// <summary>
        /// Creates the specified view model.
        /// </summary>
        /// <param name="viewModel">The view model.</param>
        /// <returns>A new MediaShellWindow with the given data context.</returns>
        public override Common.Wpf.Framework.Views.IWindowView Create(IWindowViewModel viewModel)
        {
            var window = new MediaShellWindow { DataContext = viewModel };
            return window;
        }
    }
}
