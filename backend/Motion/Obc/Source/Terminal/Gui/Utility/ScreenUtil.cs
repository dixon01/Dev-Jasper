// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ScreenUtil.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the ScreenUtil type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Obc.Terminal.Gui.Utility
{
    using System;
    using System.Drawing;
    using System.Runtime.InteropServices;
    using System.Windows.Forms;

    using Gorba.Common.Utility.Core;

    using NLog;

    /// <summary>
    /// The screen utility.
    /// </summary>
    public static class ScreenUtil
    {
        private static readonly Logger Logger = LogHelper.GetLogger(typeof(ScreenUtil));

        private static bool? isIhmi;

        /// <summary>
        /// Gets a value indicating whether this application is running on IHMI.
        /// </summary>
        public static bool IsIhmi
        {
            get
            {
                if (!isIhmi.HasValue)
                {
                    isIhmi = Screen.PrimaryScreen.WorkingArea.Width >= 800;
                }

                return isIhmi.Value;
            }
        }

        /// <summary>
        /// Adapts a control to be shown correctly on IHMI.
        /// </summary>
        /// <param name="control">
        /// The control.
        /// </param>
        /// <param name="scaleChildren">
        /// A flag indicating whether to scale children.
        /// </param>
        /// <param name="scaleForm">
        /// A flag indicating whether to scale the given control.
        /// </param>
        public static void Adapt4Ihmi(Control control, bool scaleChildren, bool scaleForm)
        {
            if (!IsIhmi)
            {
                return;
            }

            try
            {
                if (scaleForm)
                {
                    control.Size = new Size(control.Size.Width * 2, control.Size.Height * 2);
                }

                foreach (Control child in control.Controls)
                {
                    if (child != null)
                    {
                        if (scaleChildren)
                        {
                            if ((child.Anchor & AnchorStyles.Right) == AnchorStyles.Right)
                            {
                                child.Location = new Point(
                                    (child.Location.X * 2) - child.Size.Width,
                                    child.Location.Y * 2);
                            }
                            else
                            {
                                child.Location = new Point(child.Location.X * 2, child.Location.Y * 2);
                            }

                            Adapt4Ihmi(child, true, true);
                        }

                        try
                        {
                            child.Font = new Font(child.Font.Name, child.Font.Size * 2, child.Font.Style);
                        }
                        catch (Exception ex)
                        {
                            Logger.TraceException("Couldn't scale font", ex);
                        }

                        var img = child as PictureBox;
                        if (img != null)
                        {
                            img.SizeMode = PictureBoxSizeMode.StretchImage;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.ErrorException("Adapt4Ihmi", ex);
            }
        }

        /// <summary>
        /// Sets the cursor visibility.
        /// </summary>
        /// <param name="show">
        /// A flag indicating whether to show the cursor.
        /// </param>
        public static void SetCursorVisible(bool show)
        {
            NativeMethods.ShowCursor(show);
        }

        /// <summary>
        /// Set Full Screen Mode.
        /// </summary>
        /// <param name = "form">
        /// The form to show full screen.
        /// </param>
        public static void StartFullScreen(Form form)
        {
            // Set Full Screen For Windows CE Device
            Screen screen = Screen.PrimaryScreen;

            // Normalize windows state
            form.WindowState = FormWindowState.Normal;

            IntPtr iptr = form.Handle;
            if (!IsIhmi)
            {
                NativeMethods.SHFullScreen(iptr, 0x22);
            }

            // detect taskbar height
            int taskbarHeight = screen.Bounds.Height - screen.WorkingArea.Height;

            // move the viewing window north taskbar height to get rid of the command bar
            //// MoveWindow(iptr, 0, -taskbarHeight, Screen.PrimaryScreen.Bounds.Width,
            ////   Screen.PrimaryScreen.Bounds.Height + taskbarHeight, 1);

            // move the task bar south taskbar height so that its not visible any longer
            IntPtr iptrTb = NativeMethods.FindWindowW("HHTaskBar", null);
            NativeMethods.MoveWindow(
                       iptrTb,
                       0,
                       screen.Bounds.Height,
                       screen.Bounds.Width,
                       taskbarHeight,
                       1);
        }

        private static class NativeMethods
        {
            /// <summary>
            ///   This function can be used to take over certain areas of the screen
            ///   It is used to modify the taskbar, Input Panel button,
            ///   or Start menu icon.
            /// </summary>
            /// <param name = "hwnd">The window handle</param>
            /// <param name = "state">The window state</param>
            /// <returns>True if successful</returns>
            [DllImport("aygshell.dll", SetLastError = true)]
            public static extern bool SHFullScreen(IntPtr hwnd, int state);

            /// <summary>
            ///   The function retrieves the handle to the top-level
            ///   window whose class name and window name match
            ///   the specified strings. This function does not search child windows.
            /// </summary>
            /// <param name = "lpClass">The class name</param>
            /// <param name = "lpWindow">The window name</param>
            /// <returns>A handle to the window.</returns>
            [DllImport("coredll.dll", SetLastError = true)]
            public static extern IntPtr FindWindowW(
                string lpClass,
                string lpWindow);

            /// <summary>
            ///   changes the position and dimensions of the specified window
            /// </summary>
            /// <param name = "hwnd">The window handle</param>
            /// <param name = "x">The x coordinate</param>
            /// <param name = "y">The y coordinate</param>
            /// <param name = "w">The width</param>
            /// <param name = "l">The height</param>
            /// <param name = "repaint">A flag indicating if it should be repainted</param>
            /// <returns>The window handle if successful</returns>
            [DllImport("coredll.dll", SetLastError = true)]
            public static extern IntPtr MoveWindow(
                IntPtr hwnd,
                int x,
                int y,
                int w,
                int l,
                int repaint);

            [DllImport("coredll.dll")]
            public static extern int ShowCursor(bool show);
        }
    }
}