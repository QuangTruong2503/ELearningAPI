using ELearningAPI.Models;

namespace ELearningAPI.DataTransferObject
{
    public class LessonRequestDto
    {
        public LessonsModel LessonData { get; set; }
        public List<LessonLinksModel> LessonLink { get; set; } = new List<LessonLinksModel>();
    }
}
