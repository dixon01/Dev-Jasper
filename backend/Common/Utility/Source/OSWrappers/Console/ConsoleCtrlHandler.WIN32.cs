// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ConsoleCtrlHandler.WIN32.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the ConsoleCtrlHandler type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Utility.OSWrappers.Console
{
    using System.Runtime.InteropServices;

    /// <summary>
    /// Handler for console control events.
    /// </summary>
    public sealed partial class ConsoleCtrlHandler
    {
        private readonly NativeMethods.ConsoleHandlerFunction consoleHandler;

        /// <summary>
        /// Initializes a new instance of the <see cref="ConsoleCtrlHandler"/> class.
        /// </summary>
        public ConsoleCtrlHandler()
        {
            this.consoleHandler = this.ConsoleCtrlCheck;
            NativeMethods.SetConsoleCtrlHandler(this.consoleHandler, true);
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <filterpriority>2</filterpriority>
        public void Dispose()
        {
            NativeMethods.SetConsoleCtrlHandler(this.consoleHandler, false);
        }

        private static class NativeMethods
        {
            public delegate int ConsoleHandlerFunction(CtrlTypes ctrlType);

            [DllImport("kernel32.dll", EntryPoint = "SetConsoleCtrlHandler")]
            [return: MarshalAs(UnmanagedType.Bool)]
            public static extern bool SetConsoleCtrlHandler(
                ConsoleHandlerFunction handlerFunction, [MarshalAs(UnmanagedType.Bool)] bool add);
        }
    }
}