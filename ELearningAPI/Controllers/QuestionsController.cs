using ELearningAPI.Data;
using Microsoft.AspNetCore.Mvc;
using ELearningAPI.DataTransferObject;
using ELearningAPI.Models;
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
        // GET: api/<QuestionsController>
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/<QuestionsController>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<QuestionsController>
        //Thêm dữ liệu câu hỏi và câu trả lời tương ứng với ID câu hỏi
        [HttpPost]
        public async Task<IActionResult> AddQuestions([FromBody] List<QuestionsRequest> questionRequests)
        {
            if(questionRequests == null || questionRequests.Count == 0)
            {
                return BadRequest("No data provided.");
            }
            foreach (var questionRequest in questionRequests)
            {
                // Create new Question entity
                var question = new QuestionsModel
                {
                    question_text = questionRequest.QuestionText,
                    scores = questionRequest.Scores,
                    exam_id = questionRequest.ExamId,
                    options = questionRequest.Options.Select(o => new OptionsModel
                    {
                        option_text = o.OptionText,
                        is_correct = o.IsCorrect
                    }).ToList()
                };

                // Add question and related options to context
                _context.Questions.Add(question);
            }
            await _context.SaveChangesAsync();
            return Ok("Data added successfully.");
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
