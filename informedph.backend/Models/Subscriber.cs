using System;

namespace informedph.backend.Models
{
    public class Subscriber
    {
        public int Id { get; set; }
        public string fullName { get; set; }
        public string email { get; set; }
        public string Gender { get; set; }
        public DateTime Birthday { get; set; }
        public int Age { get; set; }
    }
}
