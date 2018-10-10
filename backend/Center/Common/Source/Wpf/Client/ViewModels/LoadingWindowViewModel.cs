// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LoadingWindowViewModel.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Common.Wpf.Client.ViewModels
{
    using System.Windows.Media;

    using Gorba.Center.Common.Wpf.Framework.ViewModels;

    /// <summary>
    /// The loading window view model.
    /// </summary>
    public class LoadingWindowViewModel : WindowViewModelCloseStrategyBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LoadingWindowViewModel"/> class.
        /// </summary>
        /// <param name="factory">
        /// The factory.
        /// </param>
        /// <param name="applicationIcon">
        /// The application icon.
        /// </param>
        /// <param name="busyContent">
        /// The busy content.
        /// </param>
        public LoadingWindowViewModel(IWindowFactory factory, ImageSource applicationIcon, string busyContent)
            : base(factory)
        {
            this.ApplicationIcon = applicationIcon;
            this.BusyContent = busyContent;
        }

        /// <summary>
        /// Gets or sets the busy content.
        /// </summary>
        public string BusyContent { get; set; }

        /// <summary>
        /// Gets the application icon that is displayed in the taskbar.
        /// </summary>
        public ImageSource ApplicationIcon { get; private set; }
    }
}
