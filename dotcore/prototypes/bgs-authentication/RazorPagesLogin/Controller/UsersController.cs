using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TestDB.Models;

using Gorba.Common.Utility.Core;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace TestRest.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : Controller
    {
       private readonly GorbaCenterBackgroundSystemContext _context;

      

        public UsersController(GorbaCenterBackgroundSystemContext context)
            {
                _context = context;

                if (_context.Users.Count() == 0)
                {
                    // Create a new TodoItem if collection is empty,
                    // which means you can't delete all TodoItems.
                    _context.Users.Add(new Users{ Username = "admin" });
                    _context.SaveChanges();
                }

                var userAdmin = "admin";
                var passwordadmin = "1adbb3178591fd5bb0c248518f39bf6d";

                var userName = _context.Users.Single(a => a.Username == "admin");
                var passWord = userName.HashedPassword;

                bool resultUsername = String.Equals(userAdmin, userName.Username);
                bool resultPassword = String.Equals(passWord, passwordadmin);

                var text = "admin";
                var hashedPassword = SecurityUtility.Md5(text);

            //public override bool Equals(object obj)
            //{
            //    var other = obj as UserCredentials;
            //    return other != null && string.Equals(this.Username, other.Username)
            //                         && string.Equals(this.HashedPassword, other.HashedPassword);
            //}

        }

        // GET: api/<controller>

        [HttpGet]

        public ActionResult<List<Users>> GetAll()
        {
            
            return _context.Users.ToList();
            

        }


        [HttpGet("{id}", Name = "GetUser")]
        public ActionResult<Users> GetById(long id)
        {
            var item = _context.Users.Find(id);
            if (item == null)
            {
                return NotFound();
            }
            return item;
        }
        //public IEnumerable<string> Get()
        //{
        //    return new string[] { "value1", "value2" };
        //}

        // GET api/<controller>/5
        //[HttpGet("{id}")]
        //public string Get(int id)
        //{
        //    return "value";
        //}

        // POST api/<controller>
        [HttpPost]
        public void Post([FromBody]string value)
        {
        }

        // PUT api/<controller>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/<controller>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
