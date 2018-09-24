// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ApplySendButton.cs" company="Gorba AG">
//   Copyright © 2011-2012 Gorba AG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Protran.Controls
{
    using System;
    using System.Windows.Forms;

    /// <summary>
    /// <see cref="Button"/> subclass that supports pressing it with the shift key pressed.
    /// </summary>
    public class ApplySendButton : Button
    {
        private const string ApplyText = "Apply";
        private const string SendText = "Send";

        private readonly Timer shiftCheckTimer;

        /// <summary>
        /// Initializes a new instance of the <see cref="ApplySendButton"/> class.
        /// </summary>
        public ApplySendButton()
        {
            this.shiftCheckTimer = new Timer { Interval = 10 };
            this.shiftCheckTimer.Tick += (sender, args) => this.UpdateText();
        }

        /// <summary>
        /// Event that is fired whenever the shift key is pressed when clicking this button.
        /// </summary>
        public event EventHandler SendClick;

        /// <summary>
        /// Raises the <see cref="SendClick"/> event.
        /// </summary>
        /// <param name="e">the event arguments</param>
        protected virtual void OnSendClick(EventArgs e)
        {
            var handler = this.SendClick;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        /// <inheritdoc/>
        protected override void OnClick(EventArgs e)
        {
            base.OnClick(e);
            if (this.Text == SendText)
            {
                this.OnSendClick(e);
            }
        }

        /// <inheritdoc/>
        protected override void OnMouseEnter(EventArgs e)
        {
            this.UpdateText();
            this.shiftCheckTimer.Enabled = true;

            base.OnMouseEnter(e);
        }

        /// <inheritdoc/>
        protected override void OnMouseHover(EventArgs e)
        {
            this.UpdateText();
            base.OnMouseHover(e);
        }

        /// <inheritdoc/>
        protected override void OnMouseMove(MouseEventArgs mevent)
        {
            this.UpdateText();
            base.OnMouseMove(mevent);
        }

        /// <inheritdoc/>
        protected override void OnMouseLeave(EventArgs e)
        {
            this.UpdateText(true);
            base.OnMouseLeave(e);
        }

        /// <inheritdoc/>
        protected override void OnKeyDown(KeyEventArgs kevent)
        {
            this.UpdateText();
            base.OnKeyDown(kevent);
        }

        private void UpdateText(bool force = false)
        {
            bool shiftPressed = (ModifierKeys & Keys.Shift) != 0;
            var text = shiftPressed ? SendText : ApplyText;
            if (!force && this.Text == text)
            {
                return;
            }

            this.shiftCheckTimer.Enabled = shiftPressed;

            this.Text = text;
        }
    }
}
