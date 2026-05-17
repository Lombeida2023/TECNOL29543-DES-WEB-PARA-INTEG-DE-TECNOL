using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SakilaApp.Models;

namespace SakilaApp.Controllers;

[Authorize]
public class InventoriesController : Controller
{
    private readonly SakilaContext _context;
    private const int PageSize = 20;

    public InventoriesController(SakilaContext context) => _context = context;

    public async Task<IActionResult> Index(string? search, int page = 1)
    {
        ViewData["Search"] = search;
        var query = _context.Inventories.AsQueryable();

        if (!string.IsNullOrWhiteSpace(search) && int.TryParse(search, out int searchId))
            query = query.Where(i => i.FilmId == searchId || i.StoreId == searchId);

        query = query.OrderBy(i => i.StoreId).ThenBy(i => i.FilmId);

        int total = await query.CountAsync();
        int totalPages = (int)Math.Ceiling(total / (double)PageSize);
        page = Math.Max(1, Math.Min(page, totalPages == 0 ? 1 : totalPages));

        var inventories = await query
            .Skip((page - 1) * PageSize)
            .Take(PageSize)
            .ToListAsync();

        ViewData["Page"] = page;
        ViewData["TotalPages"] = totalPages;
        ViewData["Total"] = total;
        return View(inventories);
    }

    public async Task<IActionResult> Details(int id)
    {
        var inventory = await _context.Inventories.FindAsync(id);
        if (inventory == null) return NotFound();
        return View(inventory);
    }

    [HttpGet]
    public IActionResult Create() => View();

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Inventory inventory)
    {
        if (!ModelState.IsValid) return View(inventory);
        inventory.LastUpdate = DateTime.Now;
        _context.Inventories.Add(inventory);
        await _context.SaveChangesAsync();
        TempData["Success"] = "Inventario creado exitosamente";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var inventory = await _context.Inventories.FindAsync(id);
        if (inventory == null) return NotFound();
        return View(inventory);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Inventory inventory)
    {
        if (id != inventory.InventoryId) return BadRequest();
        if (!ModelState.IsValid) return View(inventory);
        inventory.LastUpdate = DateTime.Now;
        _context.Inventories.Update(inventory);
        await _context.SaveChangesAsync();
        TempData["Success"] = "Inventario actualizado exitosamente";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Delete(int id)
    {
        var inventory = await _context.Inventories.FindAsync(id);
        if (inventory == null) return NotFound();
        return View(inventory);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var inventory = await _context.Inventories.FindAsync(id);
        if (inventory != null)
        {
            _context.Inventories.Remove(inventory);
            await _context.SaveChangesAsync();
            TempData["Success"] = "Inventario eliminado exitosamente";
        }
        return RedirectToAction(nameof(Index));
    }
}
