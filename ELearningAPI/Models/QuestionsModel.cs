﻿using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace ELearningAPI.Models
{
    public class QuestionsModel
    {
        [Key]
        public int question_id { get; set; }

        [ForeignKey("exam")]
        public Guid exam_id { get; set; }

        [Required]
        public required string question_text { get; set; }

        [Required]
        public float scores { get; set; }

        // List of Options associated with this question
        public List<OptionsModel> options { get; set; } = new List<OptionsModel>();
        public  ExamsModel? Exams { get; set; }
    }
}
