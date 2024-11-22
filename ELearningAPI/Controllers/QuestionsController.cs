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

        // POST api/<QuestionsController>
        //Thêm dữ liệu câu hỏi và các câu trả lời tương ứng với ID câu hỏi
        [HttpPost]
        public async Task<IActionResult> AddQuestions([FromBody] List<QuestionsRequest> questionRequests)
        {
            if (questionRequests == null || questionRequests.Count == 0)
            {
                return BadRequest("Không có dữ liệu");
            }
            foreach (var questionRequest in questionRequests)
            {
                // Create new Question entity
                var question = new QuestionsModel
                {
                    question_id = new Guid(),
                    question_text = questionRequest.QuestionText,
                    scores = questionRequest.Scores,
                    exam_id = questionRequest.ExamId
                };
                // Add question and related options to context
                _context.Questions.Add(question);
            }
            await _context.SaveChangesAsync();
            return Ok("Thêm dữ liệu câu hỏi thành công.");
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
