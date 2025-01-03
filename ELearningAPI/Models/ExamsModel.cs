using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace ELearningAPI.Models
{
    public class ExamsModel
    {
        [Key]
        public Guid exam_id { get; set; }

        public  string exam_name { get; set; }

        public int total_score { get; set; }

        public int exam_time { get; set; } = 60;

        public bool hide_result { get; set; } = false;

        public DateTimeOffset created_at { get; set; }

        public DateTimeOffset finished_at { get; set; }

        public Guid course_id { get; set; }
    }
}
