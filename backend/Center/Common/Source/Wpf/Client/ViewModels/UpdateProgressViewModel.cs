// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UpdateProgressViewModel.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the UpdateProgressViewModel type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Common.Wpf.Client.ViewModels
{
    using System.Windows.Input;
    using System.Windows.Media;

    using Gorba.Center.Common.Wpf.Framework.Controllers;
    using Gorba.Center.Common.Wpf.Framework.ViewModels;

    /// <summary>
    /// The update progress view model.
    /// </summary>
    public class UpdateProgressViewModel : DialogViewModelBase, IStartupDialogViewModel
    {
        private readonly ICommandRegistry commandRegistry;

        private ActivityMessage activityMessage;

        private int progressValue;

        private string updateSource;

        private string updateSourceUri;

        /// <summary>
        /// Initializes a new instance of the <see cref="UpdateProgressViewModel"/> class.
        /// </summary>
        /// <param name="factory">
        /// The factory.
        /// </param>
        /// <param name="commandRegistry">
        /// The command registry.
        /// </param>
        public UpdateProgressViewModel(IDialogFactory factory, ICommandRegistry commandRegistry)
            : base(factory)
        {
            this.commandRegistry = commandRegistry;
        }

        /// <summary>
        /// Gets or sets the short update source name.
        /// </summary>
        public string UpdateSource
        {
            get
            {
                return this.updateSource;
            }

            set
            {
                this.SetProperty(ref this.updateSource, value, () => this.UpdateSource);
            }
        }

        /// <summary>
        /// Gets or sets the entire update source URI.
        /// </summary>
        public string UpdateSourceUri
        {
            get
            {
                return this.updateSourceUri;
            }

            set
            {
                this.SetProperty(ref this.updateSourceUri, value, () => this.UpdateSourceUri);
            }
        }

        /// <summary>
        /// Gets or sets the progress value between 0 and 100.
        /// </summary>
        public int ProgressValue
        {
            get
            {
                return this.progressValue;
            }

            set
            {
                this.SetProperty(ref this.progressValue, value, () => this.ProgressValue);
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
        /// Gets or sets the window title.
        /// </summary>
        public string WindowTitle { get; set; }

        /// <summary>
        /// Gets or sets the application title that is displayed in the header bar section.
        /// </summary>
        public string ApplicationTitle { get; set; }

        /// <summary>
        /// Gets or sets the application icon that is displayed in the header bar section.
        /// </summary>
        public ImageSource ApplicationIcon { get; set; }

        /// <summary>
        /// Gets or sets the version of this application.
        /// </summary>
        public string ApplicationVersion { get; set; }

        /// <summary>
        /// Gets the cancel command.
        /// </summary>
        public ICommand CancelCommand
        {
            get
            {
                return this.commandRegistry.GetCommand(ClientCommandCompositionKeys.CancelUpdate);
            }
        }
    }
}
