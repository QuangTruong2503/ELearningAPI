using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ELearningAPI.Models
{
    public class LessonsModel
    {
        [Key]
        public Guid Lesson_ID { get; set; }

        public required string Lesson_Name { get; set; }

        [ForeignKey("course_id")]
        public Guid Course_ID { get; set; }

        public DateTime Created_At { get; set; }
    }
}