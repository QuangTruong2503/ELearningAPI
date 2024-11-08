using System.ComponentModel.DataAnnotations;

namespace ELearningAPI.Models
{
    public class SubjectsModel
    {
        [Key]
        public required string subject_id { get; set; }

        public required string subject_name { get; set; }
    }
}
