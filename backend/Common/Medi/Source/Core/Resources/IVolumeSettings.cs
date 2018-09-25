namespace Gorba.Common.Medi.Core.Resources
{
    /// <summary>The AudioVolumes interface.</summary>
    public interface IVolumeSettings
    {
        byte CurrentVolume { get; set; }
        byte MaximumVolume { get; set; }
        byte MinimumVolume { get; set; }
        byte DefaultVolume { get; set; }
    }
}