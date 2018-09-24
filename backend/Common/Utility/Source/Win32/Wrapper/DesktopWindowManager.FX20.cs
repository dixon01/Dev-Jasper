// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DesktopWindowManager.FX20.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the DesktopWindowManager type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Utility.Win32.Wrapper
{
    using System;

    using Gorba.Common.Utility.Win32.Api.DLLs;
    using Gorba.Common.Utility.Win32.Api.Enums;

    /// <summary>
    /// Wrapper around different <see cref="DwmApi"/> methods.
    /// </summary>
    public static partial class DesktopWindowManager
    {
        /// <summary>
        /// Sets the non-client rendering policy.
        /// </summary>
        /// <param name="hwnd">
        /// The handle to the window that will receive the attributes.
        /// </param>
        /// <param name="policy">
        /// The non-client area rendering policy.
        /// </param>
        /// <returns>
        /// If this function succeeds, it returns S_OK. Otherwise, it returns an HRESULT error code.
        /// </returns>
        public static int SetNonClientRenderingPolicy(IntPtr hwnd, DwmncRenderingPolicy policy)
        {
            var val = (int)policy;
            return DwmApi.DwmSetWindowAttribute(hwnd, DwmWindowAttribute.NcrenderingPolicy, ref val, sizeof(int));
        }
    }
}
