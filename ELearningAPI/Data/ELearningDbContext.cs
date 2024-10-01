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
        public DbSet<ExamsModel> exams { get; set; }
        public DbSet<QuestionsModel> questions { get; set; }
        public DbSet<OptionsModel> options { get; set; }
        public DbSet<SubmissionsModel> submissions { get; set; }
        public DbSet<AnswersModel> answers { get; set; }
        public DbSet<EnrollmentsModel> enrollments { get; set; }
    }
}
