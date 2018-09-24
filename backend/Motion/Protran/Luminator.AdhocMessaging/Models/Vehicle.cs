namespace Luminator.AdhocMessaging.Models
{
    using System.Linq;

    using Newtonsoft.Json;

    public class Vehicle
    {
        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("destinationType")]
        public Destinationtype DestinationType { get; set; }

        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("isActive")]
        public bool IsActive { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("tenantId")]
        public string TenantId { get; set; }

        [JsonProperty("units")]
        public Unit[] Units { get; set; }
      

        public override string ToString()
        {
            return $"  ------------ Vehicle {nameof(this.Id)}: {this.Id} ---------------- \n "
                   + $"{nameof(this.Description)}: {this.Description}\n "
                   + $"{nameof(this.DestinationType)}: {this.DestinationType}\n "
                   + $"{nameof(this.IsActive)}: {this.IsActive}\n "
                   + $"{nameof(this.Name)}: {this.Name}\n"
                   + $"{nameof(this.TenantId)}: {this.TenantId}\n "
                   + $" === {nameof(this.Units)} ==== \n {string.Join("\n", this.Units.Select( x => x.ToString()).ToArray())}";
        }
    }
}