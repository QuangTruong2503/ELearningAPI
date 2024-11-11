using ELearningAPI.Data;
using ELearningAPI.Helpers;
using ELearningAPI.Models;
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
        public ActionResult Get(int page = 1, int pageSize = 10, string? searchName = null)
        {
            var query = _context.Courses.AsQueryable();
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
            if (!string.IsNullOrEmpty(searchName))
            {
                results = results.Where(c => c.CourseName.Contains(searchName));
            }
            //Phân trang
            var count = results.Count();
            var totalPages = (int)Math.Ceiling(count / (double)pageSize);
            results = results.Skip((page - 1) * pageSize).Take(pageSize);
            var currentPage = page;
            return Ok(new
            {
                totalPages = totalPages,
                currentPage = currentPage,
                data = results
            });
        }

        //Lấy dữ liệu Khóa học theo trạng thái public
        [HttpGet("public/{status}")]
        public ActionResult GetCoursesPublic(bool status)
        {
            var results = from course in _context.Courses
                          join teacher in _context.Users
                          on course.teacher_id equals teacher.user_id
                          join subject in _context.Subjects
                          on course.subject_id equals subject.subject_id
                          where course.is_public == status
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


        //Lấy dữ liệu Course theo trạng thái public và SubjectID
        [HttpGet("get/bySubject")]
        public ActionResult GetCourses([FromQuery] bool? isPublic, [FromQuery] string subject)
        {
            var query = _context.Courses.AsQueryable();

            if (isPublic.HasValue)
            {
                query = query.Where(course => course.is_public == isPublic.Value);
            }

            if (!string.IsNullOrEmpty(subject))
            {
                query = query.Where(course => course.subject_id == subject);
            }

            var results = query
                .Join(_context.Users,
                      course => course.teacher_id,
                      teacher => teacher.user_id,
                      (course, teacher) => new { course, teacher })
                .Select(ct => new
                {
                    CourseID = ct.course.course_id,
                    CourseName = ct.course.course_name,
                    Description = ct.course.description,
                    InviteCode = ct.course.invite_code,
                    IsPublic = ct.course.is_public,
                    CreatedAt = ct.course.created_at,
                    Thumbnail = ct.course.thumbnail,
                    TeacherID = ct.teacher.user_id,
                    TeacherFullName = $"{ct.teacher.first_name} {ct.teacher.last_name}"
                });

            return Ok(results);
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
        public async Task<IActionResult> Post(CoursesModel model)
        {
            try
            {
                string newCode;
                bool codeExists;
                do
                {
                    newCode = GetRandomCode.GetVerificationCode();
                    codeExists = await _context.Courses.AnyAsync(c => c.invite_code == newCode);
                } while (codeExists);
                //Khai báo dữ liệu
                model.course_id = new Guid();
                model.invite_code = newCode;
                model.created_at = DateTime.UtcNow; 

                //Thêm dữ liệu
                _context.Courses.Add(model);
                await _context.SaveChangesAsync();
                return Ok(new
                {
                    success = true,
                    message = "Tạo khóa học thành công!"
                });
            }
            catch (Exception ex)
            {
                return BadRequest("Gặp lỗi khi thêm khóa học: " + ex.Message);
            }
            
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
