namespace Luminator.PresentationPlayLogging.DataStore.Interfaces
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    using Luminator.PresentationPlayLogging.DataStore.Models;


    public interface IDestinationBase : IItem
    {

        bool? IsActive { get; set; }

        string Description { get; set; }

        string Name { get; set; }

        //     DateTime? CreatedOn { get; set; }

        //     DateTime? ModifiedOn { get; set; }
    }

    public interface IDestinationType : IDestinationBase
    {
        int Order { get; set; }

        [ForeignKey("Tenant")]
        Guid? TenantId { get; set; }

        ITenant Tenant { get; set; }
    }

    public interface IUnit : IDestinationBase
    {
        DestinationType DestinationType { get; set; }

        [ForeignKey("DestinationType")]
        Guid? DestinationTypeId { get; set; }

        bool? IsConnected { get; set; }

        string NetworkAddress { get; set; }

        int ProductType_Id { get; set; }

        [ForeignKey("Tenant")]
        Guid? TenantId { get; set; }

        Tenant Tenant { get; set; }

        int UpdateGroup_Id { get; set; }

        Vehicle Vehicle { get; set; }

        [ForeignKey("Vehicle")]
        Guid? VehicleId { get; set; }

        int Version { get; set; }
    }

    public interface IDepot : IDestinationBase
    {
        Guid? DepotId { get; set; }
        // TODO the other props
    }

    public interface IVehicle : IDestinationBase
    {
        IDepot Depot { get; set; }

        [ForeignKey("Depot")]
        Guid? DepotId { get; set; }

        [ForeignKey("DestinationType")]
        Guid? DestinationTypeId { get; set; }

        IDestinationType DestinationType { get; set; }

        [ForeignKey("Tenant")]
        Guid? TenantId { get; set; }

        ITenant Tenant { get; set; }

        ICollection<IUnit> Units { get; set; }

    }

    public interface ITenant : IDestinationBase
    {
        ICollection<Unit> Units { get; set; }

    }
}
