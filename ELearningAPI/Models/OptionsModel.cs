using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace ELearningAPI.Models
{
    public class OptionsModel
    {
        [Key]
        public int option_id { get; set; }

        [ForeignKey("question")]
        public int question_id { get; set; }

        [Required]
        [MaxLength(255)]
        public required string option_text { get; set; }

        [Required]
        public bool is_correct { get; set; }

        public  QuestionsModel? question { get; set; }

    }
}
