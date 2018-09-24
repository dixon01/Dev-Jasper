// --------------------------------------------------------------------------------------------------------------------
// <copyright file="User32.FX20.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Utility.Win32.Api.DLLs
{
    using System;
    using System.Runtime.InteropServices;
    using System.Text;

    using Gorba.Common.Utility.Win32.Api.Enums;
    using Gorba.Common.Utility.Win32.Api.Structs;

    /// <summary>
    /// /// Wrapper for the <c>user32.dll</c>.
    /// </summary>
    public static partial class User32
    {
        /// <summary>
        /// Retrieves a handle to the foreground window (the window with which the user is currently working).
        /// The system assigns a slightly higher priority to the thread that creates
        /// the foreground window than it does to other threads.
        /// </summary>
        /// <returns>
        /// The return value is a handle to the foreground window.
        /// The foreground window can be <see cref="IntPtr.Zero"/> in certain circumstances,
        /// such as when a window is losing activation.
        /// </returns>
        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr GetForegroundWindow();

        /// <summary>
        /// Sets the specified window's show state.
        /// </summary>
        /// <param name="handle">
        /// A handle to the window.
        /// </param>
        /// <param name="command">
        /// Controls how the window is to be shown.
        /// </param>
        /// <returns>
        /// If the window was previously visible, the return value is true.
        /// If the window was previously hidden, the return value is false.
        /// </returns>
        [DllImport("user32.dll")]
        public static extern bool ShowWindow(IntPtr handle, ShowWindow command);

        /// <summary>
        /// Brings the thread that created the specified window into the foreground and
        /// activates the window. Keyboard input is directed to the window,
        /// and various visual cues are changed for the user.
        /// The system assigns a slightly higher priority to the thread that
        /// created the foreground window than it does to other threads.
        /// </summary>
        /// <param name="handle">
        /// A handle to the window that should be activated and brought to the foreground.
        /// </param>
        /// <returns>
        /// If the window was brought to the foreground, the return value is true.
        /// If the window was not brought to the foreground, the return value is false.
        /// </returns>
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool SetForegroundWindow(IntPtr handle);

        /// <summary>
        /// Destroys an icon and frees any memory the icon occupied.
        /// </summary>
        /// <param name="iconHandle">
        /// A handle to the icon to be destroyed. The icon must not be in use.
        /// </param>
        /// <returns>
        /// If the function succeeds, the return value is nonzero.
        /// If the function fails, the return value is zero.
        /// To get extended error information, call <see cref="Marshal.GetLastWin32Error"/>.
        /// </returns>
        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool DestroyIcon(IntPtr iconHandle);

        /// <summary>
        /// Registers a hotkey with a specific modifier and id
        /// </summary>
        /// <param name="handle">
        /// The handle to the form.
        /// </param>
        /// <param name="id">
        /// The unique id for the hotkey.
        /// </param>
        /// <param name="modifiers">
        /// A modifier is a key used in conjunction with the hotkey like the Windows key etc.
        /// The registered key is Win + hotkey if the modifier is the Windows key.
        /// </param>
        /// <param name="key">
        /// The hotkey.
        /// </param>
        /// <returns>
        /// Returns true if the hotkey was registered successfully otherwise false<see cref="bool"/>.
        /// </returns>
        [DllImport("user32.dll")]
        public static extern bool RegisterHotKey(IntPtr handle, int id, KeyModifier modifiers, uint key);

        /// <summary>
        /// Deregisters the hotkey based on the unique id previously used to register the hotkey.
        /// </summary>
        /// <param name="handle">
        /// The handle to the form.
        /// </param>
        /// <param name="id">
        /// The unique id for the hotkey.
        /// </param>
        /// <returns>
        /// Returns true of the hotkey was deregistered successfully otherwise false <see cref="bool"/>.
        /// </returns>
        [DllImport("user32.dll")]
        public static extern bool UnregisterHotKey(IntPtr handle, int id);

        /// <summary>
        /// Gets information about the display devices in the current session
        /// </summary>
        /// <param name="ipDevice">
        /// Pointer to the device name.
        /// </param>
        /// <param name="deviceIndex">
        /// The index value that specifies the display device of interest.
        /// </param>
        /// <param name="pointerDisplayDevice">
        /// The pointer to a DisplayDevice structure which receives information about the display device specified
        /// by deviceIndex.
        /// </param>
        /// <param name="flags">
        /// Set this flag to EDD_GET_DEVICE_INTERFACE_NAME (0x00000001) to retrieve the device interface name for
        /// GUID_DEVINTERFACE_MONITOR, which is registered by the operating system on a per monitor basis.
        /// The value is placed in the DeviceID member of the DisplayDevice structure returned in pointerDisplayDevice.
        /// The resulting device interface name can be used with SetupAPI functions and serves as a link between
        /// GDI monitor devices and SetupAPI monitor devices.
        /// </param>
        /// <returns>
        /// Returns true if a display device is found else false. <see cref="bool"/>.
        /// </returns>
        [DllImport("User32.dll")]
        public static extern bool EnumDisplayDevices(
            IntPtr ipDevice,
            int deviceIndex,
            ref DisplayDevice pointerDisplayDevice,
            int flags);

        /// <summary>
        /// Gets the graphics mode information for a given display name.
        /// </summary>
        /// <param name="deviceName">
        /// The name of the display device.
        /// </param>
        /// <param name="modeNum">
        /// Number indicating if it is the current settings of the device.
        /// </param>
        /// <param name="devMode">
        /// The graphics mode information of the display device.
        /// </param>
        /// <returns>
        /// Returns a non-zero number if it is successful <see cref="int"/>.
        /// </returns>
        [DllImport("user32.dll")]
        public static extern int EnumDisplaySettings(string deviceName, int modeNum, ref DevMode devMode);

        /// <summary>
        /// Changes the settings of the default display device to the specified graphics mode.
        /// </summary>
        /// <param name="devMode">
        /// The graphics mode of the display device.
        /// </param>
        /// <param name="flags">
        /// Indicates how the graphics mode should be changed.
        /// </param>
        /// <returns>
        /// Returns one of the following values
        /// 0 = The settings change was successful.
        /// 1 = The computer must be restarted for the graphics mode to work.
        /// -1 = The display driver failed the specified graphics mode.
        /// -3 = Unable to write settings to the registry.
        /// <see cref="int"/>.
        /// </returns>
        [DllImport("user32.dll")]
        public static extern int ChangeDisplaySettings(ref DevMode devMode, int flags);

        /// <summary>
        /// Changes the settings of the specified display device to the specified graphics mode.
        /// </summary>
        /// <param name="deviceName">
        /// A pointer to a null-terminated string that specifies the display device whose graphics mode will change.
        /// Only display device names as returned by EnumDisplayDevices are valid.
        /// </param>
        /// <param name="devMode">
        /// A pointer to a DEVMODE structure that describes the new graphics mode.
        /// If <paramref name="devMode"/> is NULL, all the values
        /// currently in the registry will be used for the display setting.
        /// </param>
        /// <param name="hwnd">
        /// Reserved; must be NULL.
        /// </param>
        /// <param name="flags">
        /// Indicates how the graphics mode should be changed.
        /// </param>
        /// <param name="param">
        /// Must be NULL.
        /// </param>
        /// <returns>
        /// Returns one of the values in DisplaySettingResults
        /// <see cref="DisplaySettingResults"/>.
        /// </returns>
        [DllImport("user32.dll")]
        public static extern DisplaySettingResults ChangeDisplaySettingsEx(
            string deviceName, ref DevMode devMode, IntPtr hwnd, uint flags, IntPtr param);

        /// <summary>
        /// Modifies the display topology, source, and target modes by exclusively enabling the specified paths in the
        /// current session.
        /// </summary>
        /// <param name="numPathArrayElements">
        /// Number of elements in pathArray.
        /// </param>
        /// <param name="pathArray">
        /// Array of all display paths that are to be set. Only the paths within this array that have the
        /// DISPLAYCONFIG_PATH_ACTIVE flag set in the flags member of DISPLAYCONFIG_PATH_INFO are set. This parameter
        /// can be NULL. The order in which active paths appear in this array determines the path priority.
        /// </param>
        /// <param name="numModeArrayElements">
        /// Number of elements in modeInfoArray.
        /// </param>
        /// <param name="modeArray">
        /// Array of display source and target mode information (DISPLAYCONFIG_MODE_INFO) that is referenced by the
        /// modeInfoId member of DISPLAYCONFIG_PATH_SOURCE_INFO and DISPLAYCONFIG_PATH_TARGET_INFO element of path
        /// information from pathArray. This parameter can be NULL.
        /// </param>
        /// <param name="flags">
        /// A bitwise OR of flag values that indicates the behavior of this function. This parameter can be one the
        /// values in SetDisplayConfigFlags, or a combination of the following values; 0 is not valid.
        /// </param>
        /// <returns>
        /// The function returns a code. <see cref="long"/>.
        /// </returns>
        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        public static extern long SetDisplayConfig(
            uint numPathArrayElements,
            IntPtr pathArray,
            uint numModeArrayElements,
            IntPtr modeArray,
            uint flags);

        /// <summary>
        /// Changes the settings of the default display device to the specified graphics mode.
        /// </summary>
        /// <param name="intPtr">
        /// Pointer to the graphics mode of the display device.
        /// </param>
        /// <param name="flags">
        /// Indicates how the graphics mode should be changed.
        /// </param>
        /// <returns>
        /// Returns one of the values in DisplaySettingResults <see cref="int"/>.
        /// </returns>
        [DllImport("user32.dll")]
        public static extern int ChangeDisplaySettings(IntPtr intPtr, int flags);

        /// <summary>
        /// Translates the specified virtual-key code and keyboard state to the
        /// corresponding Unicode character or characters.
        /// </summary>
        /// <param name="virtKey">
        /// The virtual-key code to be translated.
        /// </param>
        /// <param name="scanCode">
        /// The hardware scan code of the key to be translated.
        /// The high-order bit of this value is set if the key is up.
        /// </param>
        /// <param name="keyState">
        /// A pointer to a 256-byte array that contains the current keyboard state.
        /// Each element (byte) in the array contains the state of one key.
        /// If the high-order bit of a byte is set, the key is down.
        /// </param>
        /// <param name="buffer">
        /// The buffer that receives the translated Unicode character or characters.
        /// However, this buffer may be returned without being null-terminated
        /// even though the variable name suggests that it is null-terminated.
        /// </param>
        /// <param name="bufferSize">
        /// The size, in characters, of the buffer pointed to by the <paramref name="buffer"/> parameter.
        /// </param>
        /// <param name="flags">
        /// The behavior of the function.
        /// </param>
        /// <returns>
        /// <see cref="https://msdn.microsoft.com/en-us/library/windows/desktop/ms646320%28v=vs.85%29.aspx"/>
        /// for details.
        /// </returns>
        [DllImport("user32.dll")]
        public static extern int ToUnicode(
            uint virtKey,
            uint scanCode,
            byte[] keyState,
            [Out, MarshalAs(UnmanagedType.LPWStr, SizeParamIndex = 4)]
            StringBuilder buffer,
            int bufferSize,
            uint flags);

        /// <summary>
        /// Copies the status of the 256 virtual keys to the specified buffer.
        /// </summary>
        /// <param name="keyState">
        /// The 256-byte array that receives the status data for each virtual key.
        /// </param>
        /// <returns>
        /// If the function succeeds, the return value is nonzero.
        /// If the function fails, the return value is zero.
        /// </returns>
        [DllImport("user32.dll")]
        public static extern bool GetKeyboardState(byte[] keyState);

        /// <summary>
        /// Translates (maps) a virtual-key code into a scan code or character value,
        /// or translates a scan code into a virtual-key code.
        /// </summary>
        /// <param name="code">
        /// The virtual key code or scan code for a key. How this value is interpreted
        /// depends on the value of the <paramref name="mapType"/> parameter.
        /// </param>
        /// <param name="mapType">
        /// The translation to be performed.
        /// The value of this parameter depends on the value of the <paramref name="code"/> parameter.
        /// </param>
        /// <returns>
        /// The return value is either a scan code, a virtual-key code, or a character value,
        /// depending on the value of <paramref name="code"/> and <paramref name="mapType"/>.
        /// If there is no translation, the return value is zero.
        /// </returns>
        [DllImport("user32.dll")]
        public static extern uint MapVirtualKey(uint code, MapVirtualKeyType mapType);

        /// <summary>
        /// Places the given window in the system-maintained clipboard format listener list.
        /// </summary>
        /// <param name="handle">
        /// A handle to the window to be placed in the clipboard format listener list.
        /// </param>
        /// <returns>
        /// Returns TRUE if successful, FALSE otherwise.
        /// </returns>
        [DllImport("user32.dll")]
        public static extern bool AddClipboardFormatListener(IntPtr handle);

        /// <summary>
        /// Removes the given window from the system-maintained clipboard format listener list.
        /// </summary>
        /// <param name="handle">
        /// A handle to the window to remove from the clipboard format listener list.
        /// </param>
        /// <returns>
        /// Returns TRUE if successful, FALSE otherwise.
        /// </returns>
        [DllImport("user32.dll")]
        public static extern bool RemoveClipboardFormatListener(IntPtr handle);
    }
}
