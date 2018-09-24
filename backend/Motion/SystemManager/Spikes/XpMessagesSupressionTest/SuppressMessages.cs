// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SuppressMessages.cs" company="Gorba AG">
//   Copyright © 2011-2012 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the SuppressMessages type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace XpMessagesSupressionTest
{
    using System;
    using System.Runtime.InteropServices;
    using System.Windows.Forms;

    /// <summary>
    /// Class to supress and display messages from operating system
    /// </summary>
    public partial class SuppressMessages : Form
    {
        private const uint WmQueryendsession = 0x0011;

        private const uint WmTimechange = 0x001E;

        private uint queryCancelAutoPlayId;

        /// <summary>
        /// Initializes a new instance of the <see cref="SuppressMessages"/> class.
        /// </summary>
        public SuppressMessages()
        {
            this.InitializeComponent();
            this.queryCancelAutoPlayId = Win32.RegisterWindowMessage("QueryCancelAutoPlay");
        }

        /// <summary>
        /// Override WndProc method
        /// </summary>
        /// <param name="m">
        /// The m.
        /// </param>
        protected override void WndProc(ref Message m)
        {
            base.WndProc(ref m);
            if (m.Msg == this.queryCancelAutoPlayId)
            {
                m.Result = (IntPtr)1;
                this.richTextBox1.AppendText("Cancel autoplay");
                this.richTextBox1.AppendText(Environment.NewLine);
            }

            if (m.Msg == WmQueryendsession)
            {
                m.Result = (IntPtr)0;
                this.richTextBox1.AppendText("Session ending");
                this.richTextBox1.AppendText(Environment.NewLine);
            }

            if (m.Msg == WmTimechange)
            {
                m.Result = (IntPtr)1;
                this.richTextBox1.AppendText("Cancel time change");
                this.richTextBox1.AppendText(Environment.NewLine);
            }
        }

        /// <summary>
        /// Win32 class
        /// </summary>
        internal static class Win32
        {
            [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
            internal static extern uint RegisterWindowMessage(string ipstring);
        }
    }
}
