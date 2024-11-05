using ELearningAPI.Data;
using ELearningAPI.DataTransferObject;
using ELearningAPI.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;


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
        private readonly IConfiguration _configuration;
        public LoginController(ELearningDbContext context, IConfiguration configuration)
        {       
            _context = context;
            _configuration = configuration;
            //Lấy dữ liệu TokenKey từ biến môi trường
            string? tokenScretKey = Environment.GetEnvironmentVariable("JWT_SECRET_KEY");
            if (string.IsNullOrEmpty(tokenScretKey))
            {
                Console.WriteLine("Không có khóa token được sử dụng trong LoginController");
                tokenScretKey =  _configuration.GetValue<string>("TokenSecretKey");
            }
            _tokenServices = new TokenServices(tokenScretKey);
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
                data = new
                {
                    UserName = user.user_name,
                    Email = user.email,
                    FirstName = user.first_name,
                    LastName = user.last_name,
                    CreateAt = user.created_at,
                    avatar = user.avatar_url,
                    token = _tokenServices.GenerateToken(user.user_id.ToString(), user.role_id)
                }
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
                    success = false,
                });
            }
            return Ok(new
            {
                success = true,
                message = "Token hợp lệ",
                data = decode
            });
        }
    
    }

}
