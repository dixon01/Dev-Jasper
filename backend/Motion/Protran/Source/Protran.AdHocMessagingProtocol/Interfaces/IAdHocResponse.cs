// Gorba.Motion.Protran
// Luminator.Protran.MessagingProtocol
// $Rev::                                   
// 

namespace Luminator.Motion.Protran.AdHocMessagingProtocol.Interfaces
{
    using System;
    using System.Net;

    public interface IAdHocResponse
    {
        DateTime? ResponseTimeStamp { get; set; }

        string Response { get; set; }

        HttpStatusCode Status { get; set; }
    }
}