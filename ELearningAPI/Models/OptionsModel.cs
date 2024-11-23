using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace ELearningAPI.Models
{
    public class OptionsModel
    {
        [Key]
        public Guid option_id { get; set; }

        public Guid question_id { get; set; }

        [Required]
        [MaxLength(255)]
        public required string option_text { get; set; }

        [Required]
        public bool is_correct { get; set; }

        [ForeignKey("question_id")]
        public virtual QuestionsModel Questions { get; set; }
    }
}
