// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MessageField.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the MessageField type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Obc.Terminal.C74.Controls
{
    using System;
    using System.Drawing;
    using System.Windows.Forms;

    using Gorba.Motion.Obc.Terminal.Core;

    /// <summary>
    /// The message field.
    /// </summary>
    public partial class MessageField : UserControl, IMessageField, IC74Input
    {
        private static readonly Color ColorDestTextBg = Color.FromArgb(51, 51, 51);

        private static readonly Color ColorDestTextFg = Color.White;

        private static readonly Color ColorErrorBg = Color.FromArgb(204, 2, 47);

        private static readonly Color ColorErrorFg = Color.White;

        private static readonly Color ColorInfoBg = Color.FromArgb(13, 32, 134);

        private static readonly Color ColorInfoFg = Color.White;

        private static readonly Color ColorInstructionBg = Color.FromArgb(235, 184, 29);

        private static readonly Color ColorInstructionFg = Color.Black;

        private int msgId;

        /// <summary>
        /// Initializes a new instance of the <see cref="MessageField"/> class.
        /// </summary>
        public MessageField()
        {
            this.InitializeComponent();

            this.ShowMessage(MessageType.Info, string.Empty, 0);

            this.labelOK.BackColor = ColorInstructionBg;
            this.labelOK.ForeColor = ColorInstructionFg;
        }

        /// <summary>
        ///   If the message was confirmed from the driver
        /// </summary>
        public event EventHandler<IndexEventArgs> Confirmed;

        /// <summary>
        /// Gets or sets a value indicating whether this control is selected.
        /// </summary>
        public bool IsSelected
        {
            get
            {
                return this.labelOK.Visible;
            }

            set
            {
                this.labelOK.Visible = value;
            }
        }

        /// <summary>
        /// This text will be shown if no other messages are active. Add here the destination
        /// </summary>
        /// <param name="txt">
        /// The text.
        /// </param>
        public void SetDestinationText(string txt)
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke(new Action(() => this.SetDestinationText(txt)));
            }
            else
            {
                this.IsSelected = false;
                this.label.Text = txt;
                this.label.BackColor = ColorDestTextBg;
                this.label.ForeColor = ColorDestTextFg;
                this.BackColor = ColorDestTextBg;
            }
        }

        /// <summary>
        ///   Shows a message
        /// </summary>
        /// <param name = "type">type of the message</param>
        /// <param name = "text">message text</param>
        /// <param name = "messageId">the ID of this message</param>
        public void ShowMessage(MessageType type, string text, int messageId)
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke(new Action(() => this.ShowMessage(type, text, messageId)));
            }
            else
            {
                this.label.Text = text;
                this.msgId = messageId;
                Color colorBack;
                Color colorText;

                switch (type)
                {
                    case MessageType.Error:
                    case MessageType.Alarm:
                        colorBack = ColorErrorBg;
                        colorText = ColorErrorFg;
                        this.IsSelected = false;
                        break;
                    case MessageType.Instruction:
                        colorBack = ColorInstructionBg;
                        colorText = ColorInstructionFg;
                        this.IsSelected = true;
                        break;
                    ////case MessageType.Info:
                    default:
                        colorBack = ColorInfoBg;
                        colorText = ColorInfoFg;
                        this.IsSelected = false;
                        break;
                }

                this.label.BackColor = colorBack;
                this.label.ForeColor = colorText;
                this.BackColor = colorBack;
            }
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
        public bool ProcessKey(C74Keys key)
        {
            if (!this.IsSelected)
            {
                return false;
            }

            if (key == C74Keys.Ok)
            {
                this.RaiseConfirmed(new IndexEventArgs(this.msgId));
            }

            return true;
        }

        /// <summary>
        /// Raises the <see cref="Confirmed"/> event.
        /// </summary>
        /// <param name="e">
        /// The event arguments.
        /// </param>
        protected virtual void RaiseConfirmed(IndexEventArgs e)
        {
            var handler = this.Confirmed;
            if (handler != null)
            {
                handler(this, e);
            }
        }
    }
}
