// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UpdateProgressForm.CF35.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the UpdateProgressForm type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Update.Core.Progress
{
    using System.Drawing;

    /// <summary>
    /// Form that shows the current update progress.
    /// </summary>
    public partial class UpdateProgressForm
    {
        /// <summary>
        /// Shows this form.
        /// </summary>
        public override void ShowForm()
        {
            base.ShowForm();
            this.BringToFront();
            this.Focus();
        }

        partial void PrepareForm()
        {
            this.panelVisibleArea.Location = this.VisibleArea.Location;

            var width = this.VisibleArea.Width >= 0 ? this.VisibleArea.Width : this.Width;
            var height = this.VisibleArea.Height >= 0 ? this.VisibleArea.Height : this.Height;
            this.panelVisibleArea.Size = new Size(width, height);

            this.panelVisibleArea.BackColor = this.BackColor;
            this.panel1.BackColor = this.BackColor;
            this.pictureBox1.BackColor = this.BackColor;
        }
    }
}
