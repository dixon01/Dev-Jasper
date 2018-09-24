namespace Luminator.Motion.Protran.AdHocMessagingProtocol.Models
{
    using Gorba.Common.Medi.Core;

    using Luminator.Motion.Protran.AdHocMessagingProtocol.Interfaces;

    public class AdHocUnit : IAdHocUnit
    {
        public AdHocUnit(string name, string description = "")
        {
            if (string.IsNullOrEmpty(name))
            {
                name = MessageDispatcher.Instance.LocalAddress.Unit;
            }

            this.Name = name;
            if (string.IsNullOrEmpty(description))
            {
                description = "New Registration";
            }

            this.Description = description;
        }

        public string Id { get; set; }

        public string Description { get; set; }

        public string Name { get; set; }

        public string VehicleId { get; set; }

        public int Version { get; set; }
    }
}
