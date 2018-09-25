namespace Luminator.AdhocMessaging.Models
{
    using System;
    using System.Collections.Generic;

    using Newtonsoft.Json;

    public partial class Message
    {
        [JsonProperty("endDate")]
        public DateTime EndDate { get; set; }

        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("isActive")]
        public bool IsActive { get; set; }

        [JsonProperty("sender")]
        public Sender Sender { get; set; }

        [JsonProperty("startDate")]
        public DateTime StartDate { get; set; }

        [JsonProperty("text")]
        public string Text { get; set; }

        [JsonProperty("timeToLive")]
        public string TimeToLive { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("type")]
        public string PurpleType { get; set; }

        [JsonProperty("userId")]
        public string UserId { get; set; }
    }

    public class Sender
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
        public DateTime LockoutEnd { get; set; }

        [JsonProperty("lockoutEnabled")]
        public bool LockoutEnabled { get; set; }

        [JsonProperty("accessFailedCount")]
        public long AccessFailedCount { get; set; }

        [JsonProperty("roles")]
        public List<Role> Roles { get; set; }

        [JsonProperty("claims")]
        public List<Claim> Claims { get; set; }

        [JsonProperty("logins")]
        public List<Login> Logins { get; set; }
    }

    public class Claim
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

    public class Login
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

    public class Role
    {
        [JsonProperty("userId")]
        public string UserId { get; set; }

        [JsonProperty("roleId")]
        public string RoleId { get; set; }
    }

    public partial class Message
    {
        public static Message FromJson(string json)
        {
            return JsonConvert.DeserializeObject<Message>(json, Converter.Settings);
        }
    }

    public static class Serialize
    {
        public static string ToJson(this Message self)
        {
            return JsonConvert.SerializeObject(self, Converter.Settings);
        }
    }

    public class Converter
    {
        public static readonly JsonSerializerSettings Settings = new JsonSerializerSettings
                                                                     {
                                                                         MetadataPropertyHandling = MetadataPropertyHandling.Ignore,
                                                                         DateParseHandling = DateParseHandling.None
                                                                     };
    }
}