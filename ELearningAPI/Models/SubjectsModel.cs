using System.ComponentModel.DataAnnotations;

namespace ELearningAPI.Models
{
    public class SubjectsModel
    {
        [Key]
        public int subject_id { get; set; }

        public string? subject_name { get; set; }
    }
}
