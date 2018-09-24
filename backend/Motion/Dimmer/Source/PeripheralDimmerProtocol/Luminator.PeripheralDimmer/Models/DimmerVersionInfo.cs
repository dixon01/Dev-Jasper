// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Luminator Technology Group" file="DimmerVersionInfo.cs">
//   Copyright © 2011-2017 LTG. All rights reserved.
// </copyright>
// <author>Kevin Hartman</author>
// --------------------------------------------------------------------------------------------------------------------
namespace Luminator.PeripheralDimmer.Models
{
    using System;
    using System.Linq;
    using System.Runtime.InteropServices;
    using System.Text;
    using System.Xml.Serialization;

    /// <summary>The dimmer version info.</summary>
    [Serializable]
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1, Size = Size)]
    public class DimmerVersionInfo
    {
        /// <summary>The Expected structure size.</summary>
        public const int Size = HardwareVersionSize + SoftwareVersionSize;

        /// <summary>The hardware version size.</summary>
        public const int HardwareVersionSize = 16;

        /// <summary>The software version size.</summary>
        public const int SoftwareVersionSize = 16;

        /// <summary>The hardware version.</summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = HardwareVersionSize)]
        [XmlIgnore]
        public byte[] HardwareVersion;

        /// <summary>The software version.</summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = SoftwareVersionSize)]
        [XmlIgnore]
        public byte[] SoftwareVersion;

        /// <summary>Initializes a new instance of the <see cref="DimmerVersionInfo"/> class.</summary>
        /// <param name="hardwareVersion">The hardware version.</param>
        /// <param name="softwareVersion">The software version.</param>
        public DimmerVersionInfo(byte[] hardwareVersion, byte[] softwareVersion)
            : this()
        {
            Array.Copy(hardwareVersion, 0, this.HardwareVersion, 0, HardwareVersionSize);
            Array.Copy(softwareVersion, 0, this.SoftwareVersion, 0, SoftwareVersionSize);
        }

        /// <summary>Initializes a new instance of the <see cref="DimmerVersionInfo"/> class.</summary>
        /// <param name="hardwareVersion">The hardware version.</param>
        /// <param name="softwareVersion">The software version.</param>
        /// <exception cref="ArgumentException"></exception>
        public DimmerVersionInfo(string hardwareVersion, string softwareVersion)
            : this()
        {
            if (hardwareVersion == null || hardwareVersion.Length >= HardwareVersionSize)
            {
                throw new ArgumentException("HardwareVersionSize max size " + HardwareVersionSize);
            }

            if (softwareVersion == null || softwareVersion.Length >= HardwareVersionSize)
            {
                throw new ArgumentException("SoftwareVersionSize max size " + SoftwareVersionSize);
            }

            Encoding.ASCII.GetBytes(hardwareVersion).ToArray().CopyTo(this.HardwareVersion, 0);
            Encoding.ASCII.GetBytes(softwareVersion).ToArray().CopyTo(this.SoftwareVersion, 0);
        }

        /// <summary>Initializes a new instance of the <see cref="DimmerVersionInfo" /> class.</summary>
        public DimmerVersionInfo()
        {
            this.HardwareVersion = new byte[HardwareVersionSize];
            this.SoftwareVersion = new byte[SoftwareVersionSize];
        }

        /// <summary>Initializes a new instance of the <see cref="DimmerVersionInfo"/> class.</summary>
        /// <param name="dimmerVersionInfo">The dimmer version info.</param>
        public DimmerVersionInfo(DimmerVersionInfo dimmerVersionInfo)
            : this(dimmerVersionInfo.HardwareVersion, dimmerVersionInfo.SoftwareVersion)
        {
        }

        /// <summary>The to version string.</summary>
        /// <param name="bytes">The bytes.</param>
        /// <returns>The <see cref="string"/>.</returns>
        private static string ToVersionString(byte[] bytes)
        {
            var buffer = new byte[20];
            var idx = 0;
            foreach (byte t in bytes)
            {
                if (t == '\0' || t == ' ')
                {
                    break;
                }
                buffer[idx++] = t;
            }
            return Encoding.ASCII.GetString(buffer).TrimEnd('\0');
            //return Encoding.ASCII.GetString(bytes).TrimEnd('\0').TrimEnd(' ');
        }

        /// <summary>The to string.</summary>
        /// <returns>The <see cref="string" />.</returns>
        public override string ToString()
        {
            return string.Format("Software Version={0}, Hardware Version={1}", this.SoftwareVersionText, this.HardwareVersionText);
        }

        /// <summary>Gets or sets the software version text. This is not a normal version number string.</summary>
        public string SoftwareVersionText
        {
            get
            {
                return this.SoftwareVersion[0] == '\0' ? string.Empty : ToVersionString(this.SoftwareVersion);
            }
        }

        /// <summary>Gets or sets the hardware version text.</summary>
        public string HardwareVersionText
        {
            get
            {
                return this.HardwareVersion[0] == '\0' ? string.Empty : ToVersionString(this.HardwareVersion);
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
    }
}