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
                       Message="This is test message!"
                   }
               );

            base.OnModelCreating(modelBuilder);
        }

        public DbSet<Test> Tests => Set<Test>();
        public DbSet<Subscriber> Subscribers => Set<Subscriber>();
        public DbSet<NewsArticle> NewsArticles => Set<NewsArticle>();
    }
}
