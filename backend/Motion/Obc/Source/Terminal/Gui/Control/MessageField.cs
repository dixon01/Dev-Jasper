// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MessageField.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the MessageField type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Obc.Terminal.Gui.Control
{
    using System;
    using System.Drawing;
    using System.Windows.Forms;

    using Gorba.Motion.Obc.Terminal.Core;
    using Gorba.Motion.Obc.Terminal.Gui.Utility;

    /// <summary>
    /// The message field.
    /// </summary>
    public partial class MessageField : UserControl, IMessageField
    {
        /// <summary>
        /// The destination text background color.
        /// </summary>
        protected readonly Color ColorDestTextBg = Color.FromArgb(51, 51, 51);

        ////protected readonly Color colorDefaultBG = Color.FromArgb(13, 32, 134); //Color.FromArgb(51, 51, 51);

        /// <summary>
        /// The destination text foreground color.
        /// </summary>
        protected readonly Color ColorDestTextFg = Color.White;

        /// <summary>
        /// The error background color.
        /// </summary>
        protected readonly Color ColorErrorBg = Color.FromArgb(204, 2, 47);

        /// <summary>
        /// The error foreground color.
        /// </summary>
        protected readonly Color ColorErrorFg = Color.White;

        /// <summary>
        /// The information background color.
        /// </summary>
        protected readonly Color ColorInfoBg = Color.FromArgb(13, 32, 134);

        /// <summary>
        /// The information foreground color.
        /// </summary>
        protected readonly Color ColorInfoFg = Color.White;

        /// <summary>
        /// The instruction background color.
        /// </summary>
        protected readonly Color ColorInstructionBg = Color.FromArgb(235, 184, 29);

        /// <summary>
        /// The instruction foreground color.
        /// </summary>
        protected readonly Color ColorInstructionFg = Color.Black;

        ////protected readonly Color colorDefaultFG = Color.FromArgb(51, 51, 51);

        private int msgId;

        /// <summary>
        /// Initializes a new instance of the <see cref="MessageField"/> class.
        /// </summary>
        public MessageField()
        {
            this.InitializeComponent();
            this.ShowMessage(MessageType.Info, string.Empty, 0);

            ScreenUtil.Adapt4Ihmi(this, true, false);
        }

        /// <summary>
        ///   If the message was confirmed from the driver
        /// </summary>
        public event EventHandler<IndexEventArgs> Confirmed;

        /*   private void ReadColor()
    {
        while (true)
        {
            try
            {
                Console.Write("Enter color code: ");
                string input = Console.ReadLine();
                tmpColor = Color.FromArgb(int.Parse(input, System.Globalization.NumberStyles.AllowHexSpecifier));
                SetColor();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Invalid input!!! " + ex.Message);
            }
        }
    }

    private void SetColor()
    {
        if (this.InvokeRequired)
        {
            this.BeginInvoke(new SimpleDelegate(SetColor));
        }
        else
        {
            colorInfoBG = tmpColor;
            lblMessage.BackColor = colorInfoBG;
            this.BackColor = colorInfoBG;
        }
    }*/

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
                this.picMessage.Visible = false;
                this.lblMessage.Text = txt;
                this.lblMessage.BackColor = this.ColorDestTextBg;
                this.lblMessage.ForeColor = this.ColorDestTextFg;
                this.BackColor = this.ColorDestTextBg;
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
                this.lblMessage.Text = text;
                this.msgId = messageId;
                Color colorBack;
                Color colorText;

                switch (type)
                {
                    case MessageType.Error:
                    case MessageType.Alarm:
                        colorBack = this.ColorErrorBg;
                        colorText = this.ColorErrorFg;
                        this.picMessage.Visible = false;
                        break;
                    case MessageType.Instruction:
                        colorBack = this.ColorInstructionBg;
                        colorText = this.ColorInstructionFg;
                        this.picMessage.Visible = true;
                        break;
                        ////case MessageType.Info:
                    default:
                        colorBack = this.ColorInfoBg;
                        colorText = this.ColorInfoFg;
                        this.picMessage.Visible = false;
                        break;
                }

                this.lblMessage.BackColor = colorBack;
                this.lblMessage.ForeColor = colorText;
                this.BackColor = colorBack;
            }
        }

        private void PicMessageClick(object sender, EventArgs e)
        {
            if (this.Confirmed != null)
            {
                this.Confirmed(this, new IndexEventArgs(this.msgId));
            }
        }

        /*  private void HideMessage()
    {
        lblMessage.Text = "";
        lblMessage.BackColor = colorDefaultBG;
        lblMessage.ForeColor = colorDefaultFG;
        this.BackColor = colorDefaultBG;
        picMessage.Visible = false;
    }*/

        private void MessageFieldClick(object sender, EventArgs e)
        {
            if (this.picMessage.Visible)
            {
                if (this.Confirmed != null)
                {
                    this.Confirmed(this, new IndexEventArgs(this.msgId));
                }
            }
        }
    }
}