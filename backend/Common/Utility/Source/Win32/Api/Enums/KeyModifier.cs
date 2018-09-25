// --------------------------------------------------------------------------------------------------------------------
// <copyright file="KeyModifier.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the KeyModifier type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Utility.Win32.Api.Enums
{
    using System;

    /// <summary>
    /// The enumeration of possible key modifiers.
    /// </summary>
    [Flags]
    public enum KeyModifier : uint
    {
        /// <summary>
        /// Either ALT key must be held down.
        /// </summary>
        Alt = 1,

        /// <summary>
        /// Either CTRL key must be held down.
        /// </summary>
        Control = 2,

        /// <summary>
        /// Either SHIFT key must be held down.
        /// </summary>
        Shift = 4,

        /// <summary>
        /// Either WINDOWS key was held down. These keys are labeled with the Windows logo.
        /// Keyboard shortcuts that involve the WINDOWS key are reserved for use by the operating system.
        /// </summary>
        Win = 8
    }
}
