using DotNetEnv;
using informedph.backend.Background_Jobs;
using informedph.backend.Clients;
using informedph.backend.Data;
using informedph.backend.Renderers;
using Microsoft.EntityFrameworkCore;
using Resend;

namespace informedph.backend
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Env.Load();

            var builder = WebApplication.CreateBuilder(args);

            var connectionString = builder.Configuration
                .GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException("Database connection string is missing.");

            // Services
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseMySQL(connectionString);
            });

            builder.Services.AddResend(options =>
            {
                options.ApiToken =
                    Environment.GetEnvironmentVariable("RESEND_API_KEY")
                    ?? throw new InvalidOperationException(
                        "RESEND_API_KEY is not configured."
                    );
            });

            builder.Services.AddScoped<ResendEmailClient>();
            builder.Services.AddScoped<NewsletterTemplateRenderer>();

            // Add HttpClient Factory
            builder.Services.AddHttpClient();
            builder.Services.AddHttpClient("NewsApi", client =>
            {
                client.BaseAddress = new Uri("http://127.0.0.1:8000/");
                client.Timeout = TimeSpan.FromMinutes(5);
            });

            // Background Jobs Services
            builder.Services.AddHostedService<NewsAggregator>();
            builder.Services.AddHostedService<NewsScheduler>();

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