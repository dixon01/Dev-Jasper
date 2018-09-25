// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TenantSelectionFactory.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   The tenant selection factory.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Common.Wpf.Client.ViewModels
{
    using Gorba.Center.Common.Wpf.Client.Views;
    using Gorba.Center.Common.Wpf.Framework.ViewModels;

    /// <summary>
    /// The tenant selection factory.
    /// </summary>
    public class TenantSelectionFactory : DialogFactory<ITenantSelectionWindowView>
    {
        /// <summary>
        /// Creates a new dialog for the specified view model.
        /// </summary>
        /// <param name="viewModel">The view model.</param>
        /// <returns>A new login dialog.</returns>
        public override Common.Wpf.Framework.Views.IDialogView Create(IDialogViewModel viewModel)
        {
            var window = new TenantSelectionWindow { DataContext = viewModel };
            return window;
        }
    }
}
