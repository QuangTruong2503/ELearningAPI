using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace ELearningAPI.Models
{
    public class AnswersModel
    {
        [Key]
        public int answer_id { get; set; }

        [ForeignKey("submission")]
        public Guid submission_id { get; set; }

        [ForeignKey("question")]
        public int question_id { get; set; }

        [ForeignKey("option")]
        public int selected_option_id { get; set; }

        public  SubmissionsModel? Submissions { get; set; }
        public  QuestionsModel? Questions { get; set; }
        public  OptionsModel? Options { get; set; }
    }
}
