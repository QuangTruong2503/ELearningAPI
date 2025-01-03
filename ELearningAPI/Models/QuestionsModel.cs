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

        public  string question_text { get; set; }

        public float scores { get; set; }

        // Navigation property for the related options
        public virtual ICollection<OptionsModel> Options { get; set; }
    }
}
