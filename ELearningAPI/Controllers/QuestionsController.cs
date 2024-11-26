using ELearningAPI.Data;
using Microsoft.AspNetCore.Mvc;
using ELearningAPI.DataTransferObject;
using ELearningAPI.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.CodeAnalysis.Options;
// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ELearningAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class QuestionsController : ControllerBase
    {
        private readonly ELearningDbContext _context;
        public QuestionsController(ELearningDbContext context)
        {
            _context = context;
        }
        [HttpGet("Auto-update-ID")]
        public async Task<IActionResult> GetAll()
        {

            return Ok();
        }

        // GET: api/<QuestionsController>
        [HttpGet]
        public async Task<IActionResult> GetQuestionsWithOptions()
        {
            var results = await _context.Questions
                .Select(question => new
                {
                    QuestionId = question.question_id, // Sử dụng tên thuộc tính từ model
                    QuestionText = question.question_text,
                    Scores = question.scores,
                    ExamId = question.exam_id,
                    Options = question.Options.Select(option => new
                    {
                        OptionId = option.option_id,
                        OptionText = option.option_text,
                        IsCorrect = option.is_correct
                    }).ToList()
                })
                .ToListAsync();

            if (results == null || !results.Any())
            {
                return NotFound(new { Message = "No questions found." });
            }

            return Ok(results);
        }


        // GET api/<QuestionsController>/5
        [HttpGet("by-examID")]
        public async Task<IActionResult> GetQuestionByExamID(Guid id)
        {
            var results = await _context.Questions
                .Where(q => q.exam_id == id)
                .Select(question => new
                {
                    QuestionId = question.question_id,
                    QuestionText = question.question_text,
                    Scores = question.scores,
                    ExamId = question.exam_id,
                    Options = question.Options
                        .Select(option => new
                        {
                            OptionId = option.option_id,
                            OptionText = option.option_text,
                            IsCorrect = option.is_correct
                        }).ToList()
                })
                .ToListAsync();

            return Ok(results);
        }
        //Lấy dữ liệu câu hỏi cho bài làm của học sinh
        [HttpGet("get/without-correct-option")]
        public async Task<IActionResult> GetQuestionNoCorrectOption(Guid examID)
        {
            var result = await _context.Questions.Where(q => q.exam_id == examID).Select(q => new
            {
                QuestionId = q.question_id,
                QuestionText = q.question_text,
                Scores = q.scores,
                ExamId = q.exam_id,
                Options = q.Options.Select(o => new {
                    OptionId = o.option_id,
                    OptionText = o.option_text,
                    IsCorrect = false
                }).ToList()
            }).ToListAsync();
            return Ok(result);
        }
        //Thêm dữ liệu câu hỏi và các câu trả lời tương ứng với ID câu hỏi
        [HttpPost("upsert-questions-and-options")]
        public async Task<IActionResult> UpsertQuestion([FromBody] List<QuestionsRequestDTO.QuestionRequest> questionsRequest)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Lấy danh sách tất cả các câu hỏi hiện có trong database
            var existingQuestions = await _context.Questions
                .Include(q => q.Options) // Bao gồm các tùy chọn liên quan
                .ToListAsync();

            // Lấy danh sách các questionId được truyền vào
            var incomingQuestionIds = questionsRequest.Select(q => q.questionId).ToList();

            // Xóa các câu hỏi trong database không có trong danh sách questionId được truyền vào
            var questionsToDelete = existingQuestions
                .Where(eq => !incomingQuestionIds.Contains(eq.question_id))
                .ToList();

            if (questionsToDelete.Any())
            {
                _context.Questions.RemoveRange(questionsToDelete);
            }

            // Xử lý thêm/cập nhật câu hỏi và các tùy chọn
            foreach (var request in questionsRequest)
            {
                // Kiểm tra xem câu hỏi đã tồn tại trong database hay chưa
                var existingQuestion = existingQuestions
                    .FirstOrDefault(q => q.question_id == request.questionId);

                if (existingQuestion != null)
                {
                    // Cập nhật thông tin câu hỏi
                    existingQuestion.exam_id = request.examId;
                    existingQuestion.question_text = request.questionText;
                    existingQuestion.scores = request.scores;

                    // Xử lý các tùy chọn (Options)
                    var incomingOptions = request.options.Select(o => new
                    {
                        o.optionId,
                        o.optionText,
                        o.isCorrect
                    }).ToList();

                    // Xóa các tùy chọn không còn tồn tại
                    var optionsToDelete = existingQuestion.Options
                        .Where(o => !incomingOptions.Any(io => io.optionId == o.option_id))
                        .ToList();
                    _context.Options.RemoveRange(optionsToDelete);

                    // Cập nhật hoặc thêm các tùy chọn
                    foreach (var option in incomingOptions)
                    {
                        var existingOption = existingQuestion.Options
                            .FirstOrDefault(o => o.option_id == option.optionId);

                        if (existingOption != null)
                        {
                            // Cập nhật tùy chọn
                            existingOption.option_text = option.optionText;
                            existingOption.is_correct = option.isCorrect;
                        }
                        else
                        {
                            // Thêm mới tùy chọn
                            var newOption = new OptionsModel
                            {
                                option_id = option.optionId,
                                question_id = existingQuestion.question_id,
                                option_text = option.optionText,
                                is_correct = option.isCorrect
                            };
                            _context.Options.Add(newOption);
                        }
                    }
                }
                else
                {
                    // Thêm mới câu hỏi
                    var newQuestion = new QuestionsModel
                    {
                        question_id = request.questionId,
                        exam_id = request.examId,
                        question_text = request.questionText,
                        scores = request.scores,
                    };

                    var newOptions = request.options.Select(option => new OptionsModel
                    {
                        option_id = option.optionId,
                        question_id = newQuestion.question_id,
                        option_text = option.optionText,
                        is_correct = option.isCorrect
                    }).ToList();

                    _context.Questions.Add(newQuestion);
                    _context.Options.AddRange(newOptions);
                }
            }

            await _context.SaveChangesAsync();

            return Ok(new
            {
                success = true,
                message = "Cập nhật dữ liệu các câu hỏi thành công."
            });
        }



        // PUT api/<QuestionsController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<QuestionsController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }

    
}
