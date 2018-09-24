// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IDialogFactory.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the IDialogFactory type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Common.Wpf.Framework.ViewModels
{
    using Gorba.Center.Common.Wpf.Framework.Controllers;
    using Gorba.Center.Common.Wpf.Framework.Views;

    /// <summary>
    /// Defines a factory for dialogs.
    /// </summary>
    public interface IDialogFactory
    {
        /// <summary>
        /// Creates the specified view model.
        /// </summary>
        /// <param name="viewModel">The view model.</param>
        /// <returns>The newly created dialog.</returns>
        IDialogView Create(IDialogViewModel viewModel);
    }
}
