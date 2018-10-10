// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TimeZoneSettingsHandler.FX20.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the TimeZoneSettingsHandler type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.HardwareManager.Core.Settings
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.InteropServices;

    using Gorba.Common.Configuration.HardwareManager;

    using Microsoft.Win32;

    using NLog;

    /// <summary>
    /// Handler that sets the system time zone to the configured value.
    /// </summary>
    public partial class TimeZoneSettingsHandler : ISettingsPartHandler
    {
        private const string TimeZonesRegistry = @"SOFTWARE\Microsoft\Windows NT\CurrentVersion\Time Zones\";

        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Apply the given settings.
        /// </summary>
        /// <param name="setting">
        /// The setting object.
        /// </param>
        /// <returns>
        /// True if the system drive should be committed and the system rebooted.
        /// </returns>
        public bool ApplySetting(HardwareManagerSetting setting)
        {
            var timeZone = setting.TimeZone;
            if (string.IsNullOrEmpty(timeZone))
            {
                Logger.Info("Setting of the time zone is disabled");
                return false;
            }

            using (var rootKey = Registry.LocalMachine.OpenSubKey(TimeZonesRegistry))
            {
                if (rootKey == null)
                {
                    throw new KeyNotFoundException("Couldn't find " + TimeZonesRegistry);
                }

                var zoneKey = rootKey.OpenSubKey(timeZone);
                if (zoneKey == null)
                {
                    Logger.Warn("Couldn't find time zone '{0}', the following zones are available:", timeZone);
                    foreach (var subKeyName in rootKey.GetSubKeyNames())
                    {
                        Logger.Warn(" - '{0}'", subKeyName);
                    }

                    return false;
                }

                using (zoneKey)
                {
                    if (!SetTimeZone(zoneKey))
                    {
                        return false;
                    }
                }

                Logger.Info("Time zone successfully set to: {0}", timeZone);
                return true;
            }
        }

        private static bool SetTimeZone(RegistryKey zoneKey)
        {
            var daylightName = (string)zoneKey.GetValue("Dlt");
            var standardName = (string)zoneKey.GetValue("Std");

            var currentTimeZone = TimeZone.CurrentTimeZone;
            if (currentTimeZone != null
                && (currentTimeZone.StandardName.Equals(standardName, StringComparison.InvariantCultureIgnoreCase)
                    || currentTimeZone.DaylightName.Equals(daylightName, StringComparison.InvariantCultureIgnoreCase)))
            {
                Logger.Info("Time zone is already set to '{0}' ({1})", standardName, zoneKey.GetValue("Display"));
                return false;
            }

            Logger.Info("Setting the time zone to (display name): {0}", zoneKey.GetValue("Display"));

            var tzi = (byte[])zoneKey.GetValue("TZI");
            var regTzi = new NativeMethods.RegistryTimeZoneInformation(tzi);

            var tz = new NativeMethods.TimeZoneInformation();
            tz.Bias = regTzi.Bias;
            tz.DaylightBias = regTzi.DaylightBias;
            tz.StandardBias = regTzi.StandardBias;
            tz.DaylightDate = regTzi.DaylightDate;
            tz.StandardDate = regTzi.StandardDate;
            tz.DaylightName = daylightName;
            tz.StandardName = standardName;

            NativeMethods.TokenPrivilegesAccess.EnablePrivilege("SeTimeZonePrivilege");
            bool didSet = NativeMethods.SetTimeZoneInformation(ref tz);
            int lastError = Marshal.GetLastWin32Error();
            NativeMethods.TokenPrivilegesAccess.DisablePrivilege("SeTimeZonePrivilege");

            var key = Registry.LocalMachine.OpenSubKey("SYSTEM\\CurrentControlSet\\Control\\TimeZoneInformation", true);
            if (key != null)
            {
                key.SetValue("TimeZoneKeyName", key.GetValue("StandardName"));
            }

            if (didSet)
            {
                return true;
            }

            Marshal.ThrowExceptionForHR(lastError);
            return false;
        }

        private static class NativeMethods
        {
            // ReSharper disable FieldCanBeMadeReadOnly.Local
            // ReSharper disable MemberCanBePrivate.Local
#pragma warning disable 169

            [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
            public static extern bool SetTimeZoneInformation([In] ref TimeZoneInformation lpTimeZoneInformation);

            [StructLayoutAttribute(LayoutKind.Sequential)]
            public struct SystemTime
            {
                [MarshalAs(UnmanagedType.U2)]
                public short Year;
                [MarshalAs(UnmanagedType.U2)]
                public short Month;
                [MarshalAs(UnmanagedType.U2)]
                public short DayOfWeek;
                [MarshalAs(UnmanagedType.U2)]
                public short Day;
                [MarshalAs(UnmanagedType.U2)]
                public short Hour;
                [MarshalAs(UnmanagedType.U2)]
                public short Minute;
                [MarshalAs(UnmanagedType.U2)]
                public short Second;
                [MarshalAs(UnmanagedType.U2)]
                public short Milliseconds;
            }

            [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
            public struct TimeZoneInformation
            {
                [MarshalAs(UnmanagedType.I4)]
                public int Bias;
                [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 0x20)]
                public string StandardName;
                public SystemTime StandardDate;
                [MarshalAs(UnmanagedType.I4)]
                public int StandardBias;
                [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 0x20)]
                public string DaylightName;
                public SystemTime DaylightDate;
                [MarshalAs(UnmanagedType.I4)]
                public int DaylightBias;
            }

            [StructLayout(LayoutKind.Sequential)]
            public struct RegistryTimeZoneInformation
            {
                [MarshalAs(UnmanagedType.I4)]
                public int Bias;
                [MarshalAs(UnmanagedType.I4)]
                public int StandardBias;
                [MarshalAs(UnmanagedType.I4)]
                public int DaylightBias;
                public SystemTime StandardDate;
                public SystemTime DaylightDate;

                /*public RegistryTimeZoneInformation(TimeZoneInformation tzi)
                {
                    this.Bias = tzi.Bias;
                    this.StandardDate = tzi.StandardDate;
                    this.StandardBias = tzi.StandardBias;
                    this.DaylightDate = tzi.DaylightDate;
                    this.DaylightBias = tzi.DaylightBias;
                }*/

                public RegistryTimeZoneInformation(byte[] bytes)
                {
                    if ((bytes == null) || (bytes.Length != 0x2c))
                    {
                        throw new ArgumentException("Invalid REG_TZI_FORMAT");
                    }

                    this.Bias = BitConverter.ToInt32(bytes, 0);
                    this.StandardBias = BitConverter.ToInt32(bytes, 4);
                    this.DaylightBias = BitConverter.ToInt32(bytes, 8);
                    this.StandardDate.Year = BitConverter.ToInt16(bytes, 12);
                    this.StandardDate.Month = BitConverter.ToInt16(bytes, 14);
                    this.StandardDate.DayOfWeek = BitConverter.ToInt16(bytes, 0x10);
                    this.StandardDate.Day = BitConverter.ToInt16(bytes, 0x12);
                    this.StandardDate.Hour = BitConverter.ToInt16(bytes, 20);
                    this.StandardDate.Minute = BitConverter.ToInt16(bytes, 0x16);
                    this.StandardDate.Second = BitConverter.ToInt16(bytes, 0x18);
                    this.StandardDate.Milliseconds = BitConverter.ToInt16(bytes, 0x1a);
                    this.DaylightDate.Year = BitConverter.ToInt16(bytes, 0x1c);
                    this.DaylightDate.Month = BitConverter.ToInt16(bytes, 30);
                    this.DaylightDate.DayOfWeek = BitConverter.ToInt16(bytes, 0x20);
                    this.DaylightDate.Day = BitConverter.ToInt16(bytes, 0x22);
                    this.DaylightDate.Hour = BitConverter.ToInt16(bytes, 0x24);
                    this.DaylightDate.Minute = BitConverter.ToInt16(bytes, 0x26);
                    this.DaylightDate.Second = BitConverter.ToInt16(bytes, 40);
                    this.DaylightDate.Milliseconds = BitConverter.ToInt16(bytes, 0x2a);
                }
            }

            [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
            private struct Luid
            {
                internal uint LowPart;
                internal uint HighPart;
            }

            [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
            private struct LuidAndAttributes
            {
                internal Luid Luid;
                internal uint Attributes;
            }

            [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
            private struct TokenPrivilege
            {
                internal uint PrivilegeCount;
                internal LuidAndAttributes Privilege;
            }

            public static class TokenPrivilegesAccess
            {
                public const int TokenAssignPrimary = 0x00000001;
                public const int TokenDuplicate = 0x00000002;
                public const int TokenImpersonate = 0x00000004;
                public const int TokenQuery = 0x00000008;
                public const int TokenQuerySource = 0x00000010;
                public const int TokenAdjustPrivileges = 0x00000020;
                public const int TokenAdjustGroups = 0x00000040;
                public const int TokenAdjustDefault = 0x00000080;

                public const uint SePrivilegeEnabledByDefault = 0x00000001;
                public const uint SePrivilegeEnabled = 0x00000002;
                public const uint SePrivilegeRemoved = 0x00000004;
                public const uint SePrivilegeUsedForAccess = 0x80000000;

                public static bool EnablePrivilege(string privilege)
                {
                    try
                    {
                        int token = 0;

                        var tp = new TokenPrivilege();
                        var luid = new Luid();

                        OpenProcessToken(GetCurrentProcess(), TokenAdjustPrivileges | TokenQuery, ref token);
                        LookupPrivilegeValue(null, privilege, ref luid);
                        tp.PrivilegeCount = 1;

                        var luidAndAtt = new LuidAndAttributes();
                        luidAndAtt.Attributes = SePrivilegeEnabled;
                        luidAndAtt.Luid = luid;
                        tp.Privilege = luidAndAtt;

                        AdjustTokenPrivileges(token, 0, ref tp, 1024, 0, 0);
                        return true;
                    }
                    catch
                    {
                        return false;
                    }
                }

                public static bool DisablePrivilege(string privilege)
                {
                    try
                    {
                        int token = 0;

                        var tp = new TokenPrivilege();
                        var luid = new Luid();

                        OpenProcessToken(GetCurrentProcess(), TokenAdjustPrivileges | TokenQuery, ref token);
                        LookupPrivilegeValue(null, privilege, ref luid);
                        tp.PrivilegeCount = 1;

                        // TP.Attributes should be none (not set) to disable privilege
                        var luidAndAtt = new LuidAndAttributes();
                        luidAndAtt.Luid = luid;
                        tp.Privilege = luidAndAtt;

                        AdjustTokenPrivileges(token, 0, ref tp, 1024, 0, 0);
                        return true;
                    }
                    catch
                    {
                        return false;
                    }
                }

                [DllImport("advapi32.dll", CharSet = CharSet.Auto)]
                private static extern int OpenProcessToken(int processHandle, int desiredAccess, ref int tokenhandle);

                [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
                private static extern int GetCurrentProcess();

                [DllImport("advapi32.dll", CharSet = CharSet.Auto)]
                private static extern int LookupPrivilegeValue(
                    string lpsystemname, string lpname, [MarshalAs(UnmanagedType.Struct)] ref Luid lpLuid);

                [DllImport("advapi32.dll", CharSet = CharSet.Auto)]
                private static extern int AdjustTokenPrivileges(
                    int tokenhandle,
                    int disableprivs,
                    [MarshalAs(UnmanagedType.Struct)] ref TokenPrivilege newState,
                    int bufferlength,
                    int preivousState,
                    int returnLength);
            }

            // ReSharper restore MemberCanBePrivate.Local
            // ReSharper restore FieldCanBeMadeReadOnly.Local
#pragma warning restore 169
        }
    }
}
