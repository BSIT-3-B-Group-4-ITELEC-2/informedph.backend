using System.Text.Json.Serialization;

namespace informedph.backend.DTO
{
    public class NewsletterArticleDTO
    {
        [JsonPropertyName("id")]
        public string Id { get; set; } = string.Empty;

        [JsonPropertyName("title")]
        public string Title { get; set; } = string.Empty;

        [JsonPropertyName("summary")]
        public string Summary { get; set; } = string.Empty;

        [JsonPropertyName("explanation")]
        public string Explanation { get; set; } = string.Empty;

        [JsonPropertyName("published_at")]
        public DateTimeOffset PublishedAt { get; set; }

        [JsonPropertyName("url")]
        public string? Url { get; set; }
    }
}
