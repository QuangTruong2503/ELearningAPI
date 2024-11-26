namespace ELearningAPI.DataTransferObject
{
    public class QuestionsRequestDTO
    {
        public class QuestionRequest
        {
            public Guid questionId { get; set; }
            public string questionText { get; set; }
            public float scores { get; set; }
            public Guid examId { get; set; }
            public List<OptionRequest> options { get; set; }
        }

        public class OptionRequest
        {
            public Guid optionId { get; set; }
            public string optionText { get; set; }
            public bool isCorrect { get; set; }
        }
    }
}
