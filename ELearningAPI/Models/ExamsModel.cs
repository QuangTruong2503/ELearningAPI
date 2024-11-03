using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace ELearningAPI.Models
{
    public class ExamsModel
    {
        [Key]
        public Guid exam_id { get; set; }

        [Required]
        [MaxLength(255)]
        public required string exam_name { get; set; }

        [Required]
        public int total_score { get; set; }

        [Required]
        public int exam_time { get; set; }

        public bool hide_result { get; set; } = false;

        public DateTime created_at { get; set; } = DateTime.UtcNow;

        public DateTime? finished_at { get; set; }

        [ForeignKey("course")]
        public Guid course_id { get; set; }
    }
}
