
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EventEase.Models;
using EventEase.Data;
using Microsoft.AspNetCore.Mvc.Rendering;

public class EventsController : Controller
{
    private readonly ApplicationDbContext _context;

    public EventsController(ApplicationDbContext context)
    {
        _context = context;
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
        return View();
    }

    // POST: EVENTS/Create
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("EventId,EventName,EventDate,Description,VenueId")] Event @event)
    {
        if (ModelState.IsValid)
        {
            _context.Add(@event);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        ViewData["VenueId"] = new SelectList(_context.Venues, "VenueId", "VenueName", @event.VenueId);
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
        return View(@event);
    }

    // POST: EVENTS/Edit/5
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int? id, [Bind("EventId,EventName,EventDate,Description,VenueId,Venue")] Event @event)
    {
        if (id != @event.EventId)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            try
            {
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
    public async Task<IActionResult> DeleteConfirmed(int? id)
    {
        var @event = await _context.Events.FindAsync(id);
        if (@event != null)
        {
            _context.Events.Remove(@event);
        }

        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    private bool EventExists(int? id)
    {
        return _context.Events.Any(e => e.EventId == id);
    }
}
