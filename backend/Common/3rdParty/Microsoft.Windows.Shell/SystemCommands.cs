// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SystemCommands.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   The SystemCommands.cs
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Microsoft.Windows.Shell
{
    using System;
    using System.Windows;
    using System.Windows.Input;
    using System.Windows.Interop;
    using Standard;

    /// <summary>
    /// the SystemCommands
    /// </summary>
    public static class SystemCommands
    {
        static SystemCommands()
        {
            CloseWindowCommand = new RoutedCommand("CloseWindow", typeof(SystemCommands));
            MaximizeWindowCommand = new RoutedCommand("MaximizeWindow", typeof(SystemCommands));
            MinimizeWindowCommand = new RoutedCommand("MinimizeWindow", typeof(SystemCommands));
            RestoreWindowCommand = new RoutedCommand("RestoreWindow", typeof(SystemCommands));
            ShowSystemMenuCommand = new RoutedCommand("ShowSystemMenu", typeof(SystemCommands));
            ShowFileMenuCommand = new RoutedCommand("ShowFileMenuCommand", typeof(SystemCommands));
            ShowEditMenuCommand = new RoutedCommand("ShowEditMenuCommand", typeof(SystemCommands));
            ShowViewMenuCommand = new RoutedCommand("ShowViewMenuCommand", typeof(SystemCommands));                 
        }

        /// <summary>
        /// Gets the CloseWindowCommand
        /// </summary>
        public static RoutedCommand CloseWindowCommand { get; private set; }

        /// <summary>
        /// Gets the MaximizeWindowCommand
        /// </summary>
        public static RoutedCommand MaximizeWindowCommand { get; private set; }

        /// <summary>
        /// Gets the MinimizeWindowCommand
        /// </summary>
        public static RoutedCommand MinimizeWindowCommand { get; private set; }

        /// <summary>
        /// Gets the RestoreWindowCommand
        /// </summary>
        public static RoutedCommand RestoreWindowCommand { get; private set; }

        /// <summary>
        /// Gets the ShowSystemMenuCommand
        /// </summary>
        public static RoutedCommand ShowSystemMenuCommand { get; private set; }

        /// <summary>
        /// Gets the ShowFileMenuCommand
        /// </summary>
        public static RoutedCommand ShowFileMenuCommand { get; private set; }

        /// <summary>
        /// Gets the ShowEditMenuCommand
        /// </summary>
        public static RoutedCommand ShowEditMenuCommand { get; private set; }

        /// <summary>
        /// Gets the ShowViewMenuCommand
        /// </summary>
        public static RoutedCommand ShowViewMenuCommand { get; private set; }

        /// <summary>
        /// the close Window Handler
        /// </summary>
        /// <param name="window">the window</param>
        public static void CloseWindow(Window window)
        {
            Verify.IsNotNull(window, "window");
            _PostSystemCommand(window, SC.CLOSE);
        }

        /// <summary>
        /// the MaximizeWindow Handler
        /// </summary>
        /// <param name="window">the window</param>
        public static void MaximizeWindow(Window window)
        {
            Verify.IsNotNull(window, "window");
            _PostSystemCommand(window, SC.MAXIMIZE);
        }

        /// <summary>
        /// the MinimizeWindow Handler
        /// </summary>
        /// <param name="window">the window</param>
        public static void MinimizeWindow(Window window)
        {
            Verify.IsNotNull(window, "window");
            _PostSystemCommand(window, SC.MINIMIZE);
        }

        /// <summary>
        /// the RestoreWindow Handler
        /// </summary>
        /// <param name="window">the window</param>
        public static void RestoreWindow(Window window)
        {
            Verify.IsNotNull(window, "window");
            _PostSystemCommand(window, SC.RESTORE);
        }

        /// <summary>Display the system menu at a specified location.</summary>
        /// <param name="window">the window.</param>
        /// <param name="screenLocation">The location to display the system menu, in logical screen coordinates.</param>
        public static void ShowSystemMenu(Window window, Point screenLocation)
        {
            Verify.IsNotNull(window, "window");
            ShowSystemMenuPhysicalCoordinates(window, DpiHelper.LogicalPixelsToDevice(screenLocation));
        }

        /// <summary>
        /// the ShowSystemMenuPhysicalCoordinates
        /// </summary>
        /// <param name="window">the window </param>
        /// <param name="physicalScreenLocation">the physicalScreenLocation</param>
        public static void ShowSystemMenuPhysicalCoordinates(Window window, Point physicalScreenLocation)
        {
            const uint TPM_RETURNCMD = 0x0100;
            const uint TPM_LEFTBUTTON = 0x0;

            Verify.IsNotNull(window, "window");
            IntPtr hwnd = new WindowInteropHelper(window).Handle;
            if (hwnd == IntPtr.Zero || !NativeMethods.IsWindow(hwnd))
            {
                return;
            }

            IntPtr hmenu = NativeMethods.GetSystemMenu(hwnd, false);

            uint cmd = NativeMethods.TrackPopupMenuEx(hmenu, TPM_LEFTBUTTON | TPM_RETURNCMD, (int)physicalScreenLocation.X, (int)physicalScreenLocation.Y, hwnd, IntPtr.Zero);
            if (0 != cmd)
            {
                NativeMethods.PostMessage(hwnd, WM.SYSCOMMAND, new IntPtr(cmd), IntPtr.Zero);
            }
        }

        private static void _PostSystemCommand(Window window, SC command)
        {
            IntPtr hwnd = new WindowInteropHelper(window).Handle;
            if (hwnd == IntPtr.Zero || !NativeMethods.IsWindow(hwnd))
            {
                return;
            }

            NativeMethods.PostMessage(hwnd, WM.SYSCOMMAND, new IntPtr((int)command), IntPtr.Zero);
        }
    }
}
