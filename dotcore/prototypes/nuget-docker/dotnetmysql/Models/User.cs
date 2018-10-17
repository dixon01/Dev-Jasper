using System;
using System.Collections.Generic;

namespace dotnetmysql.Models
{
    public partial class User
    {
        public string Username { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public DateTime? CreateTime { get; set; }
        public int Id { get; set; }
    }
}
