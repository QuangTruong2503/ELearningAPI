using System.ComponentModel.DataAnnotations;

namespace ELearningAPI.Models
{
    public class RolesModel
    {
        [Key]
        public required string role_id { get; set; }

        public required string role_name { get; set; }
    }
}
