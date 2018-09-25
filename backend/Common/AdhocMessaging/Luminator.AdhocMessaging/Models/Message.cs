
namespace Luminator.AdhocMessaging.Models
{
    using System;
    using System.Net;
    using System.Collections.Generic;

    using Newtonsoft.Json;
    using Newtonsoft.Json.Serialization;

    public partial class Message
    {
        [JsonProperty("endDate")]
        public System.DateTime EndDate { get; set; }

        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("isActive")]
        public bool IsActive { get; set; }

        [JsonProperty("startDate")]
        public System.DateTime StartDate { get; set; }

        [JsonProperty("tenantId")]
        public string TenantId { get; set; }

        [JsonProperty("text")]
        public string Text { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("user")]
        public User User { get; set; }

        [JsonProperty("userId")]
        public string UserId { get; set; }

        [JsonProperty("createDate")]
        public System.DateTime CreateDate { get; set; }

        [JsonProperty("destinationTypeId")]
        public string DestinationTypeId { get; set; }

        [JsonProperty("status")]
        public string Status { get; set; }

        //  [JsonProperty("messageDestinations")]
        //  public MessageMessageDestination[] MessageDestinations { get; set; }

        [JsonProperty("messageUnits")]
        public MessageUnit[] MessageUnits { get; set; }

        public override string ToString()
        {
            return $" Message:  {nameof(this.Id)}: {this.Id},  {nameof(this.Text)}: {this.Text}, {nameof(this.Title)}: {this.Title}, {nameof(this.EndDate)}: {this.EndDate},  {nameof(this.IsActive)}: {this.IsActive}, {nameof(this.StartDate)}: {this.StartDate}, {nameof(this.TenantId)}: {this.TenantId},{nameof(this.Status)}: {this.Status}";
        }
    }


    public partial class MessageDestination
    {
        [JsonProperty("message")]
        public DestinationMessage Message { get; set; }
    }

    public partial class DestinationMessage
    {
    }

    public partial class MessageMessageUnit
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("messageId")]
        public long MessageId { get; set; }

        [JsonProperty("message")]
        public DestinationMessage Message { get; set; }

        [JsonProperty("unitId")]
        public string UnitId { get; set; }

        [JsonProperty("unitName")]
        public string UnitName { get; set; }

        [JsonProperty("createDate")]
        public System.DateTime CreateDate { get; set; }

        [JsonProperty("isActive")]
        public bool IsActive { get; set; }
    }

    public partial class MessageUser
    {
        [JsonProperty("configuration")]
        public string Configuration { get; set; }

        [JsonProperty("friendlyName")]
        public string FriendlyName { get; set; }

        [JsonProperty("fullName")]
        public string FullName { get; set; }

        [JsonProperty("isEnabled")]
        public bool IsEnabled { get; set; }

        [JsonProperty("isLockedOut")]
        public bool IsLockedOut { get; set; }

        [JsonProperty("jobTitle")]
        public string JobTitle { get; set; }

        [JsonProperty("tenantId")]
        public string TenantId { get; set; }

        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("userName")]
        public string UserName { get; set; }

        [JsonProperty("normalizedUserName")]
        public string NormalizedUserName { get; set; }

        [JsonProperty("email")]
        public string Email { get; set; }

        [JsonProperty("normalizedEmail")]
        public string NormalizedEmail { get; set; }

        [JsonProperty("emailConfirmed")]
        public bool EmailConfirmed { get; set; }

        [JsonProperty("passwordHash")]
        public string PasswordHash { get; set; }

        [JsonProperty("securityStamp")]
        public string SecurityStamp { get; set; }

        [JsonProperty("concurrencyStamp")]
        public string ConcurrencyStamp { get; set; }

        [JsonProperty("phoneNumber")]
        public string PhoneNumber { get; set; }

        [JsonProperty("phoneNumberConfirmed")]
        public bool PhoneNumberConfirmed { get; set; }

        [JsonProperty("twoFactorEnabled")]
        public bool TwoFactorEnabled { get; set; }

        [JsonProperty("lockoutEnd")]
        public System.DateTime LockoutEnd { get; set; }

        [JsonProperty("lockoutEnabled")]
        public bool LockoutEnabled { get; set; }

        [JsonProperty("accessFailedCount")]
        public long AccessFailedCount { get; set; }

        [JsonProperty("roles")]
        public Role[] Roles { get; set; }

        [JsonProperty("claims")]
        public Claim[] Claims { get; set; }

        [JsonProperty("logins")]
        public Login[] Logins { get; set; }
    }

    public partial class Claim
    {
        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("userId")]
        public string UserId { get; set; }

        [JsonProperty("claimType")]
        public string ClaimType { get; set; }

        [JsonProperty("claimValue")]
        public string ClaimValue { get; set; }
    }

    public partial class Login
    {
        [JsonProperty("loginProvider")]
        public string LoginProvider { get; set; }

        [JsonProperty("providerKey")]
        public string ProviderKey { get; set; }

        [JsonProperty("providerDisplayName")]
        public string ProviderDisplayName { get; set; }

        [JsonProperty("userId")]
        public string UserId { get; set; }
    }

    public partial class Role
    {
        [JsonProperty("userId")]
        public string UserId { get; set; }

        [JsonProperty("roleId")]
        public string RoleId { get; set; }
    }

    public partial class MessageUnit
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("messageId")]
        public long MessageId { get; set; }

        [JsonProperty("message")]
        public Message Message { get; set; }

        [JsonProperty("unitId")]
        public string UnitId { get; set; }

        [JsonProperty("unitName")]
        public string UnitName { get; set; }

        [JsonProperty("createDate")]
        public System.DateTime CreateDate { get; set; }

        [JsonProperty("isActive")]
        public bool IsActive { get; set; }
    }

    public partial class User
    {
        [JsonProperty("fullName")]
        public string FullName { get; set; }

        [JsonProperty("isEnabled")]
        public bool IsEnabled { get; set; }

        [JsonProperty("jobTitle")]
        public string JobTitle { get; set; }

        [JsonProperty("tenantId")]
        public string TenantId { get; set; }

        [JsonProperty("email")]
        public string Email { get; set; }
    }

    public partial class Message
    {
        public static Message[] FromJson(string json) => JsonConvert.DeserializeObject<Message[]>(json, Converter.Settings);
    }

    public static partial class Serialize
    {
        public static string ToJson(this Message[] self) => JsonConvert.SerializeObject(self, Converter.Settings);
    }

    public partial class Converter
    {
        public static readonly JsonSerializerSettings Settings = new JsonSerializerSettings
        {
            Formatting = Formatting.Indented,
            // TypeNameHandling = TypeNameHandling.Objects,
             DateFormatString = "dd-MM-yyyy h:mm tt",
            // ContractResolver = new CamelCasePropertyNamesContractResolver()
            MetadataPropertyHandling = MetadataPropertyHandling.Ignore,
        };
    }
}
