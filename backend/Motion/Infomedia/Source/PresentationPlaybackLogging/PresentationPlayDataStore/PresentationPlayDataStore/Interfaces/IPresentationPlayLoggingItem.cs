namespace Luminator.PresentationPlayLogging.DataStore.Interfaces
{
    using System;

    using Luminator.PresentationPlayLogging.Core.Interfaces;

    public interface IPresentationPlayLoggingItem : IInfotransitPresentationInfo
    {
        Guid? UnitId { get; set; }
       
        int Id { get; set; }

        DateTime? Modified { get; set; }

        int? TenantId { get; set; }      // TODO move to GUID down the road
    }
}