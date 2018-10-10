// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SplashScreenFormBase.FX20.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Common.Utility.SplashScreen
{
    using System;
    using System.Diagnostics;
    using System.Reflection;

    using Gorba.Common.Utility.Core;
    using Gorba.Common.Utility.Win32.Api.DLLs;
    using Gorba.Common.Utility.Win32.Api.Enums;

    using NLog;

    /// <summary>
    /// The splash screen form base.
    /// </summary>
    public partial class SplashScreenFormBase
    {
        private IntPtr previousFocusedWindow = IntPtr.Zero;

        private static readonly Logger Logger = LogHelper.GetLogger<SplashScreenFormBase>();

        /// <summary>
        /// Shows the form.
        /// </summary>
        public virtual void ShowForm()
        {
            this.Show();
        }

        /// <summary>
        /// Hides the form.
        /// </summary>
        public virtual void HideForm()
        {
            Logger.Info(MethodBase.GetCurrentMethod().Name + $" previousFocusedWindow: {this.previousFocusedWindow}");
            
            this.Invoke(new Action(() =>
            {
                this.Hide();
            }));
            
            if (this.previousFocusedWindow != IntPtr.Zero)
            {
                User32.SetForegroundWindow(this.previousFocusedWindow);
                User32.ShowWindow(this.previousFocusedWindow, ShowWindow.Maximize);
            }
        }

        /// <summary>
        /// Brings this window forcefully to focus.
        /// </summary>
        public virtual void ForceFocus()
        {
            var focusWindow = User32.GetForegroundWindow(); // The application that is currently in Foreground
            Logger.Info($" Force Focus to Splash: saving previousFocusedWindow : {focusWindow}");

            this.Invoke(new Action(() =>
            {
                this.BringToFront(); // Bring the splash to Front
                this.Focus(); // Set focus to Splash
            }));
          
            this.previousFocusedWindow = focusWindow; // save the DX/Current window handle and minimize it
            if (focusWindow != IntPtr.Zero && focusWindow != this.Handle)
            {
                User32.ShowWindow(focusWindow, ShowWindow.Minimize);
            }
        }
    }
}
