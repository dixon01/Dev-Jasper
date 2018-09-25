// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ProgressPromptBase.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Common.Wpf.Framework.Interaction
{
    using System;

    using Gorba.Center.Common.Wpf.Framework.Notifications;

    using NLog;

    /// <summary>
    /// Base class for dialog prompts with a busy indicator
    /// </summary>
    public abstract class ProgressPromptBase : PromptNotification, IBusy
    {
        /// <summary>
        /// The <see cref="Logger"/> used for logging.
        /// </summary>
        protected readonly Logger Logger;

        private double totalProgress;
        private double currentProgress;
        private string currentProgressText;

        private string busyContentTextFormat;

        private bool isBusy;

        private bool isBusyIndeterminate;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProgressPromptBase"/> class.
        /// </summary>
        protected ProgressPromptBase()
        {
            this.Logger = LogManager.GetCurrentClassLogger();
        }

        /// <summary>
        /// Gets or sets a value indicating whether the prompt is busy.
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
        /// Gets or sets a value indicating whether is busy indeterminate.
        /// </summary>
        public bool IsBusyIndeterminate
        {
            get
            {
                return this.isBusyIndeterminate;
            }

            set
            {
                this.SetProperty(ref this.isBusyIndeterminate, value, () => this.IsBusyIndeterminate);
            }
        }

        /// <summary>
        /// Gets or sets the total progress.
        /// </summary>
        public double TotalBusyProgress
        {
            get
            {
                return this.totalProgress;
            }

            set
            {
                this.SetProperty(ref this.totalProgress, value, () => this.TotalBusyProgress);
                this.RaisePropertyChanged(() => this.ProgressPercent);
                this.RaisePropertyChanged(() => this.BusyContentText);
            }
        }

        /// <summary>
        /// Gets or sets the current progress.
        /// </summary>
        public double CurrentBusyProgress
        {
            get
            {
                return this.currentProgress;
            }

            set
            {
                this.SetProperty(ref this.currentProgress, value, () => this.CurrentBusyProgress);
                this.RaisePropertyChanged(() => this.ProgressPercent);
                this.RaisePropertyChanged(() => this.BusyContentText);
            }
        }

        /// <summary>
        /// Gets or sets the current progress text (e.g. Background.jpg).
        /// </summary>
        public string CurrentBusyProgressText
        {
            get
            {
                return this.currentProgressText;
            }

            set
            {
                this.SetProperty(ref this.currentProgressText, value, () => this.CurrentBusyProgressText);
            }
        }

        /// <summary>
        /// Gets or sets the busy content text format. If the property <see cref="IsBusyIndeterminate"/> is set to true,
        /// it must have 2 placeholders {0} for current progress
        /// and {1} for the total progress. (e.g. Loading resource {0} of {1})
        /// </summary>
        public string BusyContentTextFormat
        {
            get
            {
                return this.busyContentTextFormat;
            }

            set
            {
                this.SetProperty(ref this.busyContentTextFormat, value, () => this.BusyContentTextFormat);
                this.RaisePropertyChanged(() => this.BusyContentText);
            }
        }

        /// <summary>
        /// Gets the busy content text.
        /// It fills the <see cref="BusyContentTextFormat"/> with the <see cref="CurrentBusyProgress"/>
        /// and <see cref="TotalBusyProgress"/> values if the <see cref="IsBusyIndeterminate"/> is set to true.
        /// </summary>
        public string BusyContentText
        {
            get
            {
                if (string.IsNullOrEmpty(this.BusyContentTextFormat))
                {
                    return string.Empty;
                }

                try
                {
                    if (!this.IsBusyIndeterminate)
                    {
                        return string.Format(
                            this.BusyContentTextFormat,
                            this.CurrentBusyProgress,
                            this.TotalBusyProgress);
                    }

                    return this.BusyContentTextFormat;
                }
                catch (FormatException ex)
                {
                    this.Logger.Debug(ex, "The BusyContentTextFormat is not valid.");
                }

                return string.Empty;
            }
        }

        /// <summary>
        /// Gets the current progress in %.
        /// </summary>
        public double ProgressPercent
        {
            get
            {
                if (Math.Abs(this.TotalBusyProgress) < double.Epsilon)
                {
                    return 0;
                }

                return this.CurrentBusyProgress / this.TotalBusyProgress * 100;
            }
        }

        /// <summary>
        /// Clears all properties to their default value.
        /// </summary>
        public virtual void ClearBusy()
        {
            this.IsBusyIndeterminate = false;
            this.IsBusy = false;
            this.CurrentBusyProgress = 0;
            this.TotalBusyProgress = 0;
            this.BusyContentTextFormat = string.Empty;
            this.CurrentBusyProgressText = string.Empty;
        }
    }
}
