using System;

namespace Luminator.AdhocMessaging.Models
{
    using System.Collections.Generic;
    using System.Linq;

    //public class DestinationType
    //{
    //    public string Description { get; set; }
    //    public string Id { get; set; }
    //    public bool IsActive { get; set; }
    //    public string Name { get; set; }
    //    public int Order { get; set; }
    //    public string TenantId { get; set; }
    //}

    //public class Bus
    //{
    //    public string Description { get; set; }

    //    public DestinationType DestinationType { get; set; }

    //    public Guid? Id { get; set; }

    //    public bool? IsActive { get; set; }

    //    public string Name { get; set; }

    //    public Guid? TenantId { get; set; }

    //    public List<TftUnit> Units { get; set; }

    //    public override string ToString()
    //    {
    //        return $" {nameof(this.Description)}: {this.Description}\n"
    //               + $" {nameof(this.Id)}: {this.Id} \n"
    //               + $" {nameof(this.IsActive)}: {this.IsActive} \n"
    //               + $" {nameof(this.Name)}: {this.Name}\n"
    //               + $" {nameof(this.TenantId)}: {this.TenantId}\n"
    //               + $" {nameof(this.Units)}: {this.Units.Select(x => x.ToString()).ToArray()}";

    //    }
    //}
    //}


    public class VehicleRoot
    {
        public Vehicle[] Vehicles { get; set; }
    }
}
