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
        //Khai báo dịch vụ token
        private readonly TokenServices _tokenServices;
        public LoginController(ELearningDbContext context)
        {
            _context = context;
            _tokenServices = new TokenServices("23f9dc32-e9ee-4f39-b1dd-040a6b69ac21");
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
                UserName = user.user_name,
                Email = user.email,
                FirstName = user.first_name,
                LastName = user.last_name,
                CreateAt = user.created_at,
                token = _tokenServices.GenerateToken(user.user_id.ToString(), user.role_id)
            };
            // Nếu đăng nhập thành công
            return Ok(results);
        }

        //Kiểm tra token
        [HttpGet("token/{token}")]
        public IActionResult CheckToken(string token)
        {
            var decode = _tokenServices.DecodeToken(token);
            if (decode == null)
            {
                return Ok(new
                {
                    message = "Token không hợp lệ",
                    isLogin = false,
                });
            }
            return Ok(new
            {
                isLogin = true,
                message = "Token hợp lệ",
                data = decode.Claims.Select(c => c.ToString()).ToArray()
            });
        }
    
    }

}
