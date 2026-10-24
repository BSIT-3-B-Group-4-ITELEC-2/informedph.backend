using System.ComponentModel.DataAnnotations;

namespace informedph.backend.Models
{

    public class Test
    {
        [Key]
        public int Id { get; set; }

        public string Message { get; set; } = string.Empty;
    }
}
