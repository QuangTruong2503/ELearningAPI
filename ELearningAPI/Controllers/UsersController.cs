using ELearningAPI.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ELearningAPI.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly ELearningDbContext _context;
        public UsersController(ELearningDbContext context)
        {
            _context = context;
        }
        // GET: api/<UsersController>
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var users = await _context.Users.ToListAsync();
            return Ok(users);
        }

        // GET api/<UsersController>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<UsersController>
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/<UsersController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<UsersController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
