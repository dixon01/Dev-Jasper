using System;
using dotnetmysql.Models;
using System.Linq;

namespace dotnetmysql
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("MySql Users!");

            var dbContext = new testingContext();
            var users = dbContext.Users.ToList();
            foreach(var c in users)
            {
                Console.WriteLine($"id {c.Id}, name {c.Username}, email {c.Email}");
            }

        }
    }
}
