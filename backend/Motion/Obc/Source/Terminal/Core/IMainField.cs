// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IMainField.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the IMainField type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Obc.Terminal.Core
{
    using System;

    /// <summary>
    /// Base interface for all main fields.
    /// </summary>
    public interface IMainField
    {
        /// <summary>
        ///   The event which will be called when user pressed ESC
        /// </summary>
        event EventHandler EscapePressed;

        /// <summary>
        ///   The event which will be called when user pressed ENTER
        /// </summary>
        event EventHandler ReturnPressed;

        /// <summary>
        /// Make sure that calling code is running in the GUI thread!!!
        ///   Check with GetControl().InvokeRequired
        /// </summary>
        /// <param name="visible">
        /// A flag indicating if this field should be made visible.
        /// </param>
        void MakeVisible(bool visible);

        /// <summary>
        ///   Shows a message box to the user. The message box overlay all other elements!
        ///   You can use this to show for example an error during login (e.g. wrong driver number)
        ///   The message box has to be confirmed.
        ///   If already a Progress bar should be shown, the Progress bar will be hidden
        /// </summary>
        /// <param name = "msgBoxInfo">
        /// The message box information.
        /// </param>
        void ShowMessageBox(MessageBoxInfo msgBoxInfo);

        /// <summary>
        ///   Hides the message box
        /// </summary>
        void HideMessageBox();

        /// <summary>
        ///   Shows a progress bar to the user. Use this if you need time to validate user data
        ///   Type progress bar overlay all other elements. It can't be interrupted
        ///   You may implement the IProgressElapsedCallBack interface.
        ///   If already a Message box should be shown, the Message box will be hidden
        /// </summary>
        /// <param name = "progressInfo">
        /// The progress information.
        /// </param>
        void ShowProgressBar(IProgressBarInfo progressInfo);

        /// <summary>
        ///   Hides the progress bar
        /// </summary>
        void HideProgressBar();

        /// <summary>
        ///   Will be true if a ProgressBar or Message box is active!
        ///   In this case the current main field shouldn't change
        /// </summary>
        /// <returns>
        /// True if a ProgressBar or Message box is active.
        /// </returns>
        bool IsLocked();
    }
}