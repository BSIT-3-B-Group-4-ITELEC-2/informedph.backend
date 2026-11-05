using informedph.backend.DTO;

namespace informedph.backend.View_Models
{
    public class NewsletterViewModel
    {
        public DateTimeOffset NewsletterDate { get; set; }
        public List<NewsletterArticleDTO> Articles { get; set; } = [];
    }
}
