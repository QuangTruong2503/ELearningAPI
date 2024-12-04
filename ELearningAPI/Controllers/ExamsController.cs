using ELearningAPI.Data;
using ELearningAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.EntityFrameworkCore;
using Pomelo.EntityFrameworkCore.MySql.Query.Internal;

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
                var exams = await _context.Exams.Where(e => e.course_id == courseID).OrderBy(e => e.created_at).ToListAsync();
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

        //Kiểm tra dữ liệu người dùng đã tham gia khóa học chưa
        [HttpGet("check-user-in-course-by-exam")]
        public async Task<IActionResult> CheckUserInCourse(Guid userID, Guid examID)
        {
            var userInCourse = await _context.Enrollments
                    .AnyAsync(enroll => enroll.student_id == userID &&
                        _context.Exams.Any(exam => exam.exam_id == examID && exam.course_id == enroll.course_id));
            var courseID = await _context.Exams
                .Where(exam => exam.exam_id == examID)
                .Select(exam => exam.course_id)
                .FirstOrDefaultAsync();
            if (!userInCourse)
            {
                return Ok(new
                {
                    success = false,
                    message = "Bạn chưa tham gia khóa học",
                    courseID = courseID,
                });
            }
            return Ok(new
            {
                success = true,
                message = "Bạn đã ở trong khóa học"
            });
        }

        // Thêm mới
        [HttpPost("create-new")]
        public async Task<IActionResult> CreateNew(Guid courseID)
        {
            try
            {
                var exam = new ExamsModel()
                {
                    exam_id = Guid.NewGuid(),
                    exam_name = "Bài thi mới",
                    created_at = DateTime.UtcNow,
                    exam_time = 60,
                    finished_at = DateTime.UtcNow.AddDays(30),
                    hide_result = false,
                    total_score = 100,
                    course_id = courseID,
                };
                _context.Exams.Add(exam);
                await _context.SaveChangesAsync();
                return Ok(new
                {
                    message = "Tạo mới bài thi thành công"
                });
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        // PUT api/<ExamsController>/5
        [HttpPut("update-exam")]
        public async Task<IActionResult> Put(ExamsModel model)
        {
            var exam = await _context.Exams.AnyAsync(e => e.exam_id == model.exam_id);
            if (!exam)
            {
                return Ok(new
                {
                    success = false,
                    message = "Không tồn tại bài thi này."
                });
            }
            _context.Exams.Update(model);
            await _context.SaveChangesAsync();
            return Ok(new
            {
                success = true,
                message = "Cập nhật dữ liệu bài thi thành công"
            });
        }

        // DELETE api/<ExamsController>/5
        [HttpDelete()]
        public async Task<IActionResult> Delete(Guid examID)
        {
            var exam = await _context.Exams.FirstOrDefaultAsync(e => e.exam_id == examID);
            if (exam == null)
            {
                return Ok(new
                {
                    success= false,
                    message = "Không tìm thấy bài thi"
                });
            }
            _context.Exams.Remove(exam);
            await _context.SaveChangesAsync();
            return Ok(new
            {
                success = true,
                message = $"Xóa bài thi: {exam.exam_name} thành công."
            });
        }
    }
}
