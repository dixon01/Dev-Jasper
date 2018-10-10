// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WizardViewModelBase.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Common.Wpf.Wizard.ViewModels
{
    using System;
    using System.Collections.Generic;

    using ActiproSoftware.Windows.Controls.Wizard;

    using Gorba.Center.Common.Wpf.Framework.ViewModels;

    /// <summary>
    /// Defines the base class for a wizard.
    /// </summary>
    public abstract class WizardViewModelBase : DialogViewModelBase
    {
        private bool opened;

        private string title;

        /// <summary>
        /// Initializes a new instance of the <see cref="WizardViewModelBase"/> class.
        /// </summary>
        /// <param name="factory">
        /// The dialog factory.
        /// </param>
        protected WizardViewModelBase(IDialogFactory factory)
            : base(factory)
        {
            this.Steps = new List<WizardPage>();
        }

        /// <summary>
        /// Gets or sets the steps of the wizard.
        /// </summary>
        public IList<WizardPage> Steps { get; set; }

        /// <summary>
        /// Gets or sets the current step of the wizard.
        /// </summary>
        public IWizardStep CurrentStep { get; set; }

        /// <summary>
        /// Gets or sets the title of the wizard.
        /// </summary>
        public string Title
        {
            get
            {
                return this.title;
            }

            set
            {
                this.SetProperty(ref this.title, value, () => this.Title);
            }
        }

        /// <summary>
        /// Shows the window and returns only when the newly opened window is closed.
        /// </summary>
        /// <param name="parent">
        /// The parent window.
        /// </param>
        /// <returns>
        /// The result of the dialog.
        /// </returns>
        /// <exception cref="InvalidOperationException">
        /// The window is already being shown or the window has not been created yet.
        /// </exception>
        public override bool? ShowDialog(DialogParentViewModelBase parent = null)
        {
            if (this.opened)
            {
                throw new InvalidOperationException("You must close the dialog before showing it again.");
            }

            if (this.Dialog == null)
            {
                throw new InvalidOperationException("You must first create the dialog before showing it.");
            }

            this.opened = true;
            var result = this.Dialog.ShowDialog();
            this.opened = false;
            return result;
        }
    }
}
