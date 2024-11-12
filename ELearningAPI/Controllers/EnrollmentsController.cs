using ELearningAPI.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ELearningAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EnrollmentsController : ControllerBase
    {
        private readonly ELearningDbContext _context;
        public EnrollmentsController(ELearningDbContext context)
        {
            _context = context;
        }
        // Kiểm tra người dùng đã tham gia khóa học hay chưa
        [HttpGet]
        public async Task<IActionResult> Get(Guid? courseID = null, Guid? userID = null)
        {

            try
            {
                if (courseID != null && userID != null)
                {
                    var result = await _context.Enrollments.FirstOrDefaultAsync(e => e.student_id == userID && e.course_id == courseID);
                    if (result == null)
                    {
                        return Ok(new
                        {
                            success = false,
                            message = "Bạn chưa tham gia khóa học này"
                        });
                    }
                    return Ok(new
                    {
                        success = true,
                        message = "Bạn đã tham gia khóa học này"
                    });
                }
                else
                {
                    return BadRequest(new
                    {
                        success = false,
                        message = "Thiếu dữ liệu courseID hoặc userID"
                    });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        // GET api/<EnrollmentsController>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<EnrollmentsController>
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/<EnrollmentsController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<EnrollmentsController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
