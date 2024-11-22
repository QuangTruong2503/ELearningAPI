using ELearningAPI.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ELearningAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ExamsController : ControllerBase
    {
        private readonly ELearningDbContext _context;
        public ExamsController(ELearningDbContext context)
        {
            _context = context;
        }
        // Lấy dữ liệu các bài thi theo Course
        [HttpGet("by-course/{courseID}")]
        public async Task<IActionResult> Get(Guid courseID)
        {
            try
            {
                var exams = await _context.Exams.Where(e => e.course_id == courseID).ToListAsync();
                return Ok(exams);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
            
        }

        // Lấy dữ liệu theo ExamID
        [HttpGet("{id}")]
        public async Task<IActionResult> GetByExamID(Guid id)
        {
            try
            {
                var exam = await _context.Exams.FirstOrDefaultAsync(e => e.exam_id == id);
                return Ok(exam);
            }
            catch (Exception ex)
            {
                return BadRequest("Lỗi khi truy vấn dữ liệu bài thi" + ex.Message);
            }
        }

        // POST api/<ExamsController>
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/<ExamsController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<ExamsController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
