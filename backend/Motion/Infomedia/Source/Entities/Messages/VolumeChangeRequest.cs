namespace Gorba.Motion.Infomedia.Entities.Messages
{
    using System;

    [Serializable]
    public class VolumeChangeRequest
    {
        public byte InteriorVolume { get; set; }
        public byte ExteriorVolume { get; set; }
        public ushort AudioRefreshInterval { get; set; }

        public VolumeChangeRequest()
        {
            this.InteriorVolume = VolumeSettingsMessage.Ignored;
            this.ExteriorVolume = VolumeSettingsMessage.Ignored;
            this.AudioRefreshInterval = 0;
        }

        public VolumeChangeRequest(byte interiorVolume, byte exteriorVolume, ushort audioRefreshInterval = VolumeSettingsMessage.Ignored)
        {
            this.InteriorVolume = interiorVolume;
            this.ExteriorVolume = exteriorVolume;
            this.AudioRefreshInterval = audioRefreshInterval;
        }

        public override string ToString()
        {
            return string.Format("InteriorVolume={0}, ExteriorVolume={1}, AudioRefreshInterval={2}", this.InteriorVolume, this.ExteriorVolume, this.AudioRefreshInterval);
        }
    }
}