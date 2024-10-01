using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace ELearningAPI.Models
{
    public class CoursesModel
    {
        [Key]
        public Guid course_id { get; set; }

        [Required]
        [MaxLength(255)]
        public string course_name { get; set; }

        public string description { get; set; }

        [ForeignKey("user")]
        public Guid teacher_id { get; set; }

        public DateTime created_at { get; set; } = DateTime.UtcNow;
    }
}
