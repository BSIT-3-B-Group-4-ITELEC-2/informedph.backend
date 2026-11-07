using System;
using System.ComponentModel.DataAnnotations;

namespace informedph.backend.Models
{
    public class NewsArticle
    {
        public int Id { get; set; }

        [Required, MaxLength(300)]
        public string Title { get; set; } = string.Empty;

        [Required, MaxLength(500)]
        public string Url { get; set; } = string.Empty;

        [MaxLength(1000)]
        public string? Summary { get; set; }

        [MaxLength(500)]
        public string? ImageUrl { get; set; }

        public DateTime PublishedAt { get; set; }
    }
}