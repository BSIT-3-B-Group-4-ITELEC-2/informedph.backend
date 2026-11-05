namespace informedph.backend.DTO
{
    public class NewsletterArticleDTO
    {
        public string Id { get; set; } = string.Empty;

        public string Title { get; set; } = string.Empty;

        public string Summary { get; set; } = string.Empty;

        public string Explanation { get; set; } = string.Empty;

        public DateTimeOffset PublishedAt { get; set; }

        public string Url { get; set; } = string.Empty;
    }
}
