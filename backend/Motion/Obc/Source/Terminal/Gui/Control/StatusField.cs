// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StatusField.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the StatusField type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Obc.Terminal.Gui.Control
{
    using System.Windows.Forms;

    using Gorba.Motion.Obc.Terminal.Core;
    using Gorba.Motion.Obc.Terminal.Gui.Utility;

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

            ScreenUtil.Adapt4Ihmi(this, true, false);
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
                this.BeginInvoke(new Action(() => this.SetMessage(message)));
            }
            else
            {
                this.lbl1.Text = message;
            }
        }
    }
}