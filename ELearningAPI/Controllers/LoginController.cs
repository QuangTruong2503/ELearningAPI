using ELearningAPI.Data;
using ELearningAPI.DataTransferObject;
using ELearningAPI.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ELearningAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private readonly ELearningDbContext _context;
        public LoginController(ELearningDbContext context)
        {
            _context = context;
        }
        // GET: api/<LoginController>
        [HttpGet]
        public  ActionResult Get()
        {
            
            return Ok();
        }

        // GET api/<LoginController>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<LoginController>
        [HttpPost]
        public async Task<IActionResult> Login([FromBody] LoginRequest loginRequest)
        {
            // Tìm user trong cơ sở dữ liệu theo tên đăng nhập hoặc email
            var user = await _context.Users
                .FirstOrDefaultAsync(x => x.user_name == loginRequest.UserNameOrEmail || x.email == loginRequest.UserNameOrEmail);

            if (user == null)
            {
                return Ok(new
                {
                    message = "Tài khoản hoặc Email không tồn tại",
                    isLogin = false
                });
            }

            // Xác thực mật khẩu
            bool isPasswordValid = PasswordHasher.VerifyPassword(loginRequest.Password, user.hashed_password);
            if (!isPasswordValid)
            {
                return Ok(new
                {
                    message = "Mật khẩu không đúng!",
                    isLogin = false
                });
            }
            var results = new
            {
                message = "Đăng nhập thành công",
                isLogin = true,
                UserID = user.user_id,
                UserName = user.user_name,
                Email = user.email,
                UserRole = user.user_role
            };
            // Nếu đăng nhập thành công
            return Ok(results);
        }

        // PUT api/<LoginController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<LoginController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
