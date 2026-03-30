namespace TrackAPI.Models
{
    public class AddEventDto
    {
        public string EventName { get; set; }
        public string Description { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int EventStatus { get; set; }
        public int EventType { get; set; }
        public string PhoneNumber { get; set; }
        public int OrganizerId { get; set; }
    }
}
