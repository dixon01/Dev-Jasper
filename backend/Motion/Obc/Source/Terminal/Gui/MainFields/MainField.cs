// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MainField.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the MainField type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Obc.Terminal.Gui.MainFields
{
    using System;
    using System.Windows.Forms;

    using Gorba.Motion.Obc.Terminal.Core;

    /// <summary>
    /// The main field base class.
    /// </summary>
    public partial class MainField : UserControl, IMainField
    {
        private bool isLocked;

        private MsgBox messageBox;

        private ProgressBar progressBar;

        /// <summary>
        /// Initializes a new instance of the <see cref="MainField"/> class.
        /// </summary>
        protected MainField()
        {
            this.InitializeComponent();

            this.KeyDispatcher = new KeyDispatcherImpl(this);
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
        /// The event which will be called when user pressed UP.
        /// </summary>
        public event EventHandler UpPressed;

        /// <summary>
        /// The event which will be called when user pressed DOWN.
        /// </summary>
        public event EventHandler DownPressed;

        /// <summary>
        /// The key dispatcher interface.
        /// </summary>
        public interface IKeyDispatcher
        {
            /// <summary>
            /// Dispatches the ESC key event.
            /// </summary>
            void KeyEscPressed();

            /// <summary>
            /// Dispatches the UP key event.
            /// </summary>
            void KeyUpPressed();

            /// <summary>
            /// Dispatches the DOWN key event.
            /// </summary>
            void KeyDownPressed();

            /// <summary>
            /// Dispatches the ENTER key event.
            /// </summary>
            void KeyReturnPressed();
        }

        /// <summary>
        /// Gets the key dispatcher.
        /// </summary>
        public IKeyDispatcher KeyDispatcher { get; private set; }

        /// <summary>
        /// Gets a value indicating whether this main field is active.
        /// </summary>
        protected bool IsActive { get; private set; }

        /// <summary>
        /// Make sure that calling code is running in the GUI thread!!!
        ///   Check with GetControl().InvokeRequired
        /// </summary>
        /// <param name="visible">
        /// A flag indicating if this field should be made visible.
        /// </param>
        public virtual void MakeVisible(bool visible)
        {
            this.IsActive = visible;
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
        public virtual void ShowMessageBox(MessageBoxInfo msgBoxInfo)
        {
            this.SafeBeginInvoke(
                () =>
                    {
                        this.HideMessageBox();
                        this.isLocked = true;
                        this.SetControlsEnable(false);
                        this.messageBox = new MsgBox(msgBoxInfo);
                        this.messageBox.OkClicked += (s, e) => this.HideMessageBox();
                        this.Controls.Add(this.messageBox);
                    });
        }

        /// <summary>
        ///   Hides the message box
        /// </summary>
        public virtual void HideMessageBox()
        {
            this.isLocked = false;
            if (this.messageBox == null)
            {
                return;
            }

            this.SafeBeginInvoke(
                () =>
                    {
                        this.Controls.Remove(this.messageBox);
                        MsgBox msgBox = this.messageBox;
                        this.messageBox = null;
                        this.SetControlsEnable(true);

                        // do this at the very end, so this.messageBox is already null (break possible recursion)
                        msgBox.Info.OnClosed(EventArgs.Empty);
                    });
        }

        /// <summary>
        ///   Shows a progress bar to the user. Use this if you need time to validate user data
        ///   Type progress bar overlay all other elements. It can't be interrupted
        ///   You may implement the IProgressElapsedCallBack interface.
        ///   If already a Message box should be shown, the Message box will be hidden
        /// </summary>
        /// <param name = "progressBarInfo">
        /// The progress information.
        /// </param>
        public virtual void ShowProgressBar(IProgressBarInfo progressBarInfo)
        {
            this.SafeBeginInvoke(
                () =>
                    {
                        this.HideProgressBar();
                        this.isLocked = true;
                        this.SetControlsEnable(false);
                        this.progressBar = new ProgressBar(progressBarInfo);
                        this.progressBar.Closed += (s, e) => this.HideProgressBar();
                        this.Controls.Add(this.progressBar);
                    });
        }

        /// <summary>
        ///   Hides the progress bar
        /// </summary>
        public virtual void HideProgressBar()
        {
            this.isLocked = false;
            if (this.progressBar == null)
            {
                return;
            }

            this.SafeBeginInvoke(
                () =>
                    {
                        this.Controls.Remove(this.progressBar);
                        this.progressBar = null;
                        this.SetControlsEnable(true);
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
            return this.isLocked;
        }

        /// <summary>
        /// The safe begin invoke.
        /// </summary>
        /// <param name="action">
        /// The action.
        /// </param>
        public void SafeBeginInvoke(Action action)
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
        /// The safe invoke.
        /// </summary>
        /// <param name="action">
        /// The action.
        /// </param>
        protected internal void SafeInvoke(Action action)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(action);
            }
            else
            {
                action();
            }
        }

        /// <summary>
        /// Raises the <see cref="EscapePressed"/> event.
        /// </summary>
        /// <param name="e">
        /// The event arguments.
        /// </param>
        protected virtual void OnEscapePressed(EventArgs e)
        {
            if (this.EscapePressed != null)
            {
                this.EscapePressed(this, e);
            }
        }

        /// <summary>
        /// Raises the <see cref="ReturnPressed"/> event.
        /// </summary>
        /// <param name="e">
        /// The event arguments.
        /// </param>
        protected virtual void OnReturnPressed(EventArgs e)
        {
            if (this.ReturnPressed != null)
            {
                this.ReturnPressed(this, e);
            }
        }

        /// <summary>
        /// Raises the <see cref="UpPressed"/> event.
        /// </summary>
        /// <param name="e">
        /// The event arguments.
        /// </param>
        protected virtual void OnUpPressed(EventArgs e)
        {
            if (this.UpPressed != null)
            {
                this.UpPressed(this, e);
            }
        }

        /// <summary>
        /// Raises the <see cref="DownPressed"/> event.
        /// </summary>
        /// <param name="e">
        /// The event arguments.
        /// </param>
        protected virtual void OnDownPressed(EventArgs e)
        {
            if (this.DownPressed != null)
            {
                this.DownPressed(this, e);
            }
        }

        private void SetControlsEnable(bool enable)
        {
            foreach (Control c in this.Controls)
            {
                c.Enabled = enable;
            }
        }

        private class KeyDispatcherImpl : IKeyDispatcher
        {
            private readonly MainField mainField;

            public KeyDispatcherImpl(MainField mainField)
            {
                this.mainField = mainField;
            }

            public void KeyEscPressed()
            {
                this.mainField.OnEscapePressed(EventArgs.Empty);
            }

            public void KeyReturnPressed()
            {
                this.mainField.OnReturnPressed(EventArgs.Empty);
            }

            public void KeyUpPressed()
            {
                this.mainField.OnUpPressed(EventArgs.Empty);
            }

            public void KeyDownPressed()
            {
                this.mainField.OnDownPressed(EventArgs.Empty);
            }
        }
    }
}