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
        [HttpPost("update/lessons-and-lessonlinks")]
        public async Task<IActionResult> UpsertLesson([FromBody] List<LessonRequestDto> lessonRequests)
        {
            if (lessonRequests == null || !lessonRequests.Any())
                return BadRequest("Request data is empty.");

            // Danh sách Lesson_ID từ JSON
            var lessonIdsFromJson = lessonRequests.Select(l => l.LessonData.Lesson_ID).ToList();

            // Lấy tất cả các bài học hiện có trong DB
            var existingLessons = await _context.Lessons.ToListAsync();
            var existingLessonIds = existingLessons.Select(l => l.Lesson_ID).ToHashSet();

            // Xóa các bài học không có trong JSON
            var lessonsToDelete = existingLessons.Where(l => !lessonIdsFromJson.Contains(l.Lesson_ID)).ToList();
            _context.Lessons.RemoveRange(lessonsToDelete);

            // Chuẩn bị danh sách thêm mới hoặc cập nhật bài học
            var lessonsToAdd = new List<LessonsModel>();
            var lessonsToUpdate = new List<LessonsModel>();

            foreach (var lessonRequest in lessonRequests)
            {
                var lessonData = lessonRequest.LessonData;

                // Kiểm tra bài học đã tồn tại chưa
                var existingLesson = existingLessons.FirstOrDefault(l => l.Lesson_ID == lessonData.Lesson_ID);
                if (existingLesson == null)
                {
                    // Thêm mới bài học
                    lessonsToAdd.Add(new LessonsModel
                    {
                        Lesson_Name = lessonData.Lesson_Name,
                        Course_ID = lessonData.Course_ID,
                        Created_At = DateTime.UtcNow
                    });
                }
                else
                {
                    // Cập nhật bài học
                    existingLesson.Lesson_Name = lessonData.Lesson_Name;
                    existingLesson.Course_ID = lessonData.Course_ID;
                    lessonsToUpdate.Add(existingLesson);
                }
            }

            // Áp dụng thêm mới và cập nhật bài học
            if (lessonsToAdd.Any())
                await _context.Lessons.AddRangeAsync(lessonsToAdd);

            if (lessonsToUpdate.Any())
                _context.Lessons.UpdateRange(lessonsToUpdate);

            // Lưu bài học trước khi xử lý liên kết
            await _context.SaveChangesAsync();

            // Lấy danh sách tất cả các bài học mới (bao gồm cả bài học vừa thêm)
            var allLessons = await _context.Lessons.ToListAsync();
            var allLessonIds = allLessons.Select(l => l.Lesson_ID).ToHashSet();

            // Lấy danh sách liên kết hiện có trong DB
            var existingLinks = await _context.Lesson_Links
                .Where(link => allLessonIds.Contains(link.Lesson_ID))
                .ToListAsync();

            // Chuẩn bị danh sách liên kết cần xử lý
            var linksToDelete = new List<LessonLinksModel>();
            var linksToAdd = new List<LessonLinksModel>();
            var linksToUpdate = new List<LessonLinksModel>();

            foreach (var lessonRequest in lessonRequests)
            {
                var lessonData = lessonRequest.LessonData;

                // Tìm bài học sau khi đảm bảo nó đã được lưu
                var lesson = allLessons.FirstOrDefault(l => l.Lesson_Name == lessonData.Lesson_Name);

                if (lesson == null)
                    return BadRequest($"Không thể tìm thấy bài học: {lessonData.Lesson_Name}");

                var linkIdsFromJson = lessonRequest.LessonLink.Select(link => link.Link_ID).ToHashSet();
                var existingLinksForLesson = existingLinks.Where(link => link.Lesson_ID == lesson.Lesson_ID).ToList();

                // Tìm liên kết cần xóa
                linksToDelete.AddRange(existingLinksForLesson.Where(link => !linkIdsFromJson.Contains(link.Link_ID)));

                foreach (var link in lessonRequest.LessonLink)
                {
                    var existingLink = existingLinksForLesson.FirstOrDefault(l => l.Link_ID == link.Link_ID);
                    if (existingLink == null)
                    {
                        // Thêm mới liên kết
                        linksToAdd.Add(new LessonLinksModel
                        {
                            Link_ID = link.Link_ID,
                            Link_Name = link.Link_Name,
                            Link_URL = link.Link_URL,
                            Lesson_ID = lesson.Lesson_ID,
                            Created_At = DateTime.UtcNow
                        });
                    }
                    else
                    {
                        // Cập nhật liên kết
                        existingLink.Link_Name = link.Link_Name;
                        existingLink.Link_URL = link.Link_URL;
                        linksToUpdate.Add(existingLink);
                    }
                }
            }

            // Áp dụng thêm mới, cập nhật và xóa liên kết
            if (linksToDelete.Any())
                _context.Lesson_Links.RemoveRange(linksToDelete);

            if (linksToAdd.Any())
                await _context.Lesson_Links.AddRangeAsync(linksToAdd);

            if (linksToUpdate.Any())
                _context.Lesson_Links.UpdateRange(linksToUpdate);

            // Lưu tất cả thay đổi vào DB
            await _context.SaveChangesAsync();

            return Ok(new { message = "Lưu thay đổi thành công." });
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
