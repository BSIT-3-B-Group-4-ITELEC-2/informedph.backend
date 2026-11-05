using informedph.backend.Models;
using Microsoft.EntityFrameworkCore;

namespace informedph.backend.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
       : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Test>().HasData(
                new Test
                {
                    Id = 1,
                    Message = "This is test message!"
                }
            );

            modelBuilder.Entity<Schedule>().HasData(
                new Schedule
                {
                    Id = 1,
                    ExecutionTime = new TimeSpan(6, 0, 0),
                    LastExecutedAt = null,
                    CreatedAt = new DateTime(2026, 11, 4, 0, 0, 0, DateTimeKind.Utc),
                    UpdatedAt = new DateTime(2026, 11, 4, 0, 0, 0, DateTimeKind.Utc)
                }
            );

            base.OnModelCreating(modelBuilder);
        }

        public DbSet<Test> Tests => Set<Test>();
        public DbSet<Subscriber> Subscribers => Set<Subscriber>();
        public DbSet<NewsArticle> NewsArticles => Set<NewsArticle>();
        public DbSet<Schedule> Schedules => Set<Schedule>();
    }
}
