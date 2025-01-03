using System.ComponentModel.DataAnnotations;

namespace ELearningAPI.Models
{
    public class AnswersModel
    {
        [Key]
        public Guid answer_id { get; set; }

        public Guid submission_id { get; set; }

        public Guid question_id { get; set; }

        public Guid selected_option_id { get; set; }

    }
}
