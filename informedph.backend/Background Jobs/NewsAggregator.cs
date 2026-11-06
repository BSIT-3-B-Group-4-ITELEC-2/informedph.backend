<<<<<<< Updated upstream
﻿using System.Xml;
=======
<<<<<<< HEAD:informedph.backend/NewsAggregator.cs
﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
=======
﻿using System.Xml;
>>>>>>> aa2f361bb04a52efe9b363255c80bbf1ef04fb69:informedph.backend/Background Jobs/NewsAggregator.cs
>>>>>>> Stashed changes
using informedph.backend.Data;
using informedph.backend.Models;
using Microsoft.EntityFrameworkCore;


namespace informedph.backend
{
    public class NewsAggregator : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<NewsAggregator> _logger;

        private readonly TimeSpan _fetchInterval = TimeSpan.FromMinutes(5);

        private readonly Dictionary<string, string> _categoryRssFeeds = new()
        {
            { "News", "https://data.gmanetwork.com/gno/rss/news/feed.xml" },
            { "Showbiz", "https://data.gmanetwork.com/gno/rss/showbiz/feed.xml" },
            { "Sports", "https://data.gmanetwork.com/gno/rss/sports/feed.xml" },
            { "Money", "https://data.gmanetwork.com/gno/rss/money/feed.xml" },
            { "SciTech", "https://data.gmanetwork.com/gno/rss/scitech/feed.xml" },
            { "Lifestyle", "https://data.gmanetwork.com/gno/rss/lifestyle/feed.xml" },
            { "Pinoy Abroad", "https://data.gmanetwork.com/gno/rss/pinoyabroad/feed.xml" }
        };

        public NewsAggregator(
            IServiceProvider serviceProvider,
            IHttpClientFactory httpClientFactory,
            ILogger<NewsAggregator> logger)
        {
            _serviceProvider = serviceProvider;
            _httpClientFactory = httpClientFactory;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("News Aggregator Background Worker started.");

            using var timer = new PeriodicTimer(_fetchInterval);

            do
            {
                try
                {
                    _logger.LogInformation("Starting news aggregation cycle at {Time}...", DateTimeOffset.Now);
                    await FetchAndSaveLatestNewsAsync(stoppingToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "An unexpected error occurred during news aggregation.");
                }

            } while (await timer.WaitForNextTickAsync(stoppingToken));

            _logger.LogInformation("News Aggregator Background Worker stopping.");
        }

        private async Task FetchAndSaveLatestNewsAsync(CancellationToken stoppingToken)
        {
            using var scope = _serviceProvider.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            var httpClient = _httpClientFactory.CreateClient();

            if (!httpClient.DefaultRequestHeaders.Contains("User-Agent"))
            {
                httpClient.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) InformedPH-Bot/1.0");
            }

            var newArticles = new List<NewsArticle>();

            foreach (var feed in _categoryRssFeeds)
            {
                if (stoppingToken.IsCancellationRequested) break;

                string categoryName = feed.Key;
                string rssUrl = feed.Value;

                try
                {
                    using var responseStream = await httpClient.GetStreamAsync(rssUrl, stoppingToken);

                    var xmlDoc = new XmlDocument();
                    var settings = new XmlReaderSettings
                    {
                        DtdProcessing = DtdProcessing.Ignore,
                        IgnoreComments = true
                    };

                    using (var xmlReader = XmlReader.Create(responseStream, settings))
                    {
                        xmlDoc.Load(xmlReader);
                    }

                    XmlNodeList? itemNodes = xmlDoc.SelectNodes("//channel/item");

                    if (itemNodes != null)
                    {
                        foreach (XmlNode node in itemNodes)
                        {
                            string title = node.SelectSingleNode("title")?.InnerText?.Trim() ?? "No Title";
                            string url = node.SelectSingleNode("link")?.InnerText?.Trim() ?? string.Empty;
                            string rawDescription = node.SelectSingleNode("description")?.InnerText?.Trim() ?? string.Empty;
                            string pubDateStr = node.SelectSingleNode("pubDate")?.InnerText?.Trim() ?? string.Empty;

                            // Clean HTML tags and domains from the description summary
                            string description = CleanHtmlSummary(rawDescription);

                            if (title.Length > 300) title = title[..300];
                            if (url.Length > 500) url = url[..500];
                            if (description.Length > 1000) description = description[..1000];

                            DateTime publishedAt = DateTime.TryParse(pubDateStr, out var parsedDate)
                                ? parsedDate
                                : DateTime.UtcNow;

                            bool existsInDb = !string.IsNullOrEmpty(url) && await dbContext.NewsArticles.AnyAsync(a => a.Url == url, stoppingToken);
                            bool existsInBatch = newArticles.Any(a => a.Url == url);

                            if (existsInDb || existsInBatch)
                            {
                                continue;
                            }

                            newArticles.Add(new NewsArticle
                            {
                                Title = title,
                                Url = url,
                                Summary = description,
                                PublishedAt = publishedAt
                            });
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogWarning("[NewsAggregator] Failed feed '{Category}' ({Url}): {Message}", categoryName, rssUrl, ex.Message);
                }
            }

            if (newArticles.Any())
            {
                await dbContext.NewsArticles.AddRangeAsync(newArticles, stoppingToken);
                await dbContext.SaveChangesAsync(stoppingToken);
                _logger.LogInformation("Successfully aggregated and saved {Count} new articles.", newArticles.Count);
            }
        }

        /// <summary>
        /// Removes HTML tags (like <img>, <br/>) and leftover metadata domains (like images.gmanews.tv) from the text.
        /// </summary>
        private static string CleanHtmlSummary(string input)
        {
            if (string.IsNullOrEmpty(input))
                return string.Empty;

            // 1. Remove all HTML tags (e.g. <img ... />, <br/>, etc.)
            string noHtml = Regex.Replace(input, "<.*?>", string.Empty);

            // 2. Remove trailing/isolated CDN image text fragments often found in GMA RSS feeds (e.g., "images.gmanews.tv")
            noHtml = Regex.Replace(noHtml, @"images\.gmanews\.tv", string.Empty, RegexOptions.IgnoreCase);

            // 3. Decode common HTML entities (like &amp;, &quot;, &#39;)
            string decoded = System.Net.WebUtility.HtmlDecode(noHtml);

            // 4. Trim extra whitespaces and newline breaks
            return Regex.Replace(decoded, @"\s+", " ").Trim();
        }
    }
}