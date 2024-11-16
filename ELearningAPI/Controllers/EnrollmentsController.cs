using ELearningAPI.Data;
using ELearningAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


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
        [HttpGet("user-in-course")]
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

        // Lấy dữ liệu Courses theo userID đã tham gia
        [HttpGet("by-user")]
        public async Task<IActionResult> Get(Guid userID, int page = 1, int pageSize = 8, string? search = null)
        {
            try
            {
                // Lấy danh sách các khóa học của sinh viên bằng cách join trực tiếp
                var query = from enroll in _context.Enrollments
                            join course in _context.Courses on enroll.course_id equals course.course_id
                            where enroll.student_id == userID
                            select course;

                // Lọc theo từ khóa nếu có
                if (!string.IsNullOrEmpty(search))
                {
                    query = query.Where(c => c.course_name.Contains(search));
                }

                // Tổng số khóa học
                var count = await query.CountAsync();

                // Phân trang và lấy dữ liệu
                var courses = await query
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                // Tính tổng số trang
                var totalPages = (int)Math.Ceiling(count / (double)pageSize);

                // Trả kết quả
                return Ok(new
                {
                    currentPage = page,
                    totalPages = totalPages,
                    Data = courses
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }


        // Thêm dữ liệu tham gia khóa học
        [HttpPost]
        public async Task<IActionResult> Post(Guid userID, Guid courseID)
        {

            try
            {
                var enroll = await _context.Enrollments.FirstOrDefaultAsync(e => e.course_id == courseID || e.student_id == userID);
                if (enroll != null)
                {
                    return Ok(new
                    {
                        success = false,
                        message = "Bạn đã tham gia khóa học!"
                    });
                }
                EnrollmentsModel model = new EnrollmentsModel()
                {
                    student_id = userID,
                    course_id = courseID,
                    enrolled_at = DateTime.UtcNow,
                };
                _context.Enrollments.Add(model);
                await _context.SaveChangesAsync();
                return Ok(new
                {
                    success = true,
                    message = "Tham gia khóa học thành công!"
                });
            }
            catch (Exception ex)
            {
                return BadRequest("Error: " + ex.Message);
            }
        }
        //Thêm dữ liệu tham gia khóa học thông qua mã mời
        [HttpPost("join-by-invite")]
        public async Task<IActionResult> EnrollByInviteCode(string inviteCode, Guid userID)
        {
            // Check for course existence by invite code
            var course = await _context.Courses
                .Where(c => c.invite_code == inviteCode)
                .Select(c => new { c.course_id, c.course_name })
                .FirstOrDefaultAsync();

            if (course == null)
            {
                return Ok(new
                {
                    success = false,
                    message = $"Mã mời: '{inviteCode}' không tồn tại."
                });
            }

            // Check for user existence by user ID
            var userExists = await _context.Users.AnyAsync(u => u.user_id == userID);
            if (!userExists)
            {
                return Ok(new
                {
                    success = false,
                    message = $"User with ID '{userID}' does not exist."
                });
            }

            // Check if the user is already enrolled in the course
            var alreadyEnrolled = await _context.Enrollments
                .AnyAsync(e => e.course_id == course.course_id && e.student_id == userID);

            if (alreadyEnrolled)
            {
                return Ok(new
                {
                    success = false,
                    message = $"Bạn đã tham gia khóa học: {course.course_name}"
                });
            }

            // Enroll the user in the course
            var enrollment = new EnrollmentsModel
            {
                course_id = course.course_id,
                student_id = userID,
                enrolled_at = DateTime.UtcNow,
            };
            await _context.Enrollments.AddAsync(enrollment);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                success = true,
                message = $"Tham gia khóa học thành công: {course.course_name}"
            });
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
