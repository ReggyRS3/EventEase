
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EventEase.Models;
using EventEase.Data;
using Microsoft.AspNetCore.Mvc.Rendering;

public class BookingsController : Controller
{
    private readonly ApplicationDbContext _context;

    public BookingsController(ApplicationDbContext context)
    {
        _context = context;
    }
    // GET: BOOKINGS
    public async Task<IActionResult> Index(string? search)
    {
        var bookings = _context.BookingDetails.AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            search = search.Trim();

            if (int.TryParse(search, out int bookingId))
            {
                bookings = bookings.Where(b => b.BookingId == bookingId || b.EventName.Contains(search));
            }
            else
            {
                bookings = bookings.Where(b => b.EventName.Contains(search));
            }
        }

        ViewData["Search"] = search;
        return View(await bookings.OrderBy(b => b.BookingDate).ToListAsync());
    }

    // GET: BOOKINGS/Details/5
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var booking = await _context.Bookings
     .Include(b => b.Event)
     .Include(b => b.Venue)
     .FirstOrDefaultAsync(m => m.BookingId == id);
        if (booking == null)
        {
            return NotFound();
        }

        return View(booking);
    }

    // GET: BOOKINGS/Create
    public IActionResult Create()
    {
        ViewData["EventId"] = new SelectList(_context.Events, "EventId", "EventName");
        ViewData["VenueId"] = new SelectList(_context.Venues, "VenueId", "VenueName");
        return View();
    }

    // POST: BOOKINGS/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("BookingId,BookingDate,VenueId,EventId")] Booking booking)
    {
        bool doubleBooked = await _context.Bookings.AnyAsync(b =>
    b.VenueId == booking.VenueId &&
    b.BookingDate == booking.BookingDate &&
    b.BookingId != booking.BookingId);

        if (doubleBooked)
        {
            ModelState.AddModelError("", "This venue is already booked for that date and time. Please choose another time or venue.");
            TempData["Error"] = "Booking not saved: the venue is already booked at that date and time.";
        }
        if (ModelState.IsValid)
        {
            _context.Add(booking);
            await _context.SaveChangesAsync();
            TempData["Success"] = "Booking created successfully.";
            return RedirectToAction(nameof(Index));
        }
        ViewData["EventId"] = new SelectList(_context.Events, "EventId", "EventName", booking.EventId);
        ViewData["VenueId"] = new SelectList(_context.Venues, "VenueId", "VenueName", booking.VenueId);
        return View(booking);
    }

    // GET: BOOKINGS/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var booking = await _context.Bookings.FindAsync(id);
        if (booking == null)
        {
            return NotFound();
        }
        ViewData["EventId"] = new SelectList(_context.Events, "EventId", "EventName", booking.EventId);
        ViewData["VenueId"] = new SelectList(_context.Venues, "VenueId", "VenueName", booking.VenueId);
        return View(booking);
    }

    // POST: BOOKINGS/Edit/5
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int? id, [Bind("BookingId,BookingDate,VenueId, EventId")] Booking booking)
    {
        if (id != booking.BookingId)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            try
            {
                _context.Update(booking);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Booking updated successfully.";
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!BookingExists(booking.BookingId))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return RedirectToAction(nameof(Index));
        }
        ViewData["EventId"] = new SelectList(_context.Events, "EventId", "EventName", booking.EventId);
        ViewData["VenueId"] = new SelectList(_context.Venues, "VenueId", "VenueName", booking.VenueId);
        return View(booking);
    }

    // GET: BOOKINGS/Delete/5
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var booking = await _context.Bookings
    .Include(b => b.Event)
    .Include(b => b.Venue)
    .FirstOrDefaultAsync(m => m.BookingId == id);
        if (booking == null)
        {
            return NotFound();
        }

        return View(booking);
    }

    // POST: BOOKINGS/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int? id)
    {
        var booking = await _context.Bookings.FindAsync(id);
        if (booking != null)
        {
            _context.Bookings.Remove(booking);
        }

        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    private bool BookingExists(int? id)
    {
        return _context.Bookings.Any(e => e.BookingId == id);
    }
}