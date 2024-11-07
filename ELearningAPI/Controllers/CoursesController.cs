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
            var results = from course in _context.Courses
                          join teacher in _context.Users
                          on course.teacher_id equals teacher.user_id
                          join subject in _context.Subjects
                          on course.subject_id equals subject.subject_id
                          select new
                          {
                              CourseID = course.course_id,
                              CourseName = course.course_name,
                              Description = course.description,
                              InviteCode = course.invite_code,
                              IsPublic = course.is_public,
                              CreatedAt = course.created_at,
                              Thumbnail = course.thumbnail,
                              TeacherID = teacher.user_id,
                              TeacherFullName = $"{teacher.first_name} {teacher.last_name}",
                              SubjectID = subject.subject_id,
                              SubjectName = subject.subject_name,
                          };
            return Ok(results);
        }

        //Lấy dữ liệu Khóa học public
        [HttpGet("courses/public")]
        public async Task<IActionResult> GetCoursesPublic()
        {
            var coursesPublic = await _context.Courses.Where(c => c.is_public == true).ToListAsync();
            return Ok(coursesPublic);
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
