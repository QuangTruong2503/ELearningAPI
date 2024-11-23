using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Microsoft.CodeAnalysis.Options;

namespace ELearningAPI.Models
{
    public class QuestionsModel
    {
        [Key]
        public Guid question_id { get; set; }

        public Guid exam_id { get; set; }

        [Required]
        public required string question_text { get; set; }

        [Required]
        public float scores { get; set; }
        // Navigation property for the foreign key to Exams
        [ForeignKey("exam_id")]
        public virtual ExamsModel Exams { get; set; } // Assuming there is an Exam model

        // Navigation property for the related options
        public virtual ICollection<OptionsModel> Options { get; set; }
    }
}
