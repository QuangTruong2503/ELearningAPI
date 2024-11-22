namespace ELearningAPI.DataTransferObject
{
    public class QuestionsRequest
    {
        public Guid QuestionID { get; set; }
        public string QuestionText { get; set; }  // ánh xạ với question_text
        public int Scores { get; set; }           // ánh xạ với scores
        public Guid ExamId { get; set; }          // ánh xạ với exam_id
        public List<OptionsRequest> Options { get; set; }  // ánh xạ với danh sách options
    }
}
