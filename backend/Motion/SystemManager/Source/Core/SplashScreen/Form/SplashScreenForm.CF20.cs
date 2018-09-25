// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SplashScreenForm.CF20.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the SplashScreenForm type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.SystemManager.Core.SplashScreen.Form
{
    using System;
    using System.Drawing;
    using System.Runtime.InteropServices;
    using System.Windows.Forms;

    /// <summary>
    /// The splash screen form.
    /// </summary>
    public partial class SplashScreenForm
    {
        private Bitmap backBuffer;

        private Rectangle desktopArea;

        private IntPtr inputPanelHWnd;
        private IntPtr sipButtonHWnd;
        private IntPtr taskBarHWnd;

        /// <summary>
        /// Shows the form.
        /// </summary>
        public override void ShowForm()
        {
            this.SetFullScreen(true);
            base.ShowForm();
            this.WindowState = FormWindowState.Maximized;
            this.updateScreenTimer.Enabled = true;
        }

        /// <summary>
        /// Hides the form.
        /// </summary>
        public override void HideForm()
        {
            base.HideForm();
            this.updateScreenTimer.Enabled = false;
            this.SetFullScreen(false);
        }

        /// <summary>
        /// Raises the <see cref="E:System.Windows.Forms.Control.Paint"/> event.
        /// </summary>
        /// <param name="e">The <see cref="PaintEventArgs"/> that contains the event data. </param>
        protected override void OnPaint(PaintEventArgs e)
        {
            if (this.backBuffer == null
                || this.backBuffer.Width < this.ClientSize.Width
                || this.backBuffer.Height < this.ClientSize.Height)
            {
                if (this.backBuffer != null)
                {
                    this.backBuffer.Dispose();
                }

                this.backBuffer = new Bitmap(this.ClientSize.Width, this.ClientSize.Height);
            }

            var graphics = Graphics.FromImage(this.backBuffer);
            graphics.Clear(this.BackColor);
            this.PaintParts(graphics);
            e.Graphics.DrawImage(this.backBuffer, 0, 0);

            base.OnPaint(e);
        }

        /// <summary>
        /// Blocks painting the background (not needed because we implement our own double-buffering).
        /// </summary>
        /// <param name="e">
        /// The event arguments.
        /// </param>
        protected override void OnPaintBackground(PaintEventArgs e)
        {
        }

        partial void PrepareForm()
        {
            this.desktopArea = Screen.PrimaryScreen.WorkingArea;

            this.inputPanelHWnd = NativeMethods.FindWindowW("SipWndClass", null);
            this.sipButtonHWnd = NativeMethods.FindWindowW("MS_SIPBUTTON", null);
            this.taskBarHWnd = NativeMethods.FindWindowW("HHTaskBar", null);
        }

        private void SetFullScreen(bool fullScreen)
        {
            var command = fullScreen ? NativeMethods.SW_HIDE : NativeMethods.SW_SHOW;
            if (fullScreen)
            {
                // only hide the input panel, don't show it again
                NativeMethods.ShowWindow(this.inputPanelHWnd, command);
            }

            NativeMethods.ShowWindow(this.sipButtonHWnd, command);
            NativeMethods.ShowWindow(this.taskBarHWnd, command);

            var bounds = fullScreen ? Screen.PrimaryScreen.Bounds : this.desktopArea;
            NativeMethods.SetWorkArea(bounds);
        }

        private static class NativeMethods
        {
            // ReSharper disable InconsistentNaming
            internal const int SW_HIDE = 0;
            internal const int SW_SHOW = 1;

            private const uint WM_SETTINGCHANGE = 0x1a;
            private const uint SPI_SETWORKAREA = 47;

            public static void ShowWindow(IntPtr hWnd, int nCmdShow)
            {
                if (hWnd != IntPtr.Zero)
                {
                    return;
                }

                _ShowWindow(hWnd, nCmdShow);
            }

            public static void SetWorkArea(Rectangle bounds)
            {
                var rect = new RECT(bounds);
                _SystemParametersInfo(SPI_SETWORKAREA, 0, ref rect, WM_SETTINGCHANGE);
            }

            [DllImport("coredll.dll")]
            internal static extern IntPtr FindWindowW(string lpClassName, string lpWindowName);

            [DllImport("coredll.dll", EntryPoint = "ShowWindow")]
            private static extern bool _ShowWindow(IntPtr hWnd, int nCmdShow);

            [DllImport("coredll.dll", EntryPoint = "SystemParametersInfo")]
            private static extern bool _SystemParametersInfo(uint uAction, uint uparam, ref RECT rect, uint fuWinIni);

            private struct RECT
            {
                // ReSharper disable NotAccessedField.Local
                private int Left;
                private int Top;
                private int Right;
                private int Bottom;

                // ReSharper restore NotAccessedField.Local
                public RECT(Rectangle rect)
                    : this()
                {
                    this.Left = rect.Left;
                    this.Right = rect.Left + rect.Width;
                    this.Top = rect.Top;
                    this.Bottom = rect.Top + rect.Height;
                }
            }
            // ReSharper restore InconsistentNaming
        }
    }
}
