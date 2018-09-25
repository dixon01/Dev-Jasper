// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MainField.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the MainField type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Obc.Terminal.C74.Controls
{
    using System;
    using System.Drawing;
    using System.Windows.Forms;

    using Gorba.Motion.Obc.Terminal.Core;

    /// <summary>
    /// The main field base class for C74 GUI.
    /// </summary>
    public partial class MainField : UserControl, IMainField
    {
        private static readonly Color ColorErrorMsg = Color.FromArgb(204, 2, 47);

        private static readonly Color ColorInfoMsg = Color.FromArgb(13, 32, 134);

        private static readonly Color ColorWarningMsg = Color.FromArgb(235, 184, 29);

        private MessageBoxInfo currentMessageBox;
        private IProgressBarInfo currentProgressBar;
        private IC74Input hiddenSelected;

        /// <summary>
        /// Initializes a new instance of the <see cref="MainField"/> class.
        /// </summary>
        public MainField()
        {
            this.InitializeComponent();
        }

        /// <summary>
        ///   The event which will be called when user pressed ESC
        /// </summary>
        public event EventHandler EscapePressed;

        /// <summary>
        ///   The event which will be called when user pressed ENTER
        /// </summary>
        public event EventHandler ReturnPressed;

        /// <summary>
        /// Event that is fired when the menu button was pressed.
        /// </summary>
        public event EventHandler MenuButtonPressed;

        /// <summary>
        /// The safe begin invoke.
        /// </summary>
        /// <param name="action">
        /// The action.
        /// </param>
        public void SafeBeginInvoke(MethodInvoker action)
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke(action);
            }
            else
            {
                action();
            }
        }

        /// <summary>
        /// Make sure that calling code is running in the GUI thread!!!
        ///   Check with GetControl().InvokeRequired
        /// </summary>
        /// <param name="visible">
        /// A flag indicating if this field should be made visible.
        /// </param>
        public virtual void MakeVisible(bool visible)
        {
            this.Visible = visible;

            if (!visible)
            {
                this.HideMessageBox();
                this.HideProgressBar();
            }
        }

        /// <summary>
        ///   Shows a message box to the user. The message box overlay all other elements!
        ///   You can use this to show for example an error during login (e.g. wrong driver number)
        ///   The message box has to be confirmed.
        ///   If already a Progress bar should be shown, the Progress bar will be hidden
        /// </summary>
        /// <param name = "msgBoxInfo">
        /// The message box information.
        /// </param>
        public void ShowMessageBox(MessageBoxInfo msgBoxInfo)
        {
            this.SafeBeginInvoke(
                () =>
                {
                    this.HideMessageBox();
                    this.HideSelection();
                    this.currentMessageBox = msgBoxInfo;
                    this.messageBox.Caption = msgBoxInfo.Caption;
                    this.messageBox.Message = msgBoxInfo.Message;
                    switch (msgBoxInfo.Type)
                    {
                        case MessageBoxInfo.MsgType.Warning:
                            this.messageBox.TitleColor = ColorWarningMsg;
                            break;
                        case MessageBoxInfo.MsgType.Error:
                            this.messageBox.TitleColor = ColorErrorMsg;
                            break;
                        default:
                            this.messageBox.TitleColor = ColorInfoMsg;
                            break;
                    }

                    this.messageBox.Visible = true;
                    this.messageBox.BringToFront();
                });
        }

        /// <summary>
        ///   Hides the message box
        /// </summary>
        public void HideMessageBox()
        {
            if (this.currentMessageBox == null)
            {
                return;
            }

            var msgBox = this.currentMessageBox;
            this.currentMessageBox = null;
            this.SafeBeginInvoke(
                () =>
                {
                    this.messageBox.Visible = false;
                    this.ShowSelection();
                    msgBox.OnClosed(EventArgs.Empty);
                });
        }

        /// <summary>
        ///   Shows a progress bar to the user. Use this if you need time to validate user data
        ///   Type progress bar overlay all other elements. It can't be interrupted
        ///   You may implement the IProgressElapsedCallBack interface.
        ///   If already a Message box should be shown, the Message box will be hidden
        /// </summary>
        /// <param name = "progressInfo">
        /// The progress information.
        /// </param>
        public void ShowProgressBar(IProgressBarInfo progressInfo)
        {
            this.SafeBeginInvoke(
                () =>
                    {
                        this.HideProgressBar();
                        this.HideSelection();
                        this.currentProgressBar = progressInfo;
                        this.progressBox.Caption = progressInfo.Caption;
                        this.progressBox.TitleColor = ColorInfoMsg;
                        this.progressBox.Visible = true;
                        this.progressBox.BringToFront();
                        this.progressBox.Start(TimeSpan.FromSeconds(progressInfo.MaxTime));
                    });
        }

        /// <summary>
        ///   Hides the progress bar
        /// </summary>
        public void HideProgressBar()
        {
            if (this.currentProgressBar == null)
            {
                return;
            }

            var progressBar = this.currentProgressBar;
            this.currentProgressBar = null;
            this.SafeBeginInvoke(
                () =>
                {
                    this.progressBox.Stop();
                    this.progressBox.Visible = false;
                    this.ShowSelection();
                    progressBar.ProgressElapsed();
                });
        }

        /// <summary>
        ///   Will be true if a ProgressBar or Message box is active!
        ///   In this case the current main field shouldn't change
        /// </summary>
        /// <returns>
        /// True if a ProgressBar or Message box is active.
        /// </returns>
        public bool IsLocked()
        {
            return this.currentMessageBox != null || this.currentProgressBar != null;
        }

        /// <summary>
        /// Processes the given key.
        /// </summary>
        /// <param name="key">
        /// The key. This is never <see cref="C74Keys.None"/>.
        /// </param>
        /// <returns>
        /// True if the key was handled otherwise false.
        /// </returns>
        public virtual bool ProcessKey(C74Keys key)
        {
            if (this.messageBox.Visible)
            {
                this.messageBox.ProcessKey(key);
                return true;
            }

            if (this.progressBox.Visible)
            {
                return true;
            }

            IC74Input input = null;

            // send the key to the selected child
            if (TraverseChildren(
                    this,
                    i =>
                        {
                            if (i.IsSelected)
                            {
                                input = i;
                            }

                            return i.IsSelected && i.ProcessKey(key);
                        }))
            {
                return true;
            }

            if (key == C74Keys.Back)
            {
                this.RaiseEscapePressed();
                return true;
            }

            if (key == C74Keys.Ok)
            {
                this.RaiseReturnPressed();
                return true;
            }

            if (input == null || (key != C74Keys.Up && key != C74Keys.Down))
            {
                return false;
            }

            this.ProcessNavigation(input, key);
            return true;
        }

        /// <summary>
        /// Initializes the children.
        /// This resets all children to not be selected.
        /// </summary>
        protected void Init()
        {
            TraverseChildren(this, i => i.IsSelected = false);
        }

        /// <summary>
        /// Raises the <see cref="EscapePressed"/> event.
        /// </summary>
        protected virtual void RaiseEscapePressed()
        {
            var handler = this.EscapePressed;
            if (handler != null)
            {
                handler(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// Raises the <see cref="ReturnPressed"/> event.
        /// </summary>
        protected virtual void RaiseReturnPressed()
        {
            var handler = this.ReturnPressed;
            if (handler != null)
            {
                handler(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// Raises the <see cref="MenuButtonPressed"/> event.
        /// </summary>
        protected virtual void RaiseMenuButtonPressed()
        {
            var handler = this.MenuButtonPressed;
            if (handler != null)
            {
                handler(this, EventArgs.Empty);
            }
        }

        private static bool TraverseChildren(Control control, Predicate<IC74Input> predicate)
        {
            var controls = new Control[control.Controls.Count];
            control.Controls.CopyTo(controls, 0);
            Array.Sort(controls, (a, b) => a.TabIndex.CompareTo(b.TabIndex));
            foreach (var child in controls)
            {
                if (!child.Visible)
                {
                    continue;
                }

                var input = child as IC74Input;
                if ((input != null && predicate(input)) || TraverseChildren(child, predicate))
                {
                    return true;
                }
            }

            return false;
        }

        private void ProcessNavigation(IC74Input focusInput, C74Keys key)
        {
            IC74Input first = null;
            IC74Input previous = null;
            IC74Input next = null;
            IC74Input last = null;
            var found = false;
            TraverseChildren(
                this,
                i =>
                    {
                        last = i;
                        if (first == null)
                        {
                            first = i;
                        }

                        if (i == focusInput)
                        {
                            found = true;
                        }
                        else if (!found)
                        {
                            previous = i;
                        }
                        else if (next == null)
                        {
                            next = i;
                        }

                        return false;
                    });

            if (first == last && first == focusInput)
            {
                // no other control found, we are still focused
                return;
            }

            focusInput.IsSelected = false;
            if (key == C74Keys.Up)
            {
                // navigate backwards, wrapping around if needed
                (previous ?? last).IsSelected = true;
            }
            else
            {
                // navigate forwards, wrapping around if needed
                (next ?? first).IsSelected = true;
            }
        }

        private void ShowSelection()
        {
            if (this.hiddenSelected == null)
            {
                return;
            }

            this.hiddenSelected.IsSelected = true;
            this.hiddenSelected = null;
        }

        private void HideSelection()
        {
            this.hiddenSelected = null;
            TraverseChildren(
                this,
                i =>
                    {
                        if (!i.IsSelected)
                        {
                            return false;
                        }

                        i.IsSelected = false;
                        this.hiddenSelected = i;
                        return true;
                    });
        }

        private void ButtonMenuOnPressed(object sender, EventArgs e)
        {
            this.RaiseMenuButtonPressed();
        }

        private void MessageBoxOnOkPressed(object sender, EventArgs e)
        {
            this.HideMessageBox();
        }

        private void ProgressBoxOnStopped(object sender, EventArgs e)
        {
            this.HideProgressBar();
        }
    }
}
