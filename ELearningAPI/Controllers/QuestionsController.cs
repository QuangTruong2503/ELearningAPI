using ELearningAPI.Data;
using Microsoft.AspNetCore.Mvc;
using ELearningAPI.DataTransferObject;
using ELearningAPI.Models;
using Microsoft.EntityFrameworkCore;
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
        public async Task<IActionResult> AutoUpdateID()
        {
            var questions = await _context.Questions.ToListAsync();
            return Ok(questions);
        }

        // GET: api/<QuestionsController>
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var results = from question in _context.Questions
                          join option in _context.Options
                          on question.question_id equals option.question_id into optionGroup
                          select new
                          {
                              QuestionId = question.question_id,
                              QuestionText = question.question_text,
                              Scores = question.scores,
                              ExamId = question.exam_id,
                              Options = optionGroup.Select(o => new
                              {
                                  OptionId = o.option_id,
                                  OptionText = o.option_text,
                                  IsCorrect = o.is_correct
                              }).ToList()
                          };

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
                    Options = _context.Options
                        .Where(option => option.question_id == question.question_id)
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

        //Thêm dữ liệu câu hỏi và các câu trả lời tương ứng với ID câu hỏi
        [HttpPut]
        public async Task<IActionResult> UpsertQuestion([FromBody] QuestionRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var question = new QuestionsModel
            {
                question_id = Guid.NewGuid(),
                exam_id = request.examId,
                question_text = request.questionText,
                scores = request.scores,
            };

            var options = request.options.Select(option => new OptionsModel
            {
                option_id = Guid.NewGuid(),
                question_id = question.question_id,
                option_text = option.optionText,
                is_correct = option.isCorrect
            }).ToList();

            _context.Questions.Add(question);
            _context.Options.AddRange(options);

            await _context.SaveChangesAsync();

            return Ok(new
            {
                success = true,
                message = "Cập nhật dữ liệu bài thi thành công."
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

    public class QuestionRequest
    {
        public Guid questionId { get; set; }
        public string questionText { get; set; }
        public float scores { get; set; }
        public Guid examId { get; set; }
        public List<OptionRequest> options { get; set; }
    }

    public class OptionRequest
    {
        public Guid optionId { get; set; }
        public string optionText { get; set; }
        public bool isCorrect { get; set; }
    }
}
