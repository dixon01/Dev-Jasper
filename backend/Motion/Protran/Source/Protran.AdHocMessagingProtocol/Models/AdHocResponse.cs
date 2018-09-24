namespace Luminator.Motion.Protran.AdHocMessagingProtocol.Models
{
    using System;
    using System.Net;

    using Luminator.Motion.Protran.AdHocMessagingProtocol.Interfaces;

    public class AdHocResponse : IAdHocResponse
    {
        public DateTime? ResponseTimeStamp { get; set; }

        public string Response { get; set; }

        public HttpStatusCode Status { get; set; }

        public bool IsRegistered
        {
            get
            {
                return this.Status == HttpStatusCode.OK;
            }
        }
    }
}
