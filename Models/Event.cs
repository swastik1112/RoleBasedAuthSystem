namespace RoleBasedAuthSystem.Models
{
    public class Event
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public DateTime Start { get; set; }
        public DateTime? End { get; set; }
        public bool AllDay { get; set; } = false;
        public string? BackgroundColor { get; set; }
        public string? Description { get; set; }
        public string? Type { get; set; }  // maintenance, meeting, task, holiday...
        public string? UserId { get; set; } // optional – for user-specific events
    }

}
