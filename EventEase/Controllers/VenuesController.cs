
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EventEase.Models;
using EventEase.Data;
using EventEase.Services;

public class VenuesController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly BlobService _blobService;

    public VenuesController(ApplicationDbContext context, BlobService blobService)
    {
        _context = context;
        _blobService = blobService;
    }

    // GET: VENUES
    public async Task<IActionResult> Index()    
    {
        return View(await _context.Venues.ToListAsync());
    }

    // GET: VENUES/Details/5
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var venue = await _context.Venues
            .FirstOrDefaultAsync(m => m.VenueId == id);
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
    public async Task<IActionResult> Create([Bind("VenueId,VenueName,Location,Capacity,ImageUrl,IsAvailable")] Venue venue, IFormFile? imageFile)
    {
        if (ModelState.IsValid)
        {
            if (imageFile != null && imageFile.Length > 0)
            {
                venue.ImageUrl = await _blobService.UploadImageAsync(imageFile);
            }

            _context.Add(venue);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        return View(venue);
    }

    // GET: VENUES/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var venue = await _context.Venues.FindAsync(id);
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
    public async Task<IActionResult> Edit(int id, [Bind("VenueId,VenueName,Location,Capacity,ImageUrl,IsAvailable")] Venue venue, IFormFile? imageFile)
    {
        if (id != venue.VenueId)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            try
            {
                if (imageFile != null && imageFile.Length > 0)
                {
                    venue.ImageUrl = await _blobService.UploadImageAsync(imageFile);
                }

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
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var venue = await _context.Venues
            .FirstOrDefaultAsync(m => m.VenueId == id);
        if (venue == null)
        {
            return NotFound();
        }

        return View(venue);
    }

    // POST: VENUES/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        bool hasBookings = await _context.Bookings.AnyAsync(b => b.VenueId == id);
        if (hasBookings)
        {
            TempData["Error"] = "This venue cannot be deleted because it has existing bookings. Remove those bookings first.";
            return RedirectToAction(nameof(Index));
        }

        var venue = await _context.Venues.FindAsync(id);
        if (venue != null)
        {
            try
            {
                _context.Venues.Remove(venue);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Venue deleted successfully.";
            }
            catch (DbUpdateException)
            {
                TempData["Error"] = "The venue could not be deleted because other records depend on it.";
            }
        }
        return RedirectToAction(nameof(Index));
    }
    private bool VenueExists(int? id)
    {
        return _context.Venues.Any(e => e.VenueId == id);
    }
}
