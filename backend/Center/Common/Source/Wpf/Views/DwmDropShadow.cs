// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DwmDropShadow.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the DwmDropShadow type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Common.Wpf.Views
{
    using System;
    using System.Windows;
    using System.Windows.Interop;

    using Gorba.Common.Utility.Win32.Api;
    using Gorba.Common.Utility.Win32.Api.Enums;
    using Gorba.Common.Utility.Win32.Wrapper;

    /// <summary>
    /// Helper to add Shadow to windows
    /// </summary>
    public class DwmDropShadow
    {
        /// <summary>
        /// Drops a standard shadow to a WPF Window, even if the window is borderless.
        /// Only works with DWM (Vista and higher).
        /// This method is much more efficient than setting AllowsTransparency to true and using the DropShadow effect,
        /// as AllowsTransparency involves a huge performance issue
        /// (hardware acceleration is turned off for all the window).
        /// </summary>
        /// <param name="window">Window to which the shadow will be applied</param>
        public static void DropShadowToWindow(Window window)
        {
            if (!DropShadow(window))
            {
                window.SourceInitialized += WindowSourceInitialized;
            }
        }

        private static void WindowSourceInitialized(object sender, EventArgs e)
        {
            var window = (Window)sender;

            DropShadow(window);

            window.SourceInitialized -= WindowSourceInitialized;
        }

        /// <summary>
        /// The actual method that makes API calls to drop the shadow to the window
        /// </summary>
        /// <param name="window">Window to which the shadow will be applied</param>
        /// <returns>True if the method succeeded, false if not</returns>
        private static bool DropShadow(Window window)
        {
            try
            {
                var helper = new WindowInteropHelper(window);

                return DesktopWindowManager.SetNonClientRenderingPolicy(helper.Handle, DwmncRenderingPolicy.Enabled)
                       == HResult.Ok;
            }
            catch (Exception)
            {
                // Probably dwmapi.dll not found (incompatible OS)
                return false;
            }
        }
    }
}
