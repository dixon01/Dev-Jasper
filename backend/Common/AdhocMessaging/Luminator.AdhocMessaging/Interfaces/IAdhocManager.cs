namespace Luminator.AdhocMessaging.Interfaces
{
    using System;
    using System.Collections.Generic;
    using System.Net;
    using System.Threading.Tasks;

    using Luminator.AdhocMessaging.Models;

    public interface IAdhocManager
    {
        AdhocConfiguration AdhocConfiguration { get; set; }

        List<Message> GetAllMessagesForUnit(string unitIdOrName, string route = "", DateTime? unitLocalTime = null);

        Task<List<Message>> GetAllMessagesForUnitAsync(
            string unitIdOrName,
            string onRoute,
            DateTime? unitDateTime = null);

        Task<Unit> GetUnitAsync(string unitName);
        Task<List<Unit>> GetAllUnitsAsync();

        Task<List<Vehicle>> GetAllVechiclesAsync();

        HttpStatusCode RegisterUnit(Unit unit);

        HttpStatusCode RegisterUnit(
            string unitName,
            string tenantId = AdhocConstants.DefaultTenentId,
            string description = AdhocConstants.DefaultUnitRegistrationDescription,
            string vehicleId = "",
            bool isActive = true);

        Task<HttpStatusCode> RegisterUnitAsync(Unit unit);

        HttpStatusCode DeleteUnit(string unitName);
        HttpStatusCode DeleteVehicle(string vehicleName);

        Task<HttpStatusCode> RegisterUnitAsync(
            string unitName,
            string tenantId = AdhocConstants.DefaultTenentId,
            string description = AdhocConstants.DefaultUnitRegistrationDescription,
            string vehicleId = "",
            bool isActive = true);

        Task<HttpStatusCode> RegisterVehicleAndUnitAsync(
            string vehicleId,
            string unitName,
            string description = AdhocConstants.DefaultVehicleRegistrationDescription,
            string tenantId = AdhocConstants.DefaultTenentId,
            bool isActive = true);

        HttpStatusCode RegisterUnits(IList<Unit> units);

        bool RegisterVehicle(Vehicle bus);

        Task<HttpStatusCode> RegisterVehicleAsync(Vehicle bus);

        Task<HttpStatusCode> UnitExistsAsync(string unitId);

        Task<HttpStatusCode> VehicleExistsAsync(string vehicleId);
    }
}