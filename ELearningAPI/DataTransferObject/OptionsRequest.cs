namespace ELearningAPI.DataTransferObject
{
    public class OptionsRequest
    {
        public string OptionText { get; set; }  // ánh xạ với option_text
        public bool IsCorrect { get; set; }     // ánh xạ với is_correct
    }
}
