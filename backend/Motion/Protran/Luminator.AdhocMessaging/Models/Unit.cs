namespace Luminator.AdhocMessaging.Models
{
    using System;

    using Newtonsoft.Json;

    public class Unit
    {
        [JsonProperty("createdOn")]
        public DateTime? CreatedOn { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("id")]
        public Guid Id { get; set; }

        [JsonProperty("isActive")]
        public bool? IsActive { get; set; }

        [JsonProperty("isConnected")]
        public bool? IsConnected { get; set; }

        [JsonProperty("lastModifiedOn")]
        public DateTime? LastModifiedOn { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("networkAddress")]
        public string NetworkAddress { get; set; }

        [JsonProperty("productType_Id")]
        public long ProductTypeId { get; set; }

        [JsonProperty("tenantId")]
        public string TenantId { get; set; }

        [JsonProperty("updateGroup_Id")]
        public long UpdateGroupId { get; set; }

        [JsonProperty("vehicleId")]
        public string VehicleId { get; set; }

        [JsonProperty("version")]
        public long Version { get; set; }

        public override string ToString()
        {
            return $" Unit {nameof(this.Id)}: {this.Id}, {nameof(this.Name)}: {this.Name}, {nameof(this.Description)}: {this.Description}, {nameof(this.IsActive)}: {this.IsActive}, {nameof(this.IsConnected)}: {this.IsConnected},  {nameof(this.CreatedOn)}: {this.CreatedOn}, {nameof(this.LastModifiedOn)}: {this.LastModifiedOn},  {nameof(this.NetworkAddress)}: {this.NetworkAddress}, {nameof(this.ProductTypeId)}: {this.ProductTypeId}, {nameof(this.TenantId)}: {this.TenantId}, {nameof(this.UpdateGroupId)}: {this.UpdateGroupId},  {nameof(this.Version)}: {this.Version}";
        }
    }
    
}