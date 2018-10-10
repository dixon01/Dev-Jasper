// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DwmWindowAttribute.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the DwmWindowAttribute type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Utility.Win32.Api.Enums
{
    /// <summary>
    /// Flags used by the <c>DwmGetWindowAttribute</c> and <c>DwmSetWindowAttribute</c> functions
    /// to specify window attributes for non-client rendering.
    /// </summary>
    public enum DwmWindowAttribute
    {
        /// <summary>
        /// Use with <c>DwmGetWindowAttribute</c>. Discovers whether non-client rendering is enabled.
        /// The retrieved value is of type BOOL. TRUE if non-client rendering is enabled; otherwise, FALSE.
        /// </summary>
        NcrenderingEnabled = 1,

        /// <summary>
        /// Use with <c>DwmSetWindowAttribute</c>. Sets the non-client rendering policy.
        /// The attribute parameter points to a value from the DWMNCRENDERINGPOLICY enumeration.
        /// </summary>
        NcrenderingPolicy,

        /// <summary>
        /// Use with <c>DwmSetWindowAttribute</c>. Enables or forcibly disables DWM transitions.
        /// The attribute parameter points to a value of TRUE to disable transitions or FALSE to enable transitions.
        /// </summary>
        TransitionsForcedisabled,

        /// <summary>
        /// Use with <c>DwmSetWindowAttribute</c>. Enables content rendered in the non-client area to be
        /// visible on the frame drawn by DWM. The attribute parameter points to a value of TRUE
        /// to enable content rendered in the non-client area to be visible on the frame; otherwise, it points to FALSE.
        /// </summary>
        AllowNcpaint,

        /// <summary>
        /// Use with <c>DwmGetWindowAttribute</c>.
        /// Retrieves the bounds of the caption button area in the window-relative space.
        /// The retrieved value is of type RECT.
        /// </summary>
        CaptionButtonBounds,

        /// <summary>
        /// Use with <c>DwmSetWindowAttribute</c>. Specifies whether non-client content is right-to-left (RTL) mirrored.
        /// The attribute parameter points to a value of TRUE if the non-client
        /// content is right-to-left (RTL) mirrored; otherwise, it points to FALSE.
        /// </summary>
        NonclientRTLLayout,

        /// <summary>
        /// Use with <c>DwmSetWindowAttribute</c>.
        /// Forces the window to display an iconic thumbnail or peek representation (a static bitmap),
        /// even if a live or snapshot representation of the window is available.
        /// This value normally is set during a window's creation and not changed throughout the window's lifetime.
        /// Some scenarios, however, might require the value to change over time.
        /// The attribute parameter points to a value of TRUE to require a iconic thumbnail or peek representation;
        /// otherwise, it points to FALSE.
        /// </summary>
        ForceIconicRepresentation,

        /// <summary>
        /// Use with <c>DwmSetWindowAttribute</c>.
        /// Sets how Flip3D treats the window.
        /// The attribute parameter points to a value from the DWMFLIP3DWINDOWPOLICY enumeration.
        /// </summary>
        Flip3DPolicy,

        /// <summary>
        /// Use with <c>DwmGetWindowAttribute</c>.
        /// Retrieves the extended frame bounds rectangle in screen space.
        /// The retrieved value is of type RECT.
        /// </summary>
        ExtendedFrameBounds,

        /// <summary>
        /// Use with <c>DwmSetWindowAttribute</c>. The window will provide a bitmap for use by DWM as
        /// an iconic thumbnail or peek representation (a static bitmap) for the window.
        /// DWMWA_HAS_ICONIC_BITMAP can be specified with DWMWA_FORCE_ICONIC_REPRESENTATION.
        /// DWMWA_HAS_ICONIC_BITMAP normally is set during a window's creation and
        /// not changed throughout the window's lifetime.
        /// Some scenarios, however, might require the value to change over time.
        /// The attribute parameter points to a value of TRUE to inform DWM that the window will
        /// provide an iconic thumbnail or peek representation; otherwise, it points to FALSE.
        /// </summary>
        HasIconicBitmap,

        /// <summary>
        /// Use with <c>DwmGetWindowAttribute</c>. Do not show peek preview for the window.
        /// The peek view shows a full-sized preview of the window when the mouse hovers over
        /// the window's thumbnail in the taskbar. If this attribute is set, hovering the mouse pointer
        /// over the window's thumbnail dismisses peek (in case another window in the group has a peek preview showing).
        /// The attribute parameter points to a value of TRUE to prevent peek functionality or FALSE to allow it.
        /// </summary>
        DisallowPeek,

        /// <summary>
        /// Use with <c>DwmSetWindowAttribute</c>. Prevents a window from fading to a glass sheet when peek is invoked.
        /// The attribute parameter points to a value of TRUE to prevent the window from fading
        /// during another window's peek or FALSE for normal behavior.
        /// </summary>
        ExcludedFromPeek,

        /// <summary>
        /// Use with <c>DwmGetWindowAttribute</c>. Cloaks the window such that it is not visible to the user.
        /// The window is still composed by DWM.
        /// </summary>
        Cloak,

        /// <summary>
        /// Use with <c>DwmGetWindowAttribute</c>. If the window is cloaked, provides a value explaining why.
        /// </summary>
        Cloaked,

        /// <summary>
        /// Use with <c>DwmSetWindowAttribute</c>. Freeze the window's thumbnail image with its current visuals.
        /// Do no further live updates on the thumbnail image to match the window's contents.
        /// </summary>
        FreezeRepresentation,

        /// <summary>
        /// The maximum recognized DWMWINDOWATTRIBUTE value, used for validation purposes.
        /// </summary>
        Last
    }
}