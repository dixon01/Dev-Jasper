namespace Luminator.Motion.Protran.AdHocMessagingProtocol.Models
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;

    using Gorba.Common.Medi.Core;
    using Gorba.Common.Utility.Core;
    using Luminator.Motion.Protran.AdHocMessagingProtocol.Interfaces;

    /// <summary>The adhoc get messages request for a Unit.</summary>
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "Reviewed. Suppression is OK here.")]
    public class AdHocGetMessagesRequest : AdHocRequest, IAdHocGetMessagesRequest
    {
        /// <summary>Initializes a new instance of the <see cref="AdHocGetMessagesRequest"/> class.</summary>
        /// <param name="vehicleId">The vehicle id.</param>
        /// <param name="route">The route.</param>
        /// <param name="units">The list of units.</param>
        /// <param name="unitLocalTimeStamp">The unit local time stamp or null for Now.</param>
        /// <param name="trip">The optional trip.</param>
        public AdHocGetMessagesRequest(string vehicleId, string route, IList<IAdHocUnit> units, DateTime? unitLocalTimeStamp = null, string trip = "")
            : base(vehicleId, route, units, trip)
        {
            this.UnitLocalTimeStamp = unitLocalTimeStamp ?? DateTime.Now.Date.Midnight();
        }

        /// <summary>Initializes a new instance of the <see cref="AdHocGetMessagesRequest"/> class.</summary>
        /// <param name="route">The route.</param>
        /// <param name="primaryUnitName">The primary unit name.</param>
        /// <param name="vehicleId">The optional vehicle id.</param>
        /// <param name="unitLocalTimeStamp">The unit local time stamp or null for Now.</param>
        public AdHocGetMessagesRequest(string route, string primaryUnitName, string vehicleId = "", DateTime? unitLocalTimeStamp = null)
            : base(route, primaryUnitName, vehicleId)
        {
            this.UnitLocalTimeStamp = unitLocalTimeStamp ?? DateTime.Now.Date.Midnight();
            if (!this.Units.Any())
            {
                this.Units.Add(new AdHocUnit(MessageDispatcher.Instance.LocalAddress.Unit));
            }
        }

        public DateTime? UnitLocalTimeStamp { get; set; }

        public override string ToString()
        {
            return $"VehcileId=[{this.VehicleId}], Unit:[{this.FirstUnit}], Route=[{this.Route}], Date=[{this.UnitLocalTimeStamp}]";
        }
    }
}