using informedph.backend;
using informedph.backend.Data;
using Microsoft.EntityFrameworkCore;

namespace informedph.backend
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            var connectionString = builder.Configuration
                .GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException("Database connection string is missing.");

            builder.Services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseMySQL(connectionString);
            });

            // Add HttpClient Factory
            builder.Services.AddHttpClient();

            // Register NewsAggregator as Hosted Background Service
            builder.Services.AddHostedService<NewsAggregator>();

            builder.Services.AddControllers();
            builder.Services.AddOpenApi();

            var app = builder.Build();

            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
            }

            app.UseHttpsRedirection();
            app.UseAuthorization();
            app.MapControllers();

            app.Run();
        }
    }
}