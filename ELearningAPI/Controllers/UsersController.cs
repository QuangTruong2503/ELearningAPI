using ELearningAPI.Data;
using ELearningAPI.Helpers;
using ELearningAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Buffers;

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
        public async Task<IActionResult> Get(int page = 1, int pageSize = 10, string? search = null)
        {
            //Lấy dữ liệu Users mà không có hashed_passowrd
            var users = await _context.Users.ToListAsync();
            if (!string.IsNullOrEmpty(search))
            {
                users = users.Where(u => u.email.Contains(search) || u.first_name.Contains(search) || u.last_name.Contains(search)).ToList();
            }

            //Phân trang
            var usersCount = users.Count();
            users = users.Skip((page - 1) * pageSize).Take(pageSize).ToList();
            var totalPages = (int)Math.Ceiling(usersCount / (double)pageSize);
            var currentPage = page;
            return Ok(new
            {
                searchValues = search,
                pages = totalPages,
                current = currentPage,
                data = users
            });
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
            return Ok(new
            {
                data = detail
            });
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
            var data = _context.Users.FirstOrDefault(u => u.user_id == user.user_id);
            // Kiểm tra xem User có tồn tại không
            if (data == null)
            {
                return NotFound(new
                {
                    message = $"Không tìm thấy người dùng với ID: {user.user_id}",
                    isSuccess = false
                });
            }
            try
            {
                data.user_id = user.user_id;
                data.user_name = user.user_name;
                data.email = user.email;
                data.first_name = user.first_name;
                data.last_name = user.last_name;
                data.role_id = user.role_id;
                // Lưu thay đổi vào cơ sở dữ liệu
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                // Kiểm tra xem User có tồn tại không
                if (!UserExists(user.user_id))
                {
                    return NotFound( new
                    {
                        message = $"Không tìm thấy người dùng với ID: {user.user_id}",
                        isSuccess = false
                    });
                }
                else
                {
                    throw;
                }
            }
            return Ok(new
            {
                message = "Cập nhật dữ liệu người dùng thành công!",
                isSuccess = true
            });
        }

        // DELETE api/<UsersController>/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return BadRequest(new
                {
                    success = false,
                    message = $"Không tìm thấy thông tin tài khoản: {id}"
                });
            }

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                success = true,
                message = $"Xóa tài khoản: {id} thành công!"
            });
        }

        private bool UserExists(Guid id)
        {
            return _context.Users.Any(e => e.user_id == id);
        }
    }
}
