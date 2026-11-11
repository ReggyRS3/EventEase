namespace EventEase.Models
{
    public class BookingDetails
    {
        public int BookingId { get; set; }
        public DateTime BookingDate { get; set; }

        public int EventId { get; set; }
        public string EventName { get; set; } = string.Empty;
        public DateTime EventDate { get; set; }
        public string? EventDescription { get; set; }
        public string? EventImageUrl { get; set; }

        public int VenueId { get; set; }
        public string VenueName { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
        public int Capacity { get; set; }
        public string? VenueImageUrl { get; set; }
    }
}