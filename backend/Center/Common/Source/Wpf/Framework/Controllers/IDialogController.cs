// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IDialogController.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the IDialogController type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Common.Wpf.Framework.Controllers
{
    using Gorba.Center.Common.Wpf.Framework.ViewModels;

    /// <summary>
    /// Defines a controller for dialogs.
    /// </summary>
    public interface IDialogController
    {
        /// <summary>
        /// Gets or sets the associated window view.
        /// </summary>
        IDialogViewModel Dialog { get; set; }

        /// <summary>
        /// Shows the dialog and returns the result when the newly created dialog is closed.
        /// </summary>
        /// <returns>The result of the dialog.</returns>
        DialogResultBase Run();
    }
}
