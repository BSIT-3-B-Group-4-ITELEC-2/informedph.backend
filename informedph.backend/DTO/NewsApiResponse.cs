using System.Text.Json.Serialization;

namespace informedph.backend.DTO
{
    public class NewsApiResponse
    {
        [JsonPropertyName("results")]
        public List<NewsletterArticleDTO> Results { get; set; } = [];
    }
}
