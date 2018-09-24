// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UpdateProgressFactory.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the UpdateProgressFactory type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Common.Wpf.Client.ViewModels
{
    using Gorba.Center.Common.Wpf.Client.Views;
    using Gorba.Center.Common.Wpf.Framework.ViewModels;
    using Gorba.Center.Common.Wpf.Framework.Views;

    /// <summary>
    /// The factory for <see cref="UpdateProgressWindow"/>.
    /// </summary>
    public class UpdateProgressFactory : DialogFactory<IUpdateProgressWindowView>
    {
        /// <summary>
        /// Creates a new dialog for the specified view model.
        /// </summary>
        /// <param name="viewModel">The view model.</param>
        /// <returns>A new login dialog.</returns>
        public override IDialogView Create(IDialogViewModel viewModel)
        {
            var window = new UpdateProgressWindow { DataContext = viewModel };
            return window;
        }
    }
}
