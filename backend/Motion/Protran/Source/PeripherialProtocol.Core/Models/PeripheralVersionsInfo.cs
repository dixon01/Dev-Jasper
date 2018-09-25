// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Luminator Technology Group" file="PeripheralAudioVersionsInfo.cs">
//   Copyright © 2011-2015 LTG. All rights reserved.
// </copyright>
// <author>Kevin Hartman</author>
// --------------------------------------------------------------------------------------------------------------------
namespace Luminator.PeripheralProtocol.Core.Models
{
    using System;
    using System.Linq;
    using System.Runtime.InteropServices;
    using System.Runtime.Serialization;
    using System.Text;
    using System.Xml.Serialization;

    using Luminator.PeripheralProtocol.Core.Interfaces;

    /// <summary>The peripheral versions info.</summary>
    [Serializable]
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1, Size = Size)]
    public class PeripheralVersionsInfo : IPeripheralVersionsInfo
    {
        /// <summary>Initializes a new instance of the <see cref="PeripheralVersionsInfo"/> class.</summary>
        public PeripheralVersionsInfo()
        {
            this.HardwareVersion = new byte[HardwareVersionSize];
            this.SerialNumber = new byte[SerialNumberSize];
            this.SoftwareVersion = new byte[SoftwareVersionSize];
        }

        /// <summary>Initializes a new instance of the <see cref="PeripheralVersionsInfo"/> class. Initializes a new instance of the <see cref="PeripheralVersionsInfo"/> struct.</summary>
        /// <param name="hardwareVersion">The hardware version.</param>
        /// <param name="serialNumber">The serial number.</param>
        /// <param name="softwareVersion">The software version.</param>
        public PeripheralVersionsInfo(byte[] hardwareVersion, byte[] serialNumber, byte[] softwareVersion)
        {
            this.HardwareVersion = hardwareVersion ?? new byte[HardwareVersionSize];
            this.SerialNumber = serialNumber ?? new byte[SerialNumberSize];
            this.SoftwareVersion = softwareVersion ?? new byte[SoftwareVersionSize];
        }

        /// <summary>The hardware version size.</summary>
        public const int HardwareVersionSize = 4;

        /// <summary>The serial number size.</summary>
        public const int SerialNumberSize = 14;

        /// <summary>The software version size.</summary>
        public const int SoftwareVersionSize = 4;

        /// <summary>The Expected structure size.</summary>
        public const int Size = HardwareVersionSize + SerialNumberSize + SoftwareVersionSize;

        /// <summary>The hardware version.</summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = HardwareVersionSize)]
        [XmlIgnore]
        public byte[] HardwareVersion;

        /// <summary>The serial number.</summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = SerialNumberSize)]
        [XmlIgnore]
        public byte[] SerialNumber;

        /// <summary>The software version.</summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = SoftwareVersionSize)]
        [XmlIgnore]
        public byte[] SoftwareVersion;

        /// <summary>The equals.</summary>
        /// <param name="obj">The obj.</param>
        /// <returns>The <see cref="bool"/>.</returns>
        public override bool Equals(object obj)
        {
            // If parameter is null return false.
            if (obj == null)
            {
                return false;
            }

            var p = (PeripheralVersionsInfo)obj;
            return this.SerialNumber.SequenceEqual(p.SerialNumber) && this.HardwareVersion.SequenceEqual(p.HardwareVersion)
                   && this.SoftwareVersion.SequenceEqual(p.SoftwareVersion);
        }

        public override string ToString()
        {
            return string.Format("Serial#:{0}, Software Version: {1}, Hardware Version: {2}", this.SerialNumberText, this.SoftwareVersionText, this.HardwareVersionText);
        }

        /// <summary>The to version string.</summary>
        /// <param name="bytes">The bytes.</param>
        /// <returns>The <see cref="string"/>.</returns>
        public static string ToVersionString(byte[] bytes)
        {
            var sb = new StringBuilder();
            foreach (var b in bytes)
            {
                sb.Append(b + ".");
            }

            return sb.ToString().TrimEnd('.');
        }

        /// <summary>Gets or sets the hardware version text.</summary>
        [IgnoreDataMember]
        public string HardwareVersionText
        {
            get
            {
                return this.HardwareVersion[0] == '\0' ? string.Empty : ToVersionString(this.HardwareVersion);
            }

            set
            {
                Array.Copy(GetBinaryVersion(value), this.HardwareVersion, this.HardwareVersion.Length);
            }
        }

        private static byte[] GetBinaryVersion(string versionText, int versionLength = 4)
        {
            var bytes = new byte[versionLength];
            var version = Version.Parse(versionText);
            bytes[0] = (byte)version.Major;
            bytes[1] = (byte)version.Minor;
            bytes[2] = (byte)version.Build;
            bytes[3] = (byte)version.Revision;
            return bytes;
        }

        /// <summary>Gets or sets the serial number text.</summary>
        [IgnoreDataMember]
        public string SerialNumberText
        {
            get
            {
                return Encoding.ASCII.GetString(this.SerialNumber).TrimEnd('\0');
            }

            set
            {
                var bytes = Encoding.ASCII.GetBytes(value).Take(this.SerialNumber.Length).ToArray();
                bytes.CopyTo(this.SerialNumber, 0);
            }
        }

        /// <summary>Gets or sets the software version text.</summary>
        [IgnoreDataMember]
        public string SoftwareVersionText
        {
            get
            {
                return this.SoftwareVersion[0] == '\0' ? string.Empty : ToVersionString(this.SoftwareVersion);
            }

            set
            {
                Array.Copy(GetBinaryVersion(value), this.SoftwareVersion, this.SoftwareVersion.Length);
            }
        }

        /// <summary>The get hash code.</summary>
        /// <returns>The <see cref="int" />.</returns>
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        /// <summary>The ==.</summary>
        /// <param name="p1">The c 1.</param>
        /// <param name="p2">The c 2.</param>
        /// <returns></returns>
        public static bool operator ==(PeripheralVersionsInfo p1, PeripheralVersionsInfo p2)
        {
            return p1.Equals(p2);
        }

        /// <summary>The !=.</summary>
        /// <param name="p1">The c 1.</param>
        /// <param name="p2">The c 2.</param>
        /// <returns></returns>
        public static bool operator !=(PeripheralVersionsInfo p1, PeripheralVersionsInfo p2)
        {
            if ((object)p1 == null)
            {
                return false;
            }
            return !p1.Equals(p2);
        }
    }
}