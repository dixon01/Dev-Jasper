// --------------------------------------------------------------------------------------------------------------------
// <copyright file="XimpleAdHocMessage.cs" company="Luminator LTG">
//   Copyright © 2011-2018 LuminatorLTG. All rights reserved.
// </copyright>
// <summary>
//   Defines the XimpleAdHocMessage type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Luminator.Motion.Protran.AdHocMessagingProtocol
{
    using System;

    using Luminator.Motion.Protran.AdHocMessagingProtocol.Interfaces;

    [Serializable]
    public class XimpleAdHocMessage : IXimpleAdHocMessage
    {
        public XimpleAdHocMessage()
        {
            this.Text = string.Empty;
            this.Destinations = string.Empty;
            this.Title = string.Empty;
            this.Type = string.Empty;
            this.VehicleId = string.Empty;
            this.Route = string.Empty;
            this.TimeToLive = TimeSpan.Zero;
            this.Id = Guid.NewGuid();
        }

        public string Destinations { get; set; }

        public DateTime? EndDate { get; set; }

        public Guid Id { get; set; }

        public int Language { get; set; }

        public string Route { get; set; }

        public DateTime? StartDate { get; set; }

        public string Text { get; set; }

        public TimeSpan TimeToLive { get; set; }

        public string Title { get; set; }

        public string Type { get; set; }

        public string VehicleId { get; set; }
    }
}