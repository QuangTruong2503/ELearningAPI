using System.ComponentModel.DataAnnotations;

namespace ELearningAPI.Models
{
    public class LessonLinksModel
    {
        [Key]
        public Guid Link_ID { get; set; }

        public required string Link_Name { get; set; }

        public required string Link_URL { get; set; }

        public Guid Lesson_ID { get; set; }

        public DateTimeOffset Created_At { get; set; }
    }
}
