namespace TrackAPI.Entities
{
    public class Event
    {
        public int Id { get; set; }
        public string EventName { get; set; }
        public string Description { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; } = DateTime.MinValue;
        public int EventStatus {  get; set; }
        public int EventType { get; set; }
        public string PhoneNumber { get; set; }
        public int OrganizerId { get; set; }
        public virtual User Organizer { get; set; }
    }
}
