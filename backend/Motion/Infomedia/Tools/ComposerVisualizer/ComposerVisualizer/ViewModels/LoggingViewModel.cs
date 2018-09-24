// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LoggingViewModel.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   The logging view model.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Infomedia.Tools.ComposerVisualizer.ViewModels
{
    using System.Collections.ObjectModel;

    using Gorba.Motion.Infomedia.Tools.ComposerVisualizer.WpfCore;

    /// <summary>
    /// The logging view model.
    /// </summary>
    public class LoggingViewModel : ViewModelBase
    {
        private LogLevel minimumLevel;

        /// <summary>
        /// Initializes a new instance of the <see cref="LoggingViewModel"/> class.
        /// </summary>
        public LoggingViewModel()
        {
            this.Messages = new ObservableCollection<string>();
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
        /// Gets the messages.
        /// </summary>
        public ObservableCollection<string> Messages { get; private set; }
    }
}
