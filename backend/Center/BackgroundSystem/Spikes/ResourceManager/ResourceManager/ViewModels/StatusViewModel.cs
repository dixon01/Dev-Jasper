// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StatusViewModel.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the StatusViewModel type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.BackgroundSystem.Spikes.ResourceManager.ViewModels
{
    /// <summary>
    /// The status view model.
    /// </summary>
    public class StatusViewModel : ViewModelBase
    {
        private string application;

        private string currentOperation;

        private double? progress;

        /// <summary>
        /// Gets or sets the application status.
        /// </summary>
        public string Application
        {
            get
            {
                return this.application;
            }

            set
            {
                if (this.application == value)
                {
                    return;
                }

                this.application = value;
                this.OnPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets the current operation.
        /// </summary>
        public string CurrentOperation
        {
            get
            {
                return this.currentOperation;
            }

            set
            {
                if (this.currentOperation == value)
                {
                    return;
                }

                this.currentOperation = value;
                this.OnPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets the progress of the current operation.
        /// </summary>
        public double? Progress
        {
            get
            {
                return this.progress;
            }

            set
            {
                if (this.progress == value)
                {
                    return;
                }

                this.progress = value;
                this.OnPropertyChanged();
            }
        }
    }
}