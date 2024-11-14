using ELearningAPI.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ELearningAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LessonsController : ControllerBase
    {
        private readonly ELearningDbContext _context;
        public LessonsController(ELearningDbContext context)
        {
            _context = context;
        }
        //Lấy các bài học theo Course
        [HttpGet("by-course/{courseID}")]
        public async Task<IActionResult> Get(Guid courseID)
        {
            try
            {
                var lessons = await _context.Lessons.Where(l => l.Course_ID == courseID).ToListAsync();
                return Ok(lessons);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // GET api/<LessonsController>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<LessonsController>
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/<LessonsController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<LessonsController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
