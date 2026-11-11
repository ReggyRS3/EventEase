
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EventEase.Models;
using EventEase.Data;
using Microsoft.AspNetCore.Mvc.Rendering;
using EventEase.Services;

public class EventsController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly BlobService _blobService;

    public EventsController(ApplicationDbContext context, BlobService blobService)
    {
        _context = context;
        _blobService = blobService;
    }

    // GET: EVENTS
    public async Task<IActionResult> Index()    
    {
        return View(await _context.Events.Include(e => e.Venue).ToListAsync());
    }

    // GET: EVENTS/Details/5
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var @event = await _context.Events
    .Include(e => e.Venue)
    .FirstOrDefaultAsync(m => m.EventId == id);
        if (@event == null)
        {
            return NotFound();
        }

        return View(@event);
    }

    // GET: EVENTS/Create
    public IActionResult Create()
    {
        ViewData["VenueId"] = new SelectList(_context.Venues, "VenueId", "VenueName");
        ViewData["EventTypeId"] = new SelectList(_context.EventTypes, "EventTypeId", "Name");
        return View();
    }

    // POST: EVENTS/Create
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("EventId,EventName,EventDate,Description,ImageUrl,VenueId,EventTypeId")] Event @event, IFormFile? imageFile)
    {
        if (ModelState.IsValid)
        {
            if (imageFile != null && imageFile.Length > 0)
            {
                @event.ImageUrl = await _blobService.UploadImageAsync(imageFile);
            }

            _context.Add(@event);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        ViewData["VenueId"] = new SelectList(_context.Venues, "VenueId", "VenueName", @event.VenueId);
        ViewData["EventTypeId"] = new SelectList(_context.EventTypes, "EventTypeId", "Name", @event.EventTypeId);
        return View(@event);
    }

    // GET: EVENTS/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var @event = await _context.Events.FindAsync(id);
        if (@event == null)
        {
            return NotFound();
        }
        ViewData["VenueId"] = new SelectList(_context.Venues, "VenueId", "VenueName", @event.VenueId);
        ViewData["EventTypeId"] = new SelectList(_context.EventTypes, "EventTypeId", "Name", @event.EventTypeId);
        return View(@event);
    }

    // POST: EVENTS/Edit/5
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int? id, [Bind("EventId,EventName,EventDate,Description,ImageUrl,VenueId,EventTypeId")] Event @event, IFormFile? imageFile)
    {
        if (id != @event.EventId)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            try
            {
                if (imageFile != null && imageFile.Length > 0)
                {
                    @event.ImageUrl = await _blobService.UploadImageAsync(imageFile);
                }

                _context.Update(@event);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!EventExists(@event.EventId))
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
        ViewData["VenueId"] = new SelectList(_context.Venues, "VenueId", "VenueName", @event.VenueId);
        ViewData["EventTypeId"] = new SelectList(_context.EventTypes, "EventTypeId", "Name", @event.EventTypeId);
        return View(@event);
    }

    // GET: EVENTS/Delete/5
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var @event = await _context.Events
    .Include(e => e.Venue)
    .FirstOrDefaultAsync(m => m.EventId == id);
        if (@event == null)
        {
            return NotFound();
        }

        return View(@event);
    }

    // POST: EVENTS/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        bool hasBookings = await _context.Bookings.AnyAsync(b => b.EventId == id);
        if (hasBookings)
        {
            TempData["Error"] = "This event cannot be deleted because it has existing bookings. Remove those bookings first.";
            return RedirectToAction(nameof(Index));
        }

        var @event = await _context.Events.FindAsync(id);
        if (@event != null)
        {
            try
            {
                _context.Events.Remove(@event);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Event deleted successfully.";
            }
            catch (DbUpdateException)
            {
                TempData["Error"] = "The event could not be deleted because other records depend on it.";
            }
        }
        return RedirectToAction(nameof(Index));
    }

    private bool EventExists(int? id)
    {
        return _context.Events.Any(e => e.EventId == id);
    }
}
