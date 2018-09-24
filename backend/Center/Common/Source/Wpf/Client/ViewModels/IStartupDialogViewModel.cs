// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IStartupDialogViewModel.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the IStartupDialogViewModel type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Common.Wpf.Client.ViewModels
{
    using System.Windows.Media;

    /// <summary>
    /// Interface implemented by all dialog view models for dialogs shown during start-up.
    /// They all have a similar look and feel and therefore also share some properties.
    /// </summary>
    internal interface IStartupDialogViewModel
    {
        /// <summary>
        /// Gets or sets the window title.
        /// </summary>
        string WindowTitle { get; set; }

        /// <summary>
        /// Gets or sets the application title that is displayed in the header bar section.
        /// </summary>
        string ApplicationTitle { get; set; }

        /// <summary>
        /// Gets or sets the application icon that is displayed in the header bar section.
        /// </summary>
        ImageSource ApplicationIcon { get; set; }

        /// <summary>
        /// Gets or sets the version of this application.
        /// </summary>
        string ApplicationVersion { get; set; }
    }
}