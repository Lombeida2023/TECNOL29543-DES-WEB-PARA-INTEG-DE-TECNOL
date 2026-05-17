using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SakilaApp.Models;

namespace SakilaApp.Controllers;

[Authorize]
public class RentalsController : Controller
{
    private readonly SakilaContext _context;
    private const int PageSize = 20;

    public RentalsController(SakilaContext context) => _context = context;

    public async Task<IActionResult> Index(string? search, int page = 1)
    {
        ViewData["Search"] = search;
        var query = _context.Rentals.AsQueryable();

        if (!string.IsNullOrWhiteSpace(search) && int.TryParse(search, out int searchId))
            query = query.Where(r => r.CustomerId == searchId || r.InventoryId == searchId);

        query = query.OrderByDescending(r => r.RentalDate);

        int total = await query.CountAsync();
        int totalPages = (int)Math.Ceiling(total / (double)PageSize);
        page = Math.Max(1, Math.Min(page, totalPages == 0 ? 1 : totalPages));

        var rentals = await query
            .Skip((page - 1) * PageSize)
            .Take(PageSize)
            .ToListAsync();

        ViewData["Page"] = page;
        ViewData["TotalPages"] = totalPages;
        ViewData["Total"] = total;
        return View(rentals);
    }

    public async Task<IActionResult> Details(int id)
    {
        var rental = await _context.Rentals.FindAsync(id);
        if (rental == null) return NotFound();
        return View(rental);
    }

    [HttpGet]
    public IActionResult Create() => View();

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Rental rental)
    {
        if (!ModelState.IsValid) return View(rental);
        rental.RentalDate = DateTime.Now;
        rental.LastUpdate = DateTime.Now;
        _context.Rentals.Add(rental);
        await _context.SaveChangesAsync();
        TempData["Success"] = "Alquiler registrado exitosamente";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var rental = await _context.Rentals.FindAsync(id);
        if (rental == null) return NotFound();
        return View(rental);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Rental rental)
    {
        if (id != rental.RentalId) return BadRequest();
        if (!ModelState.IsValid) return View(rental);
        rental.LastUpdate = DateTime.Now;
        _context.Rentals.Update(rental);
        await _context.SaveChangesAsync();
        TempData["Success"] = "Alquiler actualizado exitosamente";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Delete(int id)
    {
        var rental = await _context.Rentals.FindAsync(id);
        if (rental == null) return NotFound();
        return View(rental);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var rental = await _context.Rentals.FindAsync(id);
        if (rental != null)
        {
            _context.Rentals.Remove(rental);
            await _context.SaveChangesAsync();
            TempData["Success"] = "Alquiler eliminado exitosamente";
        }
        return RedirectToAction(nameof(Index));
    }
}
