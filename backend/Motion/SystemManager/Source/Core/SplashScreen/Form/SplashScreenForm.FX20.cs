// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SplashScreenForm.FX20.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the SplashScreenForm type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.SystemManager.Core.SplashScreen.Form
{
    using System.Windows.Forms;

    using Gorba.Common.Utility.Compatibility;
    using Gorba.Common.Utility.Win32.Wrapper;

    /// <summary>
    /// The splash screen form.
    /// </summary>
    public partial class SplashScreenForm
    {
        /// <summary>
        /// Shows the form.
        /// </summary>
        public override void ShowForm()
        {
            base.ShowForm();
            this.updateScreenTimer.Enabled = true;
        }

        /// <summary>
        /// Hides the form.
        /// </summary>
        public override void HideForm()
        {
            base.HideForm();
            this.updateScreenTimer.Enabled = false;
        }

        /// <summary>
        /// Raises the <see cref="E:System.Windows.Forms.Control.Paint"/> event.
        /// </summary>
        /// <param name="e">The <see cref="PaintEventArgs"/> that contains the event data. </param>
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            this.PaintParts(e.Graphics);
        }

        partial void PrepareForm()
        {
            var cursor = Cursors.No;
            cursor.Dispose();
            this.Cursor = cursor;

            this.Icon = ShellFileInfo.GetFileIcon(ApplicationHelper.GetEntryAssemblyLocation(), false, false);
            this.ShowIcon = false;
            this.ShowInTaskbar = false;

            this.DoubleBuffered = true;
            this.SetStyle(
                ControlStyles.UserPaint
                | ControlStyles.AllPaintingInWmPaint
                | ControlStyles.ResizeRedraw
                | ControlStyles.OptimizedDoubleBuffer,
                true);

            if (!this.IsHandleCreated)
            {
                this.CreateHandle();
            }
        }
    }
}
