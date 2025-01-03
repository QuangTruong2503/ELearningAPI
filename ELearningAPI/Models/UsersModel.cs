using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ELearningAPI.Models
{
    public class UsersModel
    {
        [Key]
        public Guid user_id { get; set; }

        public  string? user_name { get; set; }

        public string? last_name { get; set; }

        public string? first_name { get; set; }

        public string? email { get; set; }

        public string? hashed_password { get; set; }

        public DateTimeOffset? created_at { get; set; }

        public string? avatar_url { get; set; }

        public string? role_id { get; set; }

    }
}
