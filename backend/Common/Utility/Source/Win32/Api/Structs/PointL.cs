// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PointL.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Utility.Win32.Api.Structs
{
    using System.Runtime.InteropServices;

    /// <summary>
    /// The POINTL  structure contains the coordinates of a point.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct PointL
    {
        /// <summary>
        /// The horizontal (x) coordinate of the point.
        /// </summary>
        [MarshalAs(UnmanagedType.I4)]
        public int X;

        /// <summary>
        /// The vertical (y) coordinate of the point.
        /// </summary>
        [MarshalAs(UnmanagedType.I4)]
        public int Y;
    }
}
