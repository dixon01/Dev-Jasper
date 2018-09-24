namespace Luminator.Motion.Protran.AdHocMessagingProtocol.Models
{
    using System.Collections.Generic;
    using System.Linq;

    using Luminator.Motion.Protran.AdHocMessagingProtocol.Interfaces;

    public class AdHocRegisterRequest : AdHocRequest, IAdHocRegisterRequest
    {
        public AdHocRegisterRequest(string vehicleId, IList<IAdHocUnit> units)
        {
            this.VehicleId = vehicleId;
            this.Units = units;
            this.Route = string.Empty;
            this.Trip = string.Empty;
        }

        public AdHocRegisterRequest(string vehicleId, IList<string> unitNames)
        {
            this.VehicleId = vehicleId;
            this.Route = string.Empty;
            this.Trip = string.Empty;
            this.Units = new List<IAdHocUnit>();
            foreach (var item in unitNames)
            {
                this.Units.Add(new AdHocUnit(item));
            }
        }

        public AdHocRegisterRequest(string vehicleId, string primaryUnitName) : this(vehicleId, new List<string> { primaryUnitName })
        {            
        }

        /// <summary>The request is valid.</summary>
        public override bool IsValid => base.IsValid && !string.IsNullOrEmpty(this.VehicleId) && 
            (this.Units != null && this.Units.Count > 0);

        public override string ToString()
        {
            return $"VehicleId=[{this.VehicleId}], Units=[{this.Units.Aggregate(string.Empty, (current, m) => current + m.Name + ",")}], Route=[{this.Route}], Trip=[{this.Trip}]";
        }
    }
}