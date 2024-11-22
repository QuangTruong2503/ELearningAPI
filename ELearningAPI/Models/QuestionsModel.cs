using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace ELearningAPI.Models
{
    public class QuestionsModel
    {
        [Key]
        public Guid question_id { get; set; }

        [ForeignKey("exam")]
        public Guid exam_id { get; set; }

        [Required]
        public required string question_text { get; set; }

        [Required]
        public float scores { get; set; }
    }
}
