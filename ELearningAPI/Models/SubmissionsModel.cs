using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace ELearningAPI.Models
{
    public class SubmissionsModel
    {
        [Key]
        public Guid submission_id { get; set; }

        [ForeignKey("exam")]
        public Guid exam_id { get; set; }

        [ForeignKey("user")]
        public Guid student_id { get; set; }

        public DateTime started_at { get; set; }

        public DateTime submitted_at { get; set; }

        public float scores { get; set; }

    }
}
