// Gorba.Motion.Protran
// Luminator.Protran.MessagingProtocol
// <author>Kevin Hartman</author>
// $Rev::                                   
// 

namespace Luminator.Motion.Protran.AdHocMessagingProtocol.Interfaces
{
    using System;

    public interface IXimpleAdHocMessage
    {
        string Destinations { get; set; }

        DateTime? EndDate { get; set; }

        Guid Id { get; set; }

        int Language { get; set; }

        string Route { get; set; }

        DateTime? StartDate { get; set; }

        string Text { get; set; }

        TimeSpan TimeToLive { get; set; }

        string Title { get; set; }

        string Type { get; set; }

        string VehicleId { get; set; }
    }
}