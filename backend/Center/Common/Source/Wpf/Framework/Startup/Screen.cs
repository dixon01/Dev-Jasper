// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Screen.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the Screen type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Common.Wpf.Framework.Startup
{
    using System;
    using System.Diagnostics;
    using System.Drawing;
    using System.Linq;
    using System.Windows;

    /// <summary>
    /// Defines a screen.
    /// </summary>
    [DebuggerDisplay("{ScreenIdentifier}")]
    public class Screen
    {
        private Screen()
        {
        }

        /// <summary>
        /// Gets the identifier of the screen.
        /// </summary>
        public ScreenIdentifier ScreenIdentifier { get; private set; }

        /// <summary>
        /// Gets the working area of the screen.
        /// </summary>
        public Rectangle WorkingArea { get; private set; }

        /// <summary>
        /// Gets the primary screen.
        /// </summary>
        /// <returns>
        /// The primary <see cref="Screen"/>.
        /// </returns>
        public static Screen GetPrimaryScreen()
        {
            return GetScreen(ScreenIdentifier.Primary);
        }

        /// <summary>
        /// Gets the screen corresponding the given identifier.
        /// </summary>
        /// <param name="screenIdentifier">
        /// The screen identifier.
        /// </param>
        /// <returns>
        /// The <see cref="Screen"/> corresponding to the given identifier.
        /// </returns>
        public static Screen GetScreen(ScreenIdentifier screenIdentifier)
        {
            if (screenIdentifier == null)
            {
                screenIdentifier = ScreenIdentifier.Primary;
            }

            var formsScreen =
               System.Windows.Forms.Screen.AllScreens
               .SingleOrDefault(s => s.DeviceName == screenIdentifier.DeviceName);
            if (formsScreen == null || !screenIdentifier.IsScreenAvailable())
            {
                // The screen might be currently unavailable. We use the primary screen instead.
                screenIdentifier = ScreenIdentifier.Primary;
                return GetScreen(screenIdentifier);
            }

            return new Screen { ScreenIdentifier = screenIdentifier, WorkingArea = formsScreen.WorkingArea };
        }

        /// <summary>
        /// Moves the given window to this screen.
        /// </summary>
        /// <param name="window">
        /// The window.
        /// </param>
        /// <exception cref="ArgumentNullException">The <paramref name="window"/> is null.</exception>
        public void MoveWindow(Window window)
        {
            if (window == null)
            {
                throw new ArgumentNullException("window");
            }

            var screen =
                System.Windows.Forms.Screen.AllScreens
                .SingleOrDefault(s => s.DeviceName == this.ScreenIdentifier.DeviceName);
            if (screen == null)
            {
                return;
            }

            var workingArea = screen.WorkingArea;
            window.Height = FindValue(window.ActualHeight, window.Height, window.MinHeight, 600);
            window.Width = FindValue(window.ActualWidth, window.Width, window.MinWidth, 800);
            window.Left = workingArea.Left + ((workingArea.Width - window.Width) / 2);
            window.Top = workingArea.Top + ((workingArea.Height - window.Height) / 2);
            window.WindowState = WindowState.Normal;
            window.WindowStartupLocation = WindowStartupLocation.Manual;
        }

        private static double FindValue(double actualValue, double normalValue, double minValue, double defaultValue)
        {
            if (actualValue > 0)
            {
                return actualValue;
            }

            if (!double.IsNaN(normalValue))
            {
                return normalValue;
            }

            if (!double.IsNaN(minValue))
            {
                return minValue;
            }

            return defaultValue;
        }
    }
}