using ELearningAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace ELearningAPI.Data
{
    public class ELearningDbContext : DbContext
    {
        public ELearningDbContext(DbContextOptions<ELearningDbContext> options) : base(options)
        {
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
        //Khai báo Users
        public DbSet<UsersModel> Users { get; set; }

        //Khai báo Khóa học (Courses)
        public DbSet<CoursesModel> Courses { get; set; }
        public DbSet<LessonsModel> Lessons { get; set; }
        public DbSet<ExamsModel> Exams { get; set; }
        public DbSet<QuestionsModel> Questions { get; set; }
        public DbSet<OptionsModel> Options { get; set; }
        public DbSet<SubmissionsModel> Submissions { get; set; }
        public DbSet<AnswersModel> Answers { get; set; }
        public DbSet<EnrollmentsModel> Enrollments { get; set; }
        public DbSet<RolesModel> Roles { get; set; }

    }
}
