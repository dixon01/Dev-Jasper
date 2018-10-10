// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LoggingViewModel.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the LoggingViewModel type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Diag.Core.ViewModels.Log
{
    using System.Collections.ObjectModel;
    using System.Windows.Input;

    using Gorba.Center.Common.Wpf.Core;

    /// <summary>
    /// View model that describes the list of log messages and the state of the remote logging.
    /// </summary>
    public class LoggingViewModel : ViewModelBase
    {
        private bool isAutoScroll;

        private bool isEnabled;

        private LogLevel minimumLevel;

        /// <summary>
        /// Initializes a new instance of the <see cref="LoggingViewModel"/> class.
        /// </summary>
        public LoggingViewModel()
        {
            this.IsAutoScroll = true;
            this.IsEnabled = false;
            this.MinimumLevel = LogLevel.Info;
            this.Messages = new ObservableCollection<LogEntryViewModel>();
            this.ClearAllCommand = new RelayCommand(this.Messages.Clear);
        }

        /// <summary>
        /// Gets or sets a value indicating whether the log list view
        /// should scroll automatically when new items are added.
        /// </summary>
        public bool IsAutoScroll
        {
            get
            {
                return this.isAutoScroll;
            }

            set
            {
                this.SetProperty(ref this.isAutoScroll, value, () => this.IsAutoScroll);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether receiving log messages is enabled.
        /// </summary>
        public bool IsEnabled
        {
            get
            {
                return this.isEnabled;
            }

            set
            {
                this.SetProperty(ref this.isEnabled, value, () => this.IsEnabled);
            }
        }

        /// <summary>
        /// Gets or sets the minimum level of messages to be logged.
        /// </summary>
        public LogLevel MinimumLevel
        {
            get
            {
                return this.minimumLevel;
            }

            set
            {
                this.SetProperty(ref this.minimumLevel, value, () => this.MinimumLevel);
            }
        }

        /// <summary>
        /// Gets the list of messages.
        /// </summary>
        public ObservableCollection<LogEntryViewModel> Messages { get; private set; }

        /// <summary>
        /// Gets the command to clear all <see cref="Messages"/>.
        /// </summary>
        public ICommand ClearAllCommand { get; private set; }
    }
}