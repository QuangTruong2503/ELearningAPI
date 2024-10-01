using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ELearningAPI.Models
{
    public class UsersModel
    {
        [Key]
        public Guid user_id { get; set; }

        [Required]
        [MaxLength(255)]
        public required string user_name { get; set; }

        [MaxLength(45)]
        public string? last_name { get; set; }

        [MaxLength(45)]
        public string? first_name { get; set; }

        [MaxLength(255)]
        public string? email { get; set; }

        [Required]
        [MaxLength(255)]
        public required string hashed_password { get; set; }

        [Required]
        [Column(TypeName = "varchar(20)")]
        public UserRole user_role { get; set; } = UserRole.student;

        public DateTime created_at { get; set; } = DateTime.UtcNow;
    }
    public enum UserRole
    {
        teacher,
        student
    }
}
