using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace ResourcesAPI.Controllers
{
    [Route("api/resources")]
    [ApiController]
    public class ResourcesController : ControllerBase
    {
        // GET api/resources
        [HttpGet, Route("")]
        public ActionResult<IEnumerable<string>> Get()
        {
            return new string[] { "Resource 1", "Resource 2" };
        }

        // GET api/resources/5
        [HttpGet("{id}")]
        public ActionResult<string> Get(int id)
        {
            return "value";
        }

        // POST api/resources
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/resources/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/resources/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
