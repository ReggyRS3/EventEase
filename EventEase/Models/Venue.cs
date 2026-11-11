using System.ComponentModel.DataAnnotations;

namespace EventEase.Models
{
    public class Venue
    {
        public int VenueId { get; set; }

        [Required]
        public string VenueName { get; set; }

        [Required]
        public string Location { get; set; }

        public int Capacity { get; set; }

        public string? ImageUrl { get; set; }
        public bool IsAvailable { get; set; } = true;
    }
}