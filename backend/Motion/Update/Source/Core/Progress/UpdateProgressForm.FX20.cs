// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UpdateProgressForm.FX20.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the UpdateProgressForm type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Update.Core.Progress
{
    using System;
    using System.Drawing;
    using System.Windows.Forms;

    using Gorba.Common.Utility.Compatibility;
    using Gorba.Common.Utility.Win32.Wrapper;

    /// <summary>
    /// Form that shows the current update progress.
    /// </summary>
    public partial class UpdateProgressForm
    {
        /// <summary>
        /// Hides this form and clears all data for it to be reused later.
        /// </summary>
        public override void HideForm()
        {
            this.finished = false;
            this.errorMessages.Clear();
            base.HideForm();
        }

        /// <summary>
        /// Raises the <see cref="Control.Layout"/> event.
        /// </summary>
        /// <param name="e">
        /// The event arguments.
        /// </param>
        protected override void OnLayout(LayoutEventArgs e)
        {
            base.OnLayout(e);

            this.panelVisibleArea.Location = this.VisibleArea.Location;
            var width = this.VisibleArea.Width >= 0 ? this.VisibleArea.Width : this.Width;
            var height = this.VisibleArea.Height >= 0 ? this.VisibleArea.Height : this.Height;
            this.panelVisibleArea.Size = new Size(width, height);
        }

        /// <summary>
        /// Sets the control to the specified visible state.
        /// </summary>
        /// <param name="value">true to make the control visible; otherwise, false.</param>
        protected override void SetVisibleCore(bool value)
        {
            base.SetVisibleCore(value);

            if (!value)
            {
                return;
            }

            this.ForceFocus();
        }

        partial void PrepareForm()
        {
            Application.EnableVisualStyles();
            this.progressBar.Style = ProgressBarStyle.Continuous;

            this.Icon = ShellFileInfo.GetFileIcon(ApplicationHelper.GetEntryAssemblyLocation(), false, false);
            this.labelState.AutoEllipsis = true;

            this.panelVisibleArea.Margin = new Padding(0);

            if (this.Handle == IntPtr.Zero)
            {
                this.CreateHandle();
            }
        }
    }
}
