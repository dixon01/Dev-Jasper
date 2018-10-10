// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DwmApi.FX20.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the DwmApi type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Utility.Win32.Api.DLLs
{
    using System;
    using System.Runtime.InteropServices;

    using Gorba.Common.Utility.Win32.Api.Enums;

    /// <summary>
    /// Wrapper for the <c>dwmapi.dll</c>.
    /// </summary>
    public static partial class DwmApi
    {
        /// <summary>
        /// Sets the value of non-client rendering attributes for a window.
        /// </summary>
        /// <param name="hwnd">
        /// The handle to the window that will receive the attributes.
        /// </param>
        /// <param name="attr">
        /// A single <see cref="DwmWindowAttribute"/> flag to apply to the window.
        /// This parameter specifies the attribute and the attribute parameter points to the value of that attribute.
        /// </param>
        /// <param name="attribute">
        /// A pointer to the value of the attribute specified in the <paramref name="attr"/> parameter.
        /// Different <see cref="DwmWindowAttribute"/> flags require different value types.
        /// </param>
        /// <param name="attrSize">
        /// The size, in bytes, of the value type pointed to by the attribute parameter.
        /// </param>
        /// <returns>
        /// If this function succeeds, it returns S_OK. Otherwise, it returns an HRESULT error code.
        /// </returns>
        [DllImport("dwmapi.dll", PreserveSig = true)]
        public static extern int DwmSetWindowAttribute(
            IntPtr hwnd, DwmWindowAttribute attr, ref int attribute, int attrSize);
    }
}
