using System.ComponentModel.DataAnnotations;

namespace ELearningAPI.Models
{
    public class LessonLinksModel
    {
        [Key]
        public int Link_ID { get; set; }

        public required string Link_Name { get; set; }

        public required string Link_URL { get; set; }

        public int Lesson_ID { get; set; }
    }
}
