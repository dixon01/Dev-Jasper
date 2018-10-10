using System.Device.Location;

namespace Gorba.Motion.Infomedia.Entities.Messages
{
    public interface IVehiclePositionMessage
    {
        GeoCoordinate GeoCoordinate { get; set; }
        bool IsValid { get; }

        string Route { get; set; }

        string Trip { get; set; }
    }
}