using ELearningAPI.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ELearningAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CoursesController : ControllerBase
    {
        private readonly ELearningDbContext _context;
        public CoursesController(ELearningDbContext context)
        {
            _context = context;
        }
        // GET: api/<CoursesController>
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var courses = await _context.Courses.ToListAsync();
            return Ok(courses);
        }

        // GET api/<CoursesController>/5
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(Guid id)
        {
            var detail =  await _context.Courses.FirstOrDefaultAsync(c => c.course_id == id);
            return Ok(detail);
        }

        // POST api/<CoursesController>
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/<CoursesController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<CoursesController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
