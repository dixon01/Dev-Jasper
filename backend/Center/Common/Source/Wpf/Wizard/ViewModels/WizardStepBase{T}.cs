// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WizardStepBase{T}.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Common.Wpf.Wizard.ViewModels
{
    using ActiproSoftware.Windows.Controls.Wizard;

    using Gorba.Center.Common.Wpf.Core;
    using Gorba.Center.Common.Wpf.Framework.ViewModels;

    /// <summary>
    /// Defines the base wizard step ViewModel.
    /// </summary>
    /// <typeparam name="T">
    /// The page type.
    /// </typeparam>
    public abstract class WizardStepBase<T> : ViewModelBase, IWizardStep
        where T : WizardPage, new()
    {
        private bool isBackEnabled;
        private bool isNextEnabled;

        private string caption;

        private string description;

        private bool isBusy;

        private ActivityMessage activityMessage;

        /// <summary>
        /// Initializes a new instance of the <see cref="WizardStepBase&lt;T&gt;"/> class.
        /// </summary>
        protected WizardStepBase()
        {
            this.activityMessage = new ActivityMessage();
        }

        /// <summary>
        /// Gets or sets the page model.
        /// </summary>
        public T PageModel { get; set; }

        WizardPage IWizardStep.PageModel
        {
            get
            {
                return this.PageModel;
            }

            set
            {
                this.PageModel = value as T;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the Back button is enabled.
        /// </summary>
        public bool IsBackEnabled
        {
            get
            {
                return this.isBackEnabled;
            }

            set
            {
                this.SetProperty(ref this.isBackEnabled, value, () => this.IsBackEnabled);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the Next button is enabled.
        /// </summary>
        public bool IsNextEnabled
        {
            get
            {
                return this.isNextEnabled;
            }

            set
            {
                this.SetProperty(ref this.isNextEnabled, value, () => this.IsNextEnabled);
            }
        }

        /// <summary>
        /// Gets or sets the caption of the wizard step.
        /// </summary>
        public string Caption
        {
            get
            {
                return this.caption;
            }

            set
            {
                this.SetProperty(ref this.caption, value, () => this.Caption);
            }
        }

        /// <summary>
        /// Gets or sets the description of the wizard step.
        /// </summary>
        public string Description
        {
            get
            {
                return this.description;
            }

            set
            {
                this.SetProperty(ref this.description, value, () => this.Description);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the controller is doing something.
        /// </summary>
        public bool IsBusy
        {
            get
            {
                return this.isBusy;
            }

            set
            {
                this.SetProperty(ref this.isBusy, value, () => this.IsBusy);
            }
        }

        /// <summary>
        /// Gets or sets the activity message.
        /// </summary>
        /// <value>
        /// The activity message.
        /// </value>
        public ActivityMessage ActivityMessage
        {
            get
            {
                return this.activityMessage;
            }

            set
            {
                this.SetProperty(ref this.activityMessage, value, () => this.ActivityMessage);
            }
        }

        /// <summary>
        /// Maps this ViewModel to the page.
        /// </summary>
        public virtual void Initialize()
        {
            this.PageModel = new T
            {
                DataContext = this
            };
        }

        /// <summary>
        /// Sets the activity message values.
        /// </summary>
        /// <param name="message">
        ///     The message.
        /// </param>
        /// <param name="type">
        ///     The type.
        /// </param>
        /// <param name="isHidden">
        ///     Flag set if the activity message should not be visible.
        /// </param>
        public virtual void SetActivityMessage(
            string message,
            ActivityMessage.ActivityMessageType type = ActivityMessage.ActivityMessageType.Info,
            bool isHidden = true)
        {
            this.ActivityMessage.IsHidden = isHidden;
            this.ActivityMessage.Type = type;
            this.ActivityMessage.Message = message;
        }
    }
}