namespace Luminator.PresentationPlayLogging.DataStore.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.ModelConfiguration.Conventions;

    using Luminator.PresentationPlayLogging.DataStore.Interfaces;

    public class DestinationBase : IItem
    {
        public Guid Id { get; set; }

        public bool? IsActive { get; set; }

        public string Description { get; set; }

        public string Name { get; set; }

        public DateTime Created { get; set; }

        public DateTime? Modified { get; set; }
    }

    public class DestinationType : DestinationBase//, IDestinationType
    {
        public int Order { get; set; }

        [ForeignKey("Tenant")]
        public Guid? TenantId { get; set; }

        public Tenant Tenant { get; set; }
       
    }

    public class Depot : DestinationBase//, IDepot
    {

    }

    public class Tenant : DestinationBase//, ITenant
    {
        public virtual ICollection<Unit> Units { get; set; }
    }

    public class Vehicle : DestinationBase//, IVehicle
    {
        public Depot Depot { get; set; }

        [ForeignKey("Depot")]
        public Guid? DepotId { get; set; }

        [ForeignKey("DestinationType")]
        public Guid? DestinationTypeId { get; set; }

        public DestinationType DestinationType { get; set; }

        [ForeignKey("Tenant")]
        public Guid? TenantId { get; set; }

        public Tenant Tenant { get; set; }

        public virtual ICollection<Unit> Units { get; set; }
    }


    public class Unit : DestinationBase, IUnit
    {
        public DestinationType DestinationType { get; set; }

        [ForeignKey("DestinationType")]
        public Guid? DestinationTypeId { get; set; }

        public bool? IsConnected { get; set; }

        public string NetworkAddress { get; set; }

        public int ProductType_Id { get; set; }

        [ForeignKey("Tenant")]
        public Guid? TenantId { get; set; }

        public Tenant Tenant { get; set; }

        public int UpdateGroup_Id { get; set; }

        public Vehicle Vehicle { get; set; }

        [ForeignKey("Vehicle")]
        public Guid? VehicleId { get; set; }

        public int Version { get; set; }
    }
}
