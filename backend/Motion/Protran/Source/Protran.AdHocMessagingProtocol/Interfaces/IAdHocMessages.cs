// Gorba.Motion.Protran
// Luminator.Protran.MessagingProtocol
// $Rev::                                   
// 

namespace Luminator.Motion.Protran.AdHocMessagingProtocol.Interfaces
{
    using System;
    using System.Collections.Generic;

    public interface IAdHocMessages : IAdHocResponse
    {
        DateTime Created { get; set; }

        IList<IXimpleAdHocMessage> Messages { get; set; }
    }
}