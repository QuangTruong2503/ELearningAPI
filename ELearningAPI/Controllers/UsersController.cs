using ELearningAPI.Data;
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
            var users = await _context.Users.ToListAsync();
            return Ok(users);
        }

        // GET api/<UsersController>/5
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(Guid id)
        {
            var detail = await _context.Users.FirstOrDefaultAsync(c => c.user_id == id);
            if (detail == null)
            {
                return NotFound($"Không tìm thấy thông tin tài khoản: {id}");
            }
            return Ok(detail);
        }

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
                _context.Users.Add(users);
                await _context.SaveChangesAsync();
                return Ok(new { message = "Insert into News successfully.", data = users });
            }
            catch (DbUpdateException ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
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
