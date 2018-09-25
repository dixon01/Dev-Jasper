// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LoginFactory.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   The login factory.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Common.Wpf.Client.ViewModels
{
    using Gorba.Center.Common.Wpf.Client.Views;
    using Gorba.Center.Common.Wpf.Framework.ViewModels;

    /// <summary>
    /// The login factory.
    /// </summary>
    public class LoginFactory : DialogFactory<ILoginWindowView>
    {
        /// <summary>
        /// Creates a new dialog for the specified view model.
        /// </summary>
        /// <param name="viewModel">The view model.</param>
        /// <returns>A new login dialog.</returns>
        public override Common.Wpf.Framework.Views.IDialogView Create(IDialogViewModel viewModel)
        {
            var window = new LoginWindow { DataContext = viewModel };
            return window;
        }
    }
}
