// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DialogControllerBase.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the DialogControllerBase type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Common.Wpf.Framework.Controllers
{
    using Gorba.Center.Common.Wpf.Framework.ViewModels;

    /// <summary>
    /// Provides a base implementation of the <see cref="IDialogController"/>.
    /// </summary>
    public abstract class DialogControllerBase : IDialogController
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DialogControllerBase"/> class.
        /// </summary>
        /// <param name="dialog">The window.</param>
        protected DialogControllerBase(IDialogViewModel dialog)
        {
            this.Dialog = dialog;
        }

        /// <summary>
        /// Gets or sets the window view model.
        /// </summary>
        public virtual IDialogViewModel Dialog { get; set; }

        /// <summary>
        /// Shows the dialog and returns only when the newly opened dialog is closed.
        /// </summary>
        /// <returns>The result of the dialog.</returns>
        public abstract DialogResultBase Run();
    }
}
