using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SakilaApp.Models;

namespace SakilaApp.Controllers;

[Authorize]
public class StoresController : Controller
{
    private readonly SakilaContext _context;

    public StoresController(SakilaContext context) => _context = context;

    public async Task<IActionResult> Index()
    {
        var stores = await _context.Stores.OrderBy(s => s.StoreId).ToListAsync();
        return View(stores);
    }

    public async Task<IActionResult> Details(int id)
    {
        var store = await _context.Stores.FindAsync(id);
        if (store == null) return NotFound();
        return View(store);
    }

    [HttpGet]
    public IActionResult Create() => View();

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Store store)
    {
        if (!ModelState.IsValid) return View(store);
        store.LastUpdate = DateTime.Now;
        _context.Stores.Add(store);
        await _context.SaveChangesAsync();
        TempData["Success"] = "Tienda creada exitosamente";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var store = await _context.Stores.FindAsync(id);
        if (store == null) return NotFound();
        return View(store);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Store store)
    {
        if (id != store.StoreId) return BadRequest();
        if (!ModelState.IsValid) return View(store);
        store.LastUpdate = DateTime.Now;
        _context.Stores.Update(store);
        await _context.SaveChangesAsync();
        TempData["Success"] = "Tienda actualizada exitosamente";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Delete(int id)
    {
        var store = await _context.Stores.FindAsync(id);
        if (store == null) return NotFound();
        return View(store);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var store = await _context.Stores.FindAsync(id);
        if (store != null)
        {
            _context.Stores.Remove(store);
            await _context.SaveChangesAsync();
            TempData["Success"] = "Tienda eliminada exitosamente";
        }
        return RedirectToAction(nameof(Index));
    }
}
