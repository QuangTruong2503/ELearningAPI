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
    public class SubmissionsController : ControllerBase
    {
        private readonly ELearningDbContext _context;
        public SubmissionsController(ELearningDbContext context)
        {
            _context = context;
        }
        // GET: api/<SubmissionsController>
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/<SubmissionsController>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }
        [HttpPost]
        public async Task<IActionResult> Post(Guid questionID, Guid optionID, Guid submissionID)
        {
            var answer = new AnswersModel()
            {
                answer_id = Guid.NewGuid(),
                question_id = questionID,
                selected_option_id = optionID,
                submission_id =  submissionID,
            };
            _context.Answers.Add(answer);
            await _context.SaveChangesAsync();
            return Ok("Thêm dữ liệu thành công");
        }
        // Tạo bài làm của học sinh
        [HttpPost("create-submission")]
        public async Task<IActionResult> CreateSubmission(
            [FromBody] List<QuestionsRequestDTO.QuestionRequest> questionRequests,
            Guid examID,
            Guid studentID)
        {
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    // Kiểm tra nếu sinh viên đã nộp bài kiểm tra
                    if (await _context.Submissions.AnyAsync(s => s.exam_id == examID && s.student_id == studentID))
                    {
                        return Ok(new
                        {
                            success = false,
                            message = "Bạn đã hoàn thành bài kiểm tra này."
                        });
                    }

                    // Kiểm tra tính hợp lệ của các câu hỏi
                    if (questionRequests.Any(q => q.examId != examID))
                    {
                        return BadRequest(new
                        {
                            success = false,
                            message = "Có câu hỏi không thuộc bài kiểm tra này."
                        });
                    }

                    // Tạo Submission
                    var submission = new SubmissionsModel
                    {
                        submission_id = Guid.NewGuid(),
                        student_id = studentID,
                        exam_id = examID,
                        submitted_at = DateTime.UtcNow,
                        scores = 0
                    };
                    _context.Submissions.Add(submission);
                    await _context.SaveChangesAsync(); // Lưu Submission trước

                    // Tạo Answers
                    var answers = questionRequests.Select(question =>
                    {
                        var selectedOption = question.options.FirstOrDefault(o => o.isCorrect);
                        return new AnswersModel
                        {
                            answer_id = Guid.NewGuid(),
                            submission_id = submission.submission_id,
                            question_id = question.questionId,
                            selected_option_id = selectedOption?.optionId ?? Guid.Empty
                        };
                    }).ToList();

                    await _context.Answers.AddRangeAsync(answers);

                    // Tính điểm
                    submission.scores = questionRequests
                        .Where(q => q.options.Any(o => o.isCorrect))
                        .Sum(q => q.scores);

                    await _context.SaveChangesAsync(); // Lưu tất cả thay đổi

                    await transaction.CommitAsync(); // Cam kết giao dịch

                    return Ok(new
                    {
                        success = true,
                        message = "Nộp bài thành công!"
                    });
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync(); // Hủy giao dịch nếu gặp lỗi
                    return BadRequest(new
                    {
                        success = false,
                        message = "Gặp lỗi khi tạo bài làm.",
                        error = ex.Message
                    });
                }
            }
        }





        // PUT api/<SubmissionsController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<SubmissionsController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
