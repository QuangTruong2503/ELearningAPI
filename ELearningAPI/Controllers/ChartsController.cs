using ELearningAPI.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ELearningAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChartsController : ControllerBase
    {
        private readonly ELearningDbContext _context;
        public ChartsController(ELearningDbContext context)
        {
            _context = context;
        }
        // GET: api/<ChartsController>
        [HttpGet("user-register-by-months")]
        public async Task<IActionResult> GetUserRegisterByMonth()
        {
            // Khởi tạo danh sách để lưu dữ liệu cho từng tháng
            var monthlyData = new List<object>();

            for (int month = 1; month <= 12; month++)
            {
                // Đếm số lượng người dùng đăng ký trong tháng
                var userCount = await _context.Users
                    .Where(u => u.created_at.HasValue && u.created_at.Value.Month == month)
                    .CountAsync();

                // Thêm dữ liệu vào danh sách
                monthlyData.Add(new
                {
                    Label = $"Tháng {month}",
                    Data = userCount
                });
            }

            // Trả về danh sách dữ liệu
            return Ok(monthlyData);
        }


        // GET api/<ChartsController>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<ChartsController>
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/<ChartsController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<ChartsController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
