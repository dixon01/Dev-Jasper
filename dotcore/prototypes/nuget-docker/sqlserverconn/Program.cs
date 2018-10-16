using System;
using sqlserverconn.Models;
using System.Linq;
namespace sqlserverconn
{
    class Program
    {
        static void Main(string[] args)
        {
          Console.WriteLine("SQLServer Users!");

            var dbContext = new TestingContext();
            var users = dbContext.Users.ToList();
            foreach(var c in users)
            {
                Console.WriteLine($"id {c.Id}, name {c.Username}, email {c.Email}");
            }
        }
    }
}
