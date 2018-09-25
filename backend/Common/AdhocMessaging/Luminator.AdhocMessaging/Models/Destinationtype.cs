namespace Luminator.AdhocMessaging.Models
{
    using Newtonsoft.Json;

    public class Destinationtype
    {
        public Destinationtype(string id, string name, string description,  long? order)
        {
            this.Description = description;
            this.Id = id;
            this.Name = name;
            this.Order = order;
        }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("isActive")]
        public bool IsActive { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("order")]
        public long? Order { get; set; }

        [JsonProperty("tenantId")]
        public string TenantId { get; set; }

        public override string ToString()
        {
            return $"{nameof(this.Description)}: {this.Description}, {nameof(this.Id)}: {this.Id}, {nameof(this.IsActive)}: {this.IsActive}, {nameof(this.Name)}: {this.Name}, {nameof(this.Order)}: {this.Order}, {nameof(this.TenantId)}: {this.TenantId}";
        }
    }
}