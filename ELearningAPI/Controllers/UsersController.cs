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
    public class UsersController : ControllerBase
    {
        private readonly ELearningDbContext _context;
        public UsersController(ELearningDbContext context)
        {
            _context = context;
        }
        // GET: api/<UsersController>
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            //Lấy dữ liệu Users mà không có hashed_passowrd
            var users = await _context.Users.ToListAsync();
            return Ok(users);
        }

        // GET api/<UsersController>/5
        
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(Guid id)
        {
            var detail = await _context.Users.Select(u => new
            {
                u.user_id,
                u.user_name,
                u.email,
                u.created_at,
                u.first_name,
                u.last_name,
                u.role_id
            }).FirstOrDefaultAsync(c => c.user_id == id);
            if (detail == null)
            {
                return NotFound($"Không tìm thấy thông tin tài khoản: {id}");
            }
            return Ok(detail);
        }
        
        //Tạo tài khoản mới
        // POST api/<UsersController>
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] UsersModel users)
        {
            if (users == null)
            {
                return BadRequest("Không có dữ liệu");
            }
            try
            {
                users.user_id = Guid.NewGuid();
                users.hashed_password = PasswordHasher.HashPassword(users.hashed_password);
                users.created_at = DateTime.UtcNow;
                _context.Users.Add(users);
                await _context.SaveChangesAsync();
                return Ok(new { message = "Tạo tài khoản mới thành công.", isSuccess = true, data = users });
            }
            catch (DbUpdateException ex)
            {
                return BadRequest($"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("check-user-and-email/username={username}&email={email}")]
        public async Task<IActionResult> CheckAccount(string username, string email)
        {
            if(string.IsNullOrEmpty(username) || string.IsNullOrEmpty(email))
            {
                return BadRequest("Vui lòng nhập đầy đủ thông tin tài khoản và email.");
            }
            try
            {
                var users = await _context.Users.FirstOrDefaultAsync(u => u.user_name == username || u.email == email);
                if (users == null)
                {
                    return Ok(new
                    {
                        message = "Thông tin tài khoản và email hợp lệ.",
                        isSuccess = true
                    });
                }
                else
                {

                    if(users.user_name == username && users.email == email)
                    {
                        return Ok(new
                        {
                            message = "Thông tin tài khoản và email đã tồn tại.",
                            isSuccess = false
                        });
                    }
                    else if (users.user_name == username)
                    {
                        return Ok(new
                        {
                            message = "Tên tài khoản đã tồn tại. Vui lòng chọn tên khác",
                            isSuccess = false
                        });
                    }
                    return Ok(new
                    {
                        message = "Tên email đã tồn tại. Vui lòng chọn email khác",
                        isSuccess = false
                    });
                }
            }
            catch (Exception ex)
            {
                return BadRequest("Gặp lỗi khi kiểm tra thông tin tài khoản và email: " + ex.Message);
            }

        }
        // PUT api/<UsersController>/5
        [HttpPut]
        public async Task<IActionResult> Put( [FromBody] UsersModel user)
        {
            // Kiểm tra xem User có tồn tại không
            if (!UserExists(user.user_id))
            {
                return NotFound($"Không tìm thấy người dùng với ID: {user.user_id}");
            }
            // Đánh dấu trạng thái của đối tượng user cần cập nhật
            _context.Entry(user).State = EntityState.Modified;
            try
            {
                // Lưu thay đổi vào cơ sở dữ liệu
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                // Kiểm tra xem User có tồn tại không
                if (!UserExists(user.user_id))
                {
                    return NotFound($"Không tìm thấy người dùng với ID: {user.user_id}");
                }
                else
                {
                    throw;
                }
            }
            return Ok("Cập nhật dữ liệu người dùng thành công!");
        }

        // DELETE api/<UsersController>/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var deleteID = await _context.Users.FindAsync(id);
            if (deleteID != null)
            {
                _context.Users.Remove(deleteID);
                await _context.SaveChangesAsync();
                return Ok(new { message = $"Xóa tài khoản: {id} thành công!" });
            }
            return BadRequest($"Không tìm thấy thông tin tài khoản: {id}");
        }
        private bool UserExists(Guid id)
        {
            return _context.Users.Any(e => e.user_id == id);
        }
    }
}
