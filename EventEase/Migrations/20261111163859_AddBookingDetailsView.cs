using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EventEase.Migrations
{
    /// <inheritdoc />
    public partial class AddBookingDetailsView : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
CREATE VIEW vw_BookingDetails AS
SELECT
    b.BookingId,
    b.BookingDate,
    e.EventId,
    e.EventName,
    e.EventDate,
    e.Description AS EventDescription,
    e.ImageUrl    AS EventImageUrl,
    v.VenueId,
    v.VenueName,
    v.Location,
    v.Capacity,
    v.ImageUrl    AS VenueImageUrl
FROM Bookings b
INNER JOIN Events e ON b.EventId = e.EventId
INNER JOIN Venues v ON b.VenueId = v.VenueId;");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP VIEW vw_BookingDetails;");
        }
    }
}