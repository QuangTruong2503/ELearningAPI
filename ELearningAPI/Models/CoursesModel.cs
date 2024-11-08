using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace ELearningAPI.Models
{
    public class CoursesModel
    {
        [Key]
        public Guid course_id { get; set; }

        public required string course_name { get; set; }

        public string? description { get; set; }

        public required string invite_code { get; set; }

        public required bool is_public { get; set; }

        public DateTime created_at { get; set; }

        public string? thumbnail { get; set; }

        public Guid teacher_id { get; set; }

        public required string subject_id { get; set; }

    }
}
