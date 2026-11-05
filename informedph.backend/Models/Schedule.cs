namespace informedph.backend.Models
{
    public class Schedule
    {
        public int Id { get; set; }

        public TimeSpan ExecutionTime { get; set; }

        public DateTime? LastExecutedAt { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }
    }

}
