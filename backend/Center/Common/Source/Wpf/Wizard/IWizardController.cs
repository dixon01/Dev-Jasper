// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IWizardController.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Common.Wpf.Wizard
{
    using System.Collections.Generic;

    using Gorba.Center.Common.Wpf.Framework;
    using Gorba.Center.Common.Wpf.Wizard.ViewModels;

    /// <summary>
    /// Defines a controller for a wizard.
    /// </summary>
    public interface IWizardController
    {
        /// <summary>
        /// Initializes the <see cref="ActiproSoftware.Windows.Controls.Wizard"/> control.
        /// It adds all wizard steps to the control.
        /// </summary>
        /// <param name="control">
        /// The <see cref="ActiproSoftware.Windows.Controls.Wizard"/> control.
        /// </param>
        /// <param name="steps">
        /// The wizard steps.
        /// </param>
        /// <param name="title">The title of the wizard.</param>
        void Initialize(
            ActiproSoftware.Windows.Controls.Wizard.Wizard control, IEnumerable<IWizardStep> steps, string title);

        /// <summary>
        /// Shows the dialog and returns only when the newly opened dialog is closed.
        /// </summary>
        /// <returns>
        /// The result of the dialog.
        /// </returns>
        DialogResultBase Run();
    }
}
