namespace Luminator.AdhocMessaging.Interfaces
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using Luminator.AdhocMessaging.Models;

    public interface IAdhocManager
    {
        AdhocConfiguration AdhocConfiguration { get; set; }

        bool RegisterUnit(Unit unit);
        Task<bool> RegisterUnitAsync(Unit unit);
        List<Unit> GetAllUnits();
        Task<List<Unit>> GetAllUnitsAsync();
        bool UnitExists(string unitId);

        bool VehicleExists(string vechicleId);
        bool RegisterVehicle(Vehicle bus);
        List<Vehicle> GetAllVechicles();
        Task<bool> RegisterVehicleAsync(Vehicle bus);
        Task<List<Vehicle>> GetAllVechiclesAsync();

        List<Message> GetAllMessagesForUnit(string unitIdOrName);
        List<Message> GetAllMessagesForVechicle(string vechicleIdOrName);
        List<Message> GetAllMessagesForUnitOnRoute(string unitIdOrName, string onRoute);
        List<Message> GetAllMessagesForVechicleOnRoute(string vechicleIdOrName, string onRoute);
    }
}