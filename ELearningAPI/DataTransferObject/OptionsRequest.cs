namespace ELearningAPI.DataTransferObject
{
    public class OptionsRequest
    {
        public Guid OptionID { get; set; }
        public string OptionText { get; set; }  // ánh xạ với option_text
        public bool IsCorrect { get; set; }     // ánh xạ với is_correct
    }
}
