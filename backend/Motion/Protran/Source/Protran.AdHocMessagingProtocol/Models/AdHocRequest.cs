namespace Luminator.Motion.Protran.AdHocMessagingProtocol.Models
{
    using System.Collections.Generic;
    using System.Linq;

    using Luminator.Motion.Protran.AdHocMessagingProtocol.Interfaces;

    /// <summary>The ad hoc request elements.</summary>
    public class AdHocRequest : IAdHocRequest
    {
        public AdHocRequest()
        {
            this.Route = string.Empty;
            this.Trip = string.Empty;
            this.VehicleId = string.Empty;
            this.Units = new List<IAdHocUnit>();
        }

        public AdHocRequest(string vehicleId, string route = "", IList<IAdHocUnit> units = null, string trip = "")
        {
            this.Route = route;
            this.Trip = trip;
            this.Units = units;
            this.VehicleId = vehicleId;
        }

        public AdHocRequest(string route, string primaryUnitName, string vehicleId)
            : this()
        {
            this.Route = route;
            this.Units = new List<IAdHocUnit> { new AdHocUnit(primaryUnitName) };
            this.VehicleId = vehicleId;
        }

        /// <summary>Get The first unit name.</summary>
        public string FirstUnit => this.Units != null ? this.Units?.First().Name : string.Empty;

        /// <summary>Test if the required members are valid.</summary>
        public virtual bool IsValid => this.Units != null && this.Units.Count >= 1;

        /// <summary>Gets or sets the optional route.</summary>
        public string Route { get; set; }

        /// <summary>Gets or sets the optional trip.</summary>
        public string Trip { get; set; }

        /// <summary>Gets or sets the units.</summary>
        public IList<IAdHocUnit> Units { get; set; }

        /// <summary>Gets or sets the optional vehicle id.</summary>
        public string VehicleId { get; set; }

        public override string ToString()
        {
            return $"VehicleId={this.VehicleId}, Route={this.Route}, Trip={this.Trip}, Units={this.Units?.Aggregate(string.Empty, (current, m) => current + m.Name + ",")}";
        }
    }
}