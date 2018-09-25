// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AccessViolation.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the AccessViolation type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace AccessViolationConsole
{
    using System;
    using System.Runtime.InteropServices;

    /// <summary>
    /// The access violation.
    /// </summary>
    public class AccessViolation
    {
        /// <summary>
        /// The start.
        /// </summary>
        public void Start()
        {
            var ptr = new IntPtr(123);
            Marshal.StructureToPtr(123, ptr, true);
        }
    }
}