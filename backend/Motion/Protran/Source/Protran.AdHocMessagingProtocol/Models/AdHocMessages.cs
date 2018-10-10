namespace Luminator.Motion.Protran.AdHocMessagingProtocol.Models
{
    using System;
    using System.Collections.Generic;
    using System.Net;

    using Luminator.Motion.Protran.AdHocMessagingProtocol.Interfaces;

    public class AdHocMessages : AdHocResponse, IAdHocMessages
    {
        public AdHocMessages(DateTime created, IList<IXimpleAdHocMessage> messages, HttpStatusCode status = HttpStatusCode.OK)
        {
            this.Status = status;
            this.Created = created;
            this.Messages = messages;
        }

        public AdHocMessages()
        {
            this.Status = HttpStatusCode.OK;
            this.Created = DateTime.Now;
            this.Messages = new List<IXimpleAdHocMessage>();
        }

        public DateTime Created { get; set; }

        public IList<IXimpleAdHocMessage> Messages { get; set; }
    }
}