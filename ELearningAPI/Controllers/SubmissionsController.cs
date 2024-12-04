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
        // Lấy thông tin bài làm
        [HttpGet("by-submissionID")]
        public async Task<IActionResult> GetSubmissionByID(Guid submissionID, Guid userID)
        {
            var data = await _context.Submissions.FirstOrDefaultAsync(s => s.submission_id == submissionID);
            if (data == null)
            {
                return Ok(new
                {
                    success = false,
                    message = "Không tìm thấy khóa học"
                });
            }
            if (data.student_id != userID)
            {
                return Ok(new
                {
                    success = false,
                    message = "Bạn không phải là người tạo bài thi này"
                });
            }
            return Ok(new
            {
                data = data,
                success = true
            });
        }

        [HttpGet("by-exam-user")]
        public async Task<IActionResult> GetByExamAndUser(Guid examID, Guid userID)
        {
            var submission = await _context.Submissions.FirstOrDefaultAsync(s => s.exam_id == examID && s.student_id == userID);
            if (submission == null)
            {
                return Ok(new
                {
                    data = Empty,
                    success = false,
                });
            }
            return Ok(new
            {
                data = submission,
                success = true
            });
        }

        //Lấy dữ liệu bài thi và dữ liệu câu hỏi với kết quả đúng
        [HttpGet("answer-and-questions")]
        public async Task<IActionResult> GetAnswerAndQuestions(Guid submissionID, Guid userID)
        {
            // Tìm bài nộp
            var result = await _context.Submissions
                .Where(s => s.submission_id == submissionID)
                .Join(_context.Exams, s => s.exam_id, e => e.exam_id, (s, e) => new
                {
                    submission = s,
                    exam = e
                })
                .Join(_context.Courses, se => se.exam.course_id, c => c.course_id, (se, c) => new 
                {
                    submission = se.submission, exam = se.exam, course = c
                })
                .FirstOrDefaultAsync();
            var user = await _context.Users.FirstOrDefaultAsync(u => u.user_id == userID);
            if (result == null || user == null)
            {
                return Ok(new { success = false, message = "Không tìm thấy dữ liệu." });
            }

            // Kiểm tra người dùng có phải là người tạo bài thi không
            if (result.submission.student_id != user.user_id && result.course.teacher_id != user.user_id && user.role_id != "admin")
            {
                return Ok(new { success = false, message = "Bạn không được truy cập vào kết quả bài làm này." });
            }

            // Lấy dữ liệu câu hỏi và bài làm của sinh viên trong một truy vấn
            var questionsWithAnswers = await (from q in _context.Questions
                                              join a in _context.Answers on q.question_id equals a.question_id into answersGroup
                                              where q.exam_id == result.submission.exam_id
                                              select new
                                              {
                                                  q.question_id,
                                                  q.question_text,
                                                  q.scores,
                                                  options = q.Options.Select(option => new
                                                  {
                                                      option.option_id,
                                                      option.option_text,
                                                      option.is_correct,
                                                  }).ToList(),
                                                  answers = answersGroup
                                                      .Where(ans => ans.submission_id == submissionID)
                                                      .Select(ans => new { ans.answer_id, ans.selected_option_id }).FirstOrDefault(),
                                              }).ToListAsync();

            // Kiểm tra không có câu hỏi hoặc bài làm
            if (!questionsWithAnswers.Any())
            {
                return Ok(new { success = false, message = "Không tìm thấy câu hỏi hoặc bài làm." });
            }
            var exam = await _context.Exams.FirstOrDefaultAsync(e => e.exam_id == result.submission.exam_id);
            if (exam == null)
            {
                return BadRequest(new
                {
                    success = false,
                    message = "Không tìm thấy thông tin bài thi"
                });
            }
            // Trả về dữ liệu
            return Ok(new
            {
                success = true,
                questionsData = questionsWithAnswers.Select(q => new
                {
                    q.question_id,
                    q.question_text,
                    q.scores,
                    options = q.options,
                    answersData = q.answers
                }),
                exam = new
                {
                    exam.exam_id,
                    exam.exam_name,
                    exam.total_score,
                },
                user = new
                {
                    user.first_name,
                    user.last_name,
                },
                submission = new
                {
                    result.submission.started_at,
                    result.submission.submitted_at,
                    result.submission.scores
                }
            });
        }


        [HttpPost]
        public async Task<IActionResult> CreateSubmission([FromBody] SubmissionsModel model)
        {
            var submissions = await _context.Submissions.FirstOrDefaultAsync(s => s.student_id == model.student_id && s.exam_id == model.exam_id);
            if (submissions != null)
            {
                if (submissions.submitted_at != null)
                {
                    return Ok(new
                    {
                        success = false,
                        message = "Bạn đã hoàn thành bài kiểm tra này."
                    });
                }
                    return Ok(new
                    {
                        isTesting = true,
                        submissionID = submissions.submission_id
                    });
            };
            var exam = await _context.Exams.FirstOrDefaultAsync(e => e.exam_id == model.exam_id);
            if (exam != null && exam.finished_at.UtcDateTime < DateTime.UtcNow )
            {
                return Ok(new
                {
                    success = false,
                    message = "Bài kiểm tra này đã quá hạn. Bạn không thể tham gia!"
                });
            }
            var newData = new SubmissionsModel()
            {
                submission_id = Guid.NewGuid(),
                student_id = model.student_id,
                exam_id = model.exam_id,
                started_at = model.started_at,
                scores = 0
            };
            _context.Submissions.Add(newData);
            await _context.SaveChangesAsync();
            return Ok(new
            {
                success = true,
                message = "Bạn có thể tiến hành kiểm tra.",
                submissionID = newData.submission_id
            });
        }
        [HttpPost("insert-questions-options")]
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
        [HttpPost("insert-answers")]
        public async Task<IActionResult> InsertAnswer(
            [FromBody] List<QuestionsRequestDTO.QuestionRequest> questionRequests,
            Guid submissionID)
        {
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    // Kiểm tra nếu sinh viên đã nộp bài kiểm tra
                    if (await _context.Submissions.AnyAsync(s => s.submission_id == submissionID && s.submitted_at != null))
                    {
                        return Ok(new
                        {
                            success = false,
                            message = "Bạn đã hoàn thành bài kiểm tra này."
                        });
                    }

                    // Kiểm tra tính hợp lệ của các câu hỏi
                    if (questionRequests.Any(q => q.examId != _context.Submissions.Where(s => s.submission_id == submissionID).Select(s => s.exam_id).FirstOrDefault()))
                    {
                        return BadRequest(new
                        {
                            success = false,
                            message = "Có câu hỏi không thuộc bài kiểm tra này."
                        });
                    }
                    //Lấy dữ liệu submission
                    var submission = await _context.Submissions.FirstOrDefaultAsync(s => s.submission_id == submissionID);
                    if (submission == null)
                    {
                        return BadRequest(new
                        {
                            success = false,
                            message = "Bài làm không hợp lệ"
                        });
                    };
                    // Tạo Answers
                    var answers = questionRequests.Select(question =>
                    {
                        var selectedOption = question.options.FirstOrDefault(o => o.isCorrect);
                        return new AnswersModel
                        {
                            answer_id = Guid.NewGuid(),
                            submission_id = submissionID,
                            question_id = question.questionId,
                            selected_option_id = selectedOption?.optionId ?? Guid.Empty
                        };
                    }).ToList();

                    await _context.Answers.AddRangeAsync(answers);

                    // Tính tổng điểm cho bài làm
                    float totalScore = 0;
                    var correctOptions = await _context.Questions
                        .Select(question => new
                        {
                            QuestionId = question.question_id,
                            QuestionText = question.question_text,
                            Scores = question.scores,
                            ExamId = question.exam_id,
                            Options = question.Options.Where(o => o.is_correct == true)
                                .Select(option => new
                                {
                                    OptionId = option.option_id,
                                    OptionText = option.option_text,
                                    IsCorrect = option.is_correct
                                }).ToList()
                        })
                        .ToListAsync();
                    
                    foreach (var userQuestion in questionRequests)
                    {
                        //Tìm câu hỏi tương ứng trong bài làm với câu hỏi mẫu
                        var correctQuestion = correctOptions.FirstOrDefault(o => o.QuestionId == userQuestion.questionId);
                        if (correctQuestion != null)
                        {
                            // Lấy danh sách đáp án đúng
                            var correctOptionIds = correctQuestion.Options
                                .Where(o => o.IsCorrect)
                                .Select(o => o.OptionId)
                                .ToHashSet();
                            // Lấy danh sách đáp án mà người dùng gửi lên
                            var userOptionIds = userQuestion.options
                                .Where(o => o.isCorrect)
                                .Select(o => o.optionId)
                                .ToHashSet();
                            // Nếu các đáp án khớp nhau, cộng điểm
                            if (userOptionIds.SetEquals(correctOptionIds))
                            {
                                totalScore += correctQuestion.Scores;
                            }

                        }
                    }
                    //Cập nhật thời gian nộp bài
                    submission.scores = totalScore;
                    submission.submitted_at = DateTime.UtcNow;
                    _context.Submissions.Update(submission);

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
