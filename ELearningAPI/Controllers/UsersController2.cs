//using Microsoft.AspNetCore.Mvc;
//using Microsoft.EntityFrameworkCore;
//using TestAPI.Data;
//using TestAPI.Models;

//// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

//namespace TestAPI.Controllers
//{
//    [Route("api/[controller]")]
//    [ApiController]
//    public class UsersController : ControllerBase
//    {
//        private readonly TestDBContext _dbContext;
//        public UsersController(TestDBContext dbContext)
//        {
//            _dbContext = dbContext;
//        }

//        // GET: api/<UsersController>
//        [HttpGet]
//        public async Task<IActionResult> Get()
//        {
//            var users = await _dbContext.users.ToListAsync();
//            return Ok(users);
//        }

//        // GET api/<UsersController>/5
//        [HttpGet("{id}")]
//        public string Get(int id)
//        {
//            return "value";
//        }

//        // POST api/<UsersController>
//        [HttpPost]
//        public async Task<IActionResult> Post([FromBody] users user)
//        {
//            if (user == null)
//            {
//                return BadRequest("Không có dữ liệu");
//            }
//            try
//            {

//                _dbContext.users.Add(user);
//                await _dbContext.SaveChangesAsync();
//                return Ok(new { message = "Insert into News successfully.", user });
//            }
//            catch (DbUpdateException ex)
//            {
//                return StatusCode(500, $"Internal server error: {ex.Message}");
//            }
//        }

//        // PUT api/<UsersController>/5
//        [HttpPut("{id}")]
//        public async Task<IActionResult> Put(int id, [FromBody] users user)
//        {
//            // Kiểm tra nếu ID từ URL và UserId từ body khác nhau
//            if (id != user.user_id)
//            {
//                return BadRequest();
//            }
//            // Đánh dấu trạng thái của đối tượng user cần cập nhật
//            _dbContext.Entry(user).State = EntityState.Modified;
//            try
//            {
//                // Lưu thay đổi vào cơ sở dữ liệu
//                await _dbContext.SaveChangesAsync();
//            }
//            catch (DbUpdateConcurrencyException)
//            {
//                // Kiểm tra xem User có tồn tại không
//                if (!UserExists(id))
//                {
//                    return NotFound($"User with ID {id} not found.");
//                }
//                else
//                {
//                    throw;
//                }
//            }
//            return Ok("User updated successfully");
//        }

//        // DELETE api/<UsersController>/5
//        [HttpDelete("{id}")]
//        public async Task<IActionResult> Delete(int id)
//        {
//            var deleteID = await _dbContext.users.FindAsync(id);
//            if (deleteID != null)
//            {
//                _dbContext.Remove(deleteID);
//                await _dbContext.SaveChangesAsync();
//                return Ok(new { message = $"Delete successful ID: {id}" });
//            }
//            return BadRequest($"Do not find News by id = {id}");
//        }
//        private bool UserExists(int id)
//        {
//            return _dbContext.users.Any(e => e.user_id == id);
//        }
//    }
//}
