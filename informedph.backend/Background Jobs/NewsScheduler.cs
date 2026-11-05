using informedph.backend.Clients;
using informedph.backend.Data;
using informedph.backend.DTO;
using informedph.backend.Renderers;
using informedph.backend.View_Models;
using Microsoft.EntityFrameworkCore;

namespace informedph.backend.Background_Jobs
{
    public class NewsScheduler : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<NewsScheduler> _logger;

        public NewsScheduler(
            IServiceScopeFactory scopeFactory,
            ILogger<NewsScheduler> logger)
        {
            _scopeFactory = scopeFactory;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(
            CancellationToken stoppingToken)
        {
            using var timer = new PeriodicTimer(
                TimeSpan.FromSeconds(15)
            );

            _logger.LogInformation("News scheduler has been executed");

            while (await timer.WaitForNextTickAsync(stoppingToken))
            {
                try
                {
                    await CheckScheduleAsync(stoppingToken);
                }
                catch (OperationCanceledException)
                    when (stoppingToken.IsCancellationRequested)
                {
                    break;
                }
                catch (Exception ex)
                {
                    _logger.LogError(
                        ex,
                        "An error has occurred while checking the schedule."
                    );
                }
            }
        }

        private async Task CheckScheduleAsync(
            CancellationToken cancellationToken)
        {
            using var scope = _scopeFactory.CreateScope();

            var context = scope.ServiceProvider
                .GetRequiredService<ApplicationDbContext>();

            var schedule = await context.Schedules
                .FirstOrDefaultAsync(
                    s => s.Id == 1,
                    cancellationToken
                );

            if (schedule == null)
                return;

            var philippineTime = TimeZoneInfo.ConvertTimeFromUtc(
                DateTime.UtcNow,
                TimeZoneInfo.FindSystemTimeZoneById("Asia/Manila")
            );

            var currentTime = philippineTime.TimeOfDay;

            if (currentTime.Hours != schedule.ExecutionTime.Hours ||
                currentTime.Minutes != schedule.ExecutionTime.Minutes)
            {
                return;
            }

            if (schedule.LastExecutedAt?.Date == DateTime.UtcNow.Date)
                return;

            schedule.LastExecutedAt = DateTime.UtcNow.Date;
            await context.SaveChangesAsync(cancellationToken);

            _logger.LogInformation(
                "Daily news schedule has been triggered at {Time}",
                philippineTime
            );

            // Disabled Temporarily for frontend API integration
            //await ExecuteNewsletterDistributorAsync(
            //    scope.ServiceProvider, 
            //    context, 
            //    cancellationToken
            //);
        }

        private async Task ExecuteNewsletterDistributorAsync(
            IServiceProvider scopedServices,
            ApplicationDbContext context,
            CancellationToken cancellationToken)
        {
            var httpClientFactory = scopedServices
                .GetRequiredService<IHttpClientFactory>();

            var templateRenderer = scopedServices
                .GetRequiredService<NewsletterTemplateRenderer>();

            var emailClient = scopedServices
                .GetRequiredService<ResendEmailClient>();

            var client = httpClientFactory.CreateClient("NewsApi");

            var subscribers = await context.Subscribers
              .Where(s => s.email != null && s.email != "")
              .ToListAsync(cancellationToken);

            if (subscribers.Count == 0)
            {
                _logger.LogWarning(
                    "No subscribers found. Newsletter delivery skipped."
                );

                return;
            }

            var response = await client.GetFromJsonAsync<NewsApiResponse>
            (
                "news/",
                cancellationToken
            );

            if (response == null || response.Results.Count == 0)
            {
                _logger.LogWarning(
                    "No processed articles returned. Newsletter delivery skipped."
                );

                return;
            }

            var manilaTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Asia/Manila");

            var newsletterModel = new NewsletterViewModel
            {
                NewsletterDate = DateTimeOffset.Now,
                Articles = response.Results
            };

            var htmlBody = await templateRenderer
                .RenderNewsletterTemplateAsync(newsletterModel);

            foreach (var subscriber in subscribers)
            {
                cancellationToken.ThrowIfCancellationRequested();

                try
                {
                    await emailClient.SendAsync(
                        subscriber.email,
                        "InformedPH",
                        htmlBody
                    );

                    _logger.LogInformation(
                        "Newsletter sent to {Email}.",
                        subscriber.email
                    );
                }
                catch (Exception ex)
                {
                    _logger.LogError(
                        ex,
                        "Failed to send newsletter to {Email}.",
                        subscriber.email
                    );
                }
            } 
        }
    }
}
