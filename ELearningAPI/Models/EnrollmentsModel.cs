using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace ELearningAPI.Models
{
    public class EnrollmentsModel
    {
        [Key]
        public int enrollment_id { get; set; }

        [ForeignKey("course")]
        public Guid course_id { get; set; }

        [ForeignKey("user")]
        public Guid student_id { get; set; }

        public DateTime enrolled_at { get; set; } = DateTime.UtcNow;
    }
}
