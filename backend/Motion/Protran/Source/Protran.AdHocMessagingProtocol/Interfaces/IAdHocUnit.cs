namespace Luminator.Motion.Protran.AdHocMessagingProtocol.Interfaces
{
    public interface IAdHocUnit
    {
        string Id { get; set; }
        string Description { get; set; }

        string Name { get; set; }

        string VehicleId { get; set; }

        int Version { get; set; }
    }
}