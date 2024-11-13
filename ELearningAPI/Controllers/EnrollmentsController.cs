﻿using ELearningAPI.Data;
using ELearningAPI.Models;
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

        // GET api/<EnrollmentsController>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // Thêm dữ liệu tham gia khóa học
        [HttpPost]
        public async Task<IActionResult> Post(EnrollmentsModel model)
        {

            try
            {
                model.enrolled_at = DateTime.Now;
                _context.Add(model);
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
