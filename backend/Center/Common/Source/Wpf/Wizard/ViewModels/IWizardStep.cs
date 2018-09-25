// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IWizardStep.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Common.Wpf.Wizard.ViewModels
{
    using System.ComponentModel;

    using ActiproSoftware.Windows.Controls.Wizard;

    /// <summary>
    /// Defines a wizard step.
    /// </summary>
    public interface IWizardStep : INotifyPropertyChanged
    {
        /// <summary>
        /// Gets or sets the page model.
        /// </summary>
        WizardPage PageModel { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the Back button is enabled.
        /// </summary>
        bool IsBackEnabled { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the Next button is enabled.
        /// </summary>
        bool IsNextEnabled { get; set; }

        /// <summary>
        /// Initializes the wizard step by mapping the ViewModel to the page.
        /// </summary>
        void Initialize();
    }
}
