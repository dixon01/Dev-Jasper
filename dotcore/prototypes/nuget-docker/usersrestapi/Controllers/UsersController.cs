using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using dotnetmysql.Models;
using Microsoft.AspNetCore.Mvc;

namespace usersrestapi.Controllers {
    [Route ("api/users")]
    [ApiController]
    public class UsersController : ControllerBase {

        private readonly testingContext _context;

        public UsersController (testingContext context) {
            _context = context;
            if (_context.Users.Count () == 0) {
                _context.Users.Add (new User {
                    Username = "Jon Doe",
                        Id = 900,
                        Email = "whats@where.com"
                });
                _context.SaveChanges ();
            }
        }
        // GET api/users
        [HttpGet]
        public ActionResult<IEnumerable<User>> Get () {
            return _context.Users.ToList ();
        }

        // GET api/users/5
        [HttpGet ("{id}")]
        public ActionResult<User> Get (int id) {
            var item = _context.Users.Find (id);
            if (item == null) {
                return NotFound ();
            }
            return item;
        }

        // POST api/users
        [HttpPost]
        public IActionResult Post ([FromBody] User value) {
            _context.Users.Add (value);
            _context.SaveChanges ();
            return NoContent ();
            //return CreatedAtRoute("User", new { id = value.Id }, value);
        }

        // PUT api/users/5
        [HttpPut ("{id}")]
        public IActionResult Put (int id, [FromBody] User value) {
            var user = _context.Users.Find (id);
            if (user == null) {
                return NotFound ();
            }

            user.Email = value.Email;
            user.Username = value.Username;

            _context.Users.Update (user);
            _context.SaveChanges ();
            return NoContent ();
        }

        // DELETE api/users/5
        [HttpDelete ("{id}")]
        public void Delete (int id) { }
    }
}