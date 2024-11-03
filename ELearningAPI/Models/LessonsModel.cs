using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ELearningAPI.Models
{
    public class LessonsModel
    {
        [Key]
        public int Lesson_ID { get; set; }

        public required string Lesson_Name { get; set; }

        [ForeignKey("course_id")]
        public Guid Course_ID { get; set; }

        public DateTime Created_At { get; set; } = DateTime.Now;

        public required string Lesson_URL { get; set; }
    }
}