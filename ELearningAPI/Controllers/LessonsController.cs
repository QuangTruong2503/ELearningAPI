using ELearningAPI.Data;
using ELearningAPI.DataTransferObject;
using ELearningAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ELearningAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LessonsController : ControllerBase
    {
        private readonly ELearningDbContext _context;
        public LessonsController(ELearningDbContext context)
        {
            _context = context;
        }
        //Lấy các bài học theo Course
        [HttpGet("by-course/{courseID}")]
        public async Task<IActionResult> Get(Guid courseID)
        {
            try
            {
                var result = new List<object>();
                var lessons = await _context.Lessons.Where(l => l.Course_ID == courseID).OrderBy(l => l.Created_At).ToListAsync();
                foreach (var lesson in lessons)
                {
                    var leslink = await _context.Lesson_Links.Where(l => l.Lesson_ID == lesson.Lesson_ID).OrderBy(l => l.Created_At).ToListAsync();
                    var data = new
                    {
                        lessonData = lesson,
                        lessonLink = leslink
                    };
                    result.Add(data);
                }
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // GET api/<LessonsController>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<LessonsController>
        [HttpPost("lessons-and-lessonlinks")]
        public async Task<IActionResult> UpsertLesson([FromBody] List<LessonRequestDto> lessonRequests)
        {
            if (lessonRequests == null || !lessonRequests.Any())
                return BadRequest("Request data is empty.");

            // Lấy danh sách Lesson_ID từ JSON
            var lessonIdsFromJson = lessonRequests.Select(l => l.LessonData.Lesson_ID).ToList();

            // Lấy danh sách tất cả Lesson_ID trong DB
            var existingLessonIds = await _context.Lessons.Select(l => l.Lesson_ID).ToListAsync();

            // Tìm các Lesson_ID cần xóa (có trong DB nhưng không có trong JSON)
            var lessonsToDelete = existingLessonIds.Except(lessonIdsFromJson).ToList();

            // Xóa bài học và các liên kết liên quan
            foreach (var lessonId in lessonsToDelete)
            {
                var lessonLinks = _context.Lesson_Links.Where(link => link.Lesson_ID == lessonId);
                _context.Lesson_Links.RemoveRange(lessonLinks);

                var lesson = await _context.Lessons.FindAsync(lessonId);
                if (lesson != null)
                {
                    _context.Lessons.Remove(lesson);
                }
            }

            // Xử lý thêm mới hoặc cập nhật các bài học
            foreach (var lessonRequest in lessonRequests)
            {
                // Xử lý dữ liệu Lesson
                var lesson = await _context.Lessons
                    .FirstOrDefaultAsync(l => l.Lesson_ID == lessonRequest.LessonData.Lesson_ID);

                if (lesson == null)
                {
                    // Nếu không tồn tại, thêm mới
                    lesson = new LessonsModel
                    {
                        Lesson_ID = new Guid(),
                        Lesson_Name = lessonRequest.LessonData.Lesson_Name,
                        Course_ID = lessonRequest.LessonData.Course_ID,
                        Created_At = DateTime.UtcNow
                    };
                    await _context.Lessons.AddAsync(lesson);
                }
                else
                {
                    // Nếu đã tồn tại, cập nhật
                    lesson.Lesson_Name = lessonRequest.LessonData.Lesson_Name;
                    lesson.Course_ID = lessonRequest.LessonData.Course_ID;
                    _context.Lessons.Update(lesson);
                }

                // Xử lý dữ liệu LessonLinks
                var linkIdsFromJson = lessonRequest.LessonLink.Select(link => link.Link_ID).ToList();

                // Lấy danh sách Link_ID hiện có trong DB cho bài học này
                var existingLinkIds = await _context.Lesson_Links
                    .Where(link => link.Lesson_ID == lesson.Lesson_ID)
                    .Select(link => link.Link_ID)
                    .ToListAsync();

                // Tìm các Link_ID cần xóa
                var linksToDelete = existingLinkIds.Except(linkIdsFromJson).ToList();
                foreach (var linkId in linksToDelete)
                {
                    var link = await _context.Lesson_Links.FindAsync(linkId);
                    if (link != null)
                    {
                        _context.Lesson_Links.Remove(link);
                    }
                }

                // Thêm mới hoặc cập nhật các liên kết từ JSON
                foreach (var link in lessonRequest.LessonLink)
                {
                    var existingLink = await _context.Lesson_Links
                        .FirstOrDefaultAsync(l => l.Link_ID == link.Link_ID);

                    if (existingLink == null)
                    {
                        // Thêm mới liên kết nếu không tồn tại
                        var newLink = new LessonLinksModel
                        {
                            Link_ID = new Guid(),
                            Link_Name = link.Link_Name,
                            Link_URL = link.Link_URL,
                            Lesson_ID = lesson.Lesson_ID, // Liên kết với bài học hiện tại
                            Created_At = DateTime.UtcNow,
                        };
                        await _context.Lesson_Links.AddAsync(newLink);
                    }
                    else
                    {
                        // Cập nhật liên kết nếu đã tồn tại
                        existingLink.Link_Name = link.Link_Name;
                        existingLink.Link_URL = link.Link_URL;
                        _context.Lesson_Links.Update(existingLink);
                    }
                }
            }

            // Lưu thay đổi vào DB
            await _context.SaveChangesAsync();

            return Ok("Lưu thay đổi thành công.");
        }
    

        // PUT api/<LessonsController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<LessonsController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
