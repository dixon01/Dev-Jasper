// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StatusField.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the StatusField type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Obc.Terminal.C74.Controls
{
    using System;
    using System.Windows.Forms;

    using Gorba.Motion.Obc.Terminal.Core;

    /// <summary>
    /// The status field.
    /// </summary>
    public partial class StatusField : UserControl, IStatusField
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StatusField"/> class.
        /// </summary>
        public StatusField()
        {
            this.InitializeComponent();
        }

        // prevent the actual mouse click from working on the status field (doesn't help in C74 anyways)
        event EventHandler IStatusField.Click
        {
            add
            {
            }

            remove
            {
            }
        }

        /// <summary>
        ///   Adds a text to be shown.
        /// </summary>
        /// <param name = "message">
        /// The message.
        /// </param>
        public void SetMessage(string message)
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke(new MethodInvoker(() => this.SetMessage(message)));
                return;
            }

            this.label.Text = message;
        }
    }
}
