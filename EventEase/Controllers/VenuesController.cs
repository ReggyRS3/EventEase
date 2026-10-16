
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EventEase.Models;
using EventEase.Data;

public class VenuesController : Controller
{
    private readonly ApplicationDbContext _context;

    public VenuesController(ApplicationDbContext context)
    {
        _context = context;
    }

    // GET: VENUES
    public async Task<IActionResult> Index()    
    {
        return View(await _context.Venues.ToListAsync());
    }

    // GET: VENUES/Details/5
    public async Task<IActionResult> Details(int? venueid)
    {
        if (venueid == null)
        {
            return NotFound();
        }

        var venue = await _context.Venues
            .FirstOrDefaultAsync(m => m.VenueId == venueid);
        if (venue == null)
        {
            return NotFound();
        }

        return View(venue);
    }

    // GET: VENUES/Create
    public IActionResult Create()
    {
        return View();
    }

    // POST: VENUES/Create
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("VenueId,VenueName,Location,Capacity,ImageUrl")] Venue venue)
    {
        if (ModelState.IsValid)
        {
            _context.Add(venue);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        return View(venue);
    }

    // GET: VENUES/Edit/5
    public async Task<IActionResult> Edit(int? venueid)
    {
        if (venueid == null)
        {
            return NotFound();
        }

        var venue = await _context.Venues.FindAsync(venueid);
        if (venue == null)
        {
            return NotFound();
        }
        return View(venue);
    }

    // POST: VENUES/Edit/5
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int? venueid, [Bind("VenueId,VenueName,Location,Capacity,ImageUrl")] Venue venue)
    {
        if (venueid != venue.VenueId)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            try
            {
                _context.Update(venue);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!VenueExists(venue.VenueId))
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
        return View(venue);
    }

    // GET: VENUES/Delete/5
    public async Task<IActionResult> Delete(int? venueid)
    {
        if (venueid == null)
        {
            return NotFound();
        }

        var venue = await _context.Venues
            .FirstOrDefaultAsync(m => m.VenueId == venueid);
        if (venue == null)
        {
            return NotFound();
        }

        return View(venue);
    }

    // POST: VENUES/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int? venueid)
    {
        var venue = await _context.Venues.FindAsync(venueid);
        if (venue != null)
        {
            _context.Venues.Remove(venue);
        }

        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    private bool VenueExists(int? venueid)
    {
        return _context.Venues.Any(e => e.VenueId == venueid);
    }
}
