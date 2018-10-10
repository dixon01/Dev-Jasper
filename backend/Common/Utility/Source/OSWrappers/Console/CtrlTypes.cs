// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CtrlTypes.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the CtrlTypes type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Utility.OSWrappers.Console
{
    /// <summary>
    /// The console control event types.
    /// </summary>
    public enum CtrlTypes : uint
    {
        /// <summary>
        /// The CtrlCEvent.
        /// </summary>
        CtrlCEvent = 0,

        /// <summary>
        /// The CtrlBreakEvent.
        /// </summary>
        CtrlBreakEvent = 1,

        /// <summary>
        /// The CtrlCloseEvent.
        /// </summary>
        CtrlCloseEvent = 2,

        /// <summary>
        /// The CtrlLogoffEvent.
        /// </summary>
        CtrlLogoffEvent = 5,

        /// <summary>
        /// The CtrlShutdownEvent.
        /// </summary>
        CtrlShutdownEvent = 6
    }
}