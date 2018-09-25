// Gorba.Motion.Protran
// Luminator.Protran.MessagingProtocol
// $Rev::                                   
// 

namespace Luminator.Motion.Protran.AdHocMessagingProtocol.Interfaces
{
    using System.Collections.Generic;

    public interface IAdHocRequest
    {
        bool IsValid { get; }

        string Route { get; set; }

        string Trip { get; set; } // optional

        IList<IAdHocUnit> Units { get; set; } // may change this to the Primary unit name only 
        
        string VehicleId { get; set; }

        string FirstUnit { get; }
    }
}