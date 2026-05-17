using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SakilaApp.Models;

namespace SakilaApp.Controllers;

[Authorize]
public class CategoriesController : Controller
{
    private readonly SakilaContext _context;
    private const int PageSize = 20;

    public CategoriesController(SakilaContext context) => _context = context;

    public async Task<IActionResult> Index(string? search, int page = 1)
    {
        ViewData["Search"] = search;
        var query = _context.Categories.AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
            query = query.Where(c => c.Name.Contains(search));

        query = query.OrderBy(c => c.Name);

        int total = await query.CountAsync();
        int totalPages = (int)Math.Ceiling(total / (double)PageSize);
        page = Math.Max(1, Math.Min(page, totalPages == 0 ? 1 : totalPages));

        var categories = await query
            .Skip((page - 1) * PageSize)
            .Take(PageSize)
            .ToListAsync();

        ViewData["Page"] = page;
        ViewData["TotalPages"] = totalPages;
        ViewData["Total"] = total;
        return View(categories);
    }

    public async Task<IActionResult> Details(byte id)
    {
        var category = await _context.Categories.FindAsync(id);
        if (category == null) return NotFound();
        return View(category);
    }

    [HttpGet]
    public IActionResult Create() => View();

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Category category)
    {
        if (!ModelState.IsValid) return View(category);
        category.LastUpdate = DateTime.Now;
        _context.Categories.Add(category);
        await _context.SaveChangesAsync();
        TempData["Success"] = "Categoría creada exitosamente";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Edit(byte id)
    {
        var category = await _context.Categories.FindAsync(id);
        if (category == null) return NotFound();
        return View(category);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(byte id, Category category)
    {
        if (id != category.CategoryId) return BadRequest();
        if (!ModelState.IsValid) return View(category);
        category.LastUpdate = DateTime.Now;
        _context.Categories.Update(category);
        await _context.SaveChangesAsync();
        TempData["Success"] = "Categoría actualizada exitosamente";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Delete(byte id)
    {
        var category = await _context.Categories.FindAsync(id);
        if (category == null) return NotFound();
        return View(category);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(byte id)
    {
        var category = await _context.Categories.FindAsync(id);
        if (category != null)
        {
            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();
            TempData["Success"] = "Categoría eliminada exitosamente";
        }
        return RedirectToAction(nameof(Index));
    }
}
