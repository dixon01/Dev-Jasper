namespace Luminator.PeripheralDimmer.Models
{
    using System;

    [Serializable]
    public class VersionInfo
    {
        public VersionInfo(string hardwareVersion, string softwareVersion)
        {
            this.HardwareVersion = hardwareVersion;
            this.SoftwareVersion = softwareVersion;
        }

        public string HardwareVersion { get; set; }
        public string SoftwareVersion { get; set; }

        public override string ToString()
        {
            return string.Format("Software Version={0}, Hardware Version={1}", this.SoftwareVersion, this.HardwareVersion);
        }
    }
}