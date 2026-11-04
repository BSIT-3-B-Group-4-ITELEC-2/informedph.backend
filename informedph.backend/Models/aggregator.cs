using System;
using System.ComponentModel.DataAnnotations;

namespace informedph.backend.Models
{
    public class aggregator
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Title { get; set; } = string.Empty;

        public string Link { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public DateTime PublishedDate { get; set; }

        public string Category { get; set; } = "General";

        public DateTime AggregatedAt { get; set; } = DateTime.UtcNow;
    }
}