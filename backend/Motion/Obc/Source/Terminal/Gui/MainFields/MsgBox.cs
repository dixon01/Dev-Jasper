// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MsgBox.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the MsgBox type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Obc.Terminal.Gui.MainFields
{
    using System;
    using System.Drawing;
    using System.Windows.Forms;

    using Gorba.Motion.Obc.Terminal.Core;
    using Gorba.Motion.Obc.Terminal.Gui.Hardware;
    using Gorba.Motion.Obc.Terminal.Gui.Utility;

    /// <summary>
    /// The message box.
    /// </summary>
    internal partial class MsgBox : UserControl
    {
        private static readonly Color ColorErrorBg = Color.FromArgb(204, 2, 47);

        private static readonly Color ColorInfoBg = Color.FromArgb(13, 32, 134); // (51, 51, 51);

        private static readonly Color ColorWarningBg = Color.FromArgb(235, 184, 29);

        private readonly KeyBoard keyboard;

        private bool isInitialized;

        /// <summary>
        /// Initializes a new instance of the <see cref="MsgBox"/> class.
        /// </summary>
        /// <param name="msgBoxInfo">
        /// The message box info.
        /// </param>
        public MsgBox(MessageBoxInfo msgBoxInfo)
        {
            this.InitializeComponent();

            ScreenUtil.Adapt4Ihmi(this, true, true);

            this.Info = msgBoxInfo;
            this.keyboard = KeyBoard.Instance;
            this.keyboard.KeyboardEnabled = false;
            ////keyboard.KeyPressed += new Device.HAL.VT3.Keyboard.KeyHandler(keyboard_KeyPressed);

            this.lblCaption.Text = msgBoxInfo.Caption;
            this.lblMessage.Text = msgBoxInfo.Message;
            switch (msgBoxInfo.Type)
            {
                case MessageBoxInfo.MsgType.Error:
                    this.lblCaption.BackColor = ColorErrorBg;
                    break;
                case MessageBoxInfo.MsgType.Info:
                    this.lblCaption.BackColor = ColorInfoBg;
                    break;
                case MessageBoxInfo.MsgType.Warning:
                    this.lblCaption.BackColor = ColorWarningBg;
                    break;
            }
        }

        /// <summary>
        /// The ok clicked event.
        /// </summary>
        public event EventHandler OkClicked;

        /*    private void keyboard_KeyPressed(Device.HAL.VT3.Keyboard k, Device.HAL.VT3.Keyboard.KeyArgs e)
        {
            Console.WriteLine("keyboard_KeyPressed Event. Allowed? At the moment it's disabled");
            return;
            if (e.type == Device.HAL.VT3.Keyboard.KeyType.Return)
            {
                this.Hide();
            }
        }*/

        /// <summary>
        /// Gets the message box info.
        /// </summary>
        public MessageBoxInfo Info { get; private set; }

        private void BtnOkClick(object sender, EventArgs e)
        {
            ////keyboard.KeyPressed -= new Device.HAL.VT3.Keyboard.KeyHandler(keyboard_KeyPressed);
            this.keyboard.KeyboardEnabled = true;
            this.Visible = false;
            this.Hide();

            this.OnOkClicked(EventArgs.Empty);
        }

        private void OnOkClicked(EventArgs e)
        {
            if (this.OkClicked != null)
            {
                this.OkClicked(this, e);
            }
        }

        private void MsgBoxPaint(object sender, PaintEventArgs e)
        {
            if (this.isInitialized == false)
            {
                this.isInitialized = true;
                this.BringToFront();
                this.Focus();
            }
        }

        private void MsgBoxEnabledChanged(object sender, EventArgs e)
        {
            if (this.Enabled == false)
            {
                this.Enabled = true;
            }
        }
    }
}