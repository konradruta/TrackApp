namespace TrackAPI.Models
{
    public class EditEventDto
    {
        public string EventName { get; set; }
        public string Description { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int EventType { get; set; }
    }
}
