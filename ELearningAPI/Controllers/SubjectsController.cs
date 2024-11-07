using ELearningAPI.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ELearningAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SubjectsController : ControllerBase
    {
        private readonly ELearningDbContext _context;
        public SubjectsController(ELearningDbContext context)
        {
            _context = context;
        }

        // GET: api/<SubjectsController>
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            try
            {
                var subjects = await _context.Subjects.ToListAsync();
                return Ok(subjects);
            }
            catch (Exception ex)
            {
                return BadRequest("Có lỗi khi lấy dữ liệu Subjects: " + ex.Message);
            }
        }

        // GET api/<SubjectsController>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<SubjectsController>
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/<SubjectsController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<SubjectsController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
