namespace TimeZoneSetter
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.InteropServices;
    using System.Windows.Forms;

    using Microsoft.Win32;

    public partial class Form1 : Form
    {
        private const string TimeZonesRegistry = @"SOFTWARE\Microsoft\Windows NT\CurrentVersion\Time Zones";
        public Form1()
        {
            this.InitializeComponent();

            using (var regTimeZones = Registry.LocalMachine.OpenSubKey(TimeZonesRegistry))
            {
                if (regTimeZones == null)
                {
                    return;
                }

                foreach (var subKeyName in regTimeZones.GetSubKeyNames())
                {
                    this.listBoxTimeZones.Items.Add(subKeyName);
                }
            }
        }

        private void ListBoxTimeZonesSelectedIndexChanged(object sender, EventArgs e)
        {
            this.buttonChangeTimeZone.Enabled = this.listBoxTimeZones.SelectedItem != null;
        }

        private void ButtonChangeTimeZoneClick(object sender, EventArgs e)
        {
            var timeZone = (string)this.listBoxTimeZones.SelectedItem;
            if (MessageBox.Show(
                this,
                string.Format("Do you really want to change the time zone to '{0}'?", timeZone),
                "Time Zone",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Exclamation,
                MessageBoxDefaultButton.Button2) != DialogResult.Yes)
            {
                return;
            }

            try
            {
                this.SetTimeZone(timeZone);
                MessageBox.Show(
                    this,
                    string.Format("Time zone was successfully set to '{0}'.", timeZone),
                    "Time Zone",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.ToString(), ex.GetType().Name, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void SetTimeZone(string timeZone)
        {
            var regTimeZones = Registry.LocalMachine.OpenSubKey(TimeZonesRegistry);
            if (regTimeZones == null)
            {
                throw new KeyNotFoundException("Couldn't find " + TimeZonesRegistry);
            }

            var subKey = regTimeZones.OpenSubKey(timeZone);
            if (subKey == null)
            {
                throw new KeyNotFoundException(string.Format("Couldn't find {0}\\{1}", TimeZonesRegistry, timeZone));
            }

            string daylightName = (string)subKey.GetValue("Dlt");
            string standardName = (string)subKey.GetValue("Std");
            byte[] tzi = (byte[])subKey.GetValue("TZI");

            var regTzi = new RegistryTimeZoneInformation(tzi);

            var tz = new TimeZoneInformation();
            tz.Bias = regTzi.Bias;
            tz.DaylightBias = regTzi.DaylightBias;
            tz.StandardBias = regTzi.StandardBias;
            tz.DaylightDate = regTzi.DaylightDate;
            tz.StandardDate = regTzi.StandardDate;
            tz.DaylightName = daylightName;
            tz.StandardName = standardName;

            TokenPrivilegesAccess.EnablePrivilege("SeTimeZonePrivilege");
            bool didSet = SetTimeZoneInformation(ref tz);
            int lastError = Marshal.GetLastWin32Error();
            TokenPrivilegesAccess.DisablePrivilege("SeTimeZonePrivilege");

            var key = Registry.LocalMachine.OpenSubKey("SYSTEM\\CurrentControlSet\\Control\\TimeZoneInformation", true);
            key.SetValue("TimeZoneKeyName", key.GetValue("StandardName"));

            if (didSet)
            {
                return;
            }

            Marshal.ThrowExceptionForHR(lastError);
        }

        public const int ERROR_ACCESS_DENIED = 0x005;
        public const int CORSEC_E_MISSING_STRONGNAME = -2146233317;

        [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
        private static extern bool SetTimeZoneInformation([In] ref TimeZoneInformation lpTimeZoneInformation);

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

            public RegistryTimeZoneInformation(TimeZoneInformation tzi)
            {
                this.Bias = tzi.Bias;
                this.StandardDate = tzi.StandardDate;
                this.StandardBias = tzi.StandardBias;
                this.DaylightDate = tzi.DaylightDate;
                this.DaylightBias = tzi.DaylightBias;
            }

            public RegistryTimeZoneInformation(byte[] bytes)
            {
                if ((bytes == null) || (bytes.Length != 0x2c))
                {
                    throw new ArgumentException("Argument_InvalidREG_TZI_FORMAT");
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

        public class TokenPrivilegesAccess
        {
            [DllImport("advapi32.dll", CharSet = CharSet.Auto)]
            public static extern int OpenProcessToken(int ProcessHandle, int DesiredAccess,
            ref int tokenhandle);

            [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
            public static extern int GetCurrentProcess();

            [DllImport("advapi32.dll", CharSet = CharSet.Auto)]
            public static extern int LookupPrivilegeValue(string lpsystemname, string lpname,
            [MarshalAs(UnmanagedType.Struct)] ref LUID lpLuid);

            [DllImport("advapi32.dll", CharSet = CharSet.Auto)]
            public static extern int AdjustTokenPrivileges(int tokenhandle, int disableprivs,
                [MarshalAs(UnmanagedType.Struct)]ref TOKEN_PRIVILEGE Newstate, int bufferlength,
                int PreivousState, int Returnlength);

            public const int TOKEN_ASSIGN_PRIMARY = 0x00000001;
            public const int TOKEN_DUPLICATE = 0x00000002;
            public const int TOKEN_IMPERSONATE = 0x00000004;
            public const int TOKEN_QUERY = 0x00000008;
            public const int TOKEN_QUERY_SOURCE = 0x00000010;
            public const int TOKEN_ADJUST_PRIVILEGES = 0x00000020;
            public const int TOKEN_ADJUST_GROUPS = 0x00000040;
            public const int TOKEN_ADJUST_DEFAULT = 0x00000080;

            public const UInt32 SE_PRIVILEGE_ENABLED_BY_DEFAULT = 0x00000001;
            public const UInt32 SE_PRIVILEGE_ENABLED = 0x00000002;
            public const UInt32 SE_PRIVILEGE_REMOVED = 0x00000004;
            public const UInt32 SE_PRIVILEGE_USED_FOR_ACCESS = 0x80000000;

            public static bool EnablePrivilege(string privilege)
            {
                try
                {
                    int token = 0;
                    int retVal = 0;

                    TOKEN_PRIVILEGE TP = new TOKEN_PRIVILEGE();
                    LUID LD = new LUID();

                    retVal = OpenProcessToken(GetCurrentProcess(), TOKEN_ADJUST_PRIVILEGES | TOKEN_QUERY, ref token);
                    retVal = LookupPrivilegeValue(null, privilege, ref LD);
                    TP.PrivilegeCount = 1;

                    var luidAndAtt = new LUID_AND_ATTRIBUTES();
                    luidAndAtt.Attributes = SE_PRIVILEGE_ENABLED;
                    luidAndAtt.Luid = LD;
                    TP.Privilege = luidAndAtt;

                    retVal = AdjustTokenPrivileges(token, 0, ref TP, 1024, 0, 0);
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
                    int retVal = 0;

                    TOKEN_PRIVILEGE TP = new TOKEN_PRIVILEGE();
                    LUID LD = new LUID();

                    retVal = OpenProcessToken(GetCurrentProcess(), TOKEN_ADJUST_PRIVILEGES | TOKEN_QUERY, ref token);
                    retVal = LookupPrivilegeValue(null, privilege, ref LD);
                    TP.PrivilegeCount = 1;
                    // TP.Attributes should be none (not set) to disable privilege
                    var luidAndAtt = new LUID_AND_ATTRIBUTES();
                    luidAndAtt.Luid = LD;
                    TP.Privilege = luidAndAtt;

                    retVal = AdjustTokenPrivileges(token, 0, ref TP, 1024, 0, 0);
                    return true;
                }
                catch
                {
                    return false;
                }
            }

        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        public struct LUID
        {
            internal uint LowPart;
            internal uint HighPart;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        public struct LUID_AND_ATTRIBUTES
        {
            internal LUID Luid;
            internal uint Attributes;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        public struct TOKEN_PRIVILEGE
        {
            internal uint PrivilegeCount;
            internal LUID_AND_ATTRIBUTES Privilege;
        }

    }
}
