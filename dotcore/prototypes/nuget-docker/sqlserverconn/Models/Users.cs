using System;
using System.Collections.Generic;

namespace sqlserverconn.Models
{
    public partial class Users
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public DateTime? Created { get; set; }
    }
}
