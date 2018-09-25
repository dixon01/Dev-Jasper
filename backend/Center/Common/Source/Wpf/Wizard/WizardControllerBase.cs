// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WizardControllerBase.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Common.Wpf.Wizard
{
    using System;
    using System.Collections.Generic;
    using System.Windows;

    using ActiproSoftware.Windows.Controls.Wizard;

    using Gorba.Center.Common.Wpf.Framework.Controllers;
    using Gorba.Center.Common.Wpf.Wizard.ViewModels;

    /// <summary>
    /// Provides a base implementation of the <see cref="IWizardController"/>.
    /// </summary>
    public abstract class WizardControllerBase : DialogControllerBase, IWizardController
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="WizardControllerBase"/> class.
        /// </summary>
        /// <param name="wizard">
        /// The wizard.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// Thrown if the parameter wizard is null.
        /// </exception>
        protected WizardControllerBase(WizardViewModelBase wizard)
            : base(wizard)
        {
            if (wizard == null)
            {
                throw new ArgumentNullException("wizard");
            }
        }

        /// <summary>
        /// Gets the wizard ViewModel.
        /// </summary>
        public virtual WizardViewModelBase Wizard
        {
            get
            {
                return this.Dialog as WizardViewModelBase;
            }
        }

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
        public virtual void Initialize(Wizard control, IEnumerable<IWizardStep> steps, string title)
        {
            control.Items.Clear();
            this.Wizard.Steps.Clear();
            foreach (var wizardStep in steps)
            {
                wizardStep.Initialize();
                this.Wizard.Steps.Add(wizardStep.PageModel);
                control.Items.Add(wizardStep.PageModel);
            }

            control.DataContext = this.Wizard;
            control.SelectedPageChanging += this.ControlOnSelectedPageChanged;
            control.Finish += this.ControlOnFinish;
            this.Wizard.Title = title;
            this.Wizard.CurrentStep = this.Wizard.Steps[0].DataContext as IWizardStep;
        }

        /// <summary>
        /// Handles the Finish event.
        /// </summary>
        protected abstract void HandleFinish();

        /// <summary>
        /// Handles the values of the ViewModel before the next step is shown.
        /// </summary>
        /// <param name="eventArgs">
        /// The <see cref="WizardSelectedPageChangeEventArgs"/>.
        /// </param>
        protected abstract void ProcessBeforeNextStep(
            WizardSelectedPageChangeEventArgs eventArgs);

        private void ControlOnSelectedPageChanged(
            object sender, WizardSelectedPageChangeEventArgs eventArgs)
        {
            this.ProcessBeforeNextStep(eventArgs);
            this.Wizard.CurrentStep = eventArgs.NewSelectedPage.DataContext as IWizardStep;
        }

        private void ControlOnFinish(object sender, RoutedEventArgs e)
        {
            this.HandleFinish();
        }
    }
}
