// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IScreen.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the IScreen type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Obc.Terminal.Control.Screens
{
    using Gorba.Motion.Obc.Terminal.Core;

    /// <summary>
    /// Interface to be implemented by all screen implementations.
    /// </summary>
    internal interface IScreen
    {
        /// <summary>
        /// Gets the original MainField which was created from the factory
        /// </summary>
        IMainField MainField { get; }

        /// <summary>
        /// Shows this screen.
        /// </summary>
        void Show();

        /// <summary>
        /// Hides this screen.
        /// </summary>
        void Hide();

        /// <summary>
        /// Shows a message box on this screen.
        /// </summary>
        /// <param name="msgBoxInfo">
        /// The message box information.
        /// </param>
        void ShowMessageBox(MessageBoxInfo msgBoxInfo);

        /// <summary>
        /// Hides the message box.
        /// </summary>
        void HideMessageBox();

        /// <summary>
        /// Shows a progress bar on this screen.
        /// </summary>
        /// <param name="progressInfo">
        /// The progress information.
        /// </param>
        void ShowProgressBar(IProgressBarInfo progressInfo);

        /// <summary>
        /// Hides the progress bar.
        /// </summary>
        void HideProgressBar();
    }
}