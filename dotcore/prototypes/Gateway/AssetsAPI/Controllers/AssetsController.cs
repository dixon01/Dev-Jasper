using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace AssetsAPI.Controllers
{
    [Route("api/assets")]
    [ApiController]
    public class AssetsController : ControllerBase
    {
        // GET api/assets
        [HttpGet,Route("")]
        public ActionResult<IEnumerable<string>> Get()
        {
            return new string[] { "Asset 1", "Asset 2" };
        }

        // GET api/asset/5
        [HttpGet("{id}")]
        public ActionResult<string> Get(int id)
        {
            return "Asset " + id.ToString();
        }

        // POST api/asset
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/asset/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/asset/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
