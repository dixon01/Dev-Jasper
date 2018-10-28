using System;
using System.Collections.Generic;

namespace TestDB.Models
{
    public partial class Users
    {
        public int Id { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime? LastModifiedOn { get; set; }
        public int Version { get; set; }
        public string Username { get; set; }
        public string Domain { get; set; }
        public string HashedPassword { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Culture { get; set; }
        public string TimeZone { get; set; }
        public string Description { get; set; }
        public DateTime? LastLoginAttempt { get; set; }
        public DateTime? LastSuccessfulLogin { get; set; }
        public int ConsecutiveLoginFailures { get; set; }
        public bool IsEnabled { get; set; }
        public int OwnerTenantId { get; set; }
    }
}
