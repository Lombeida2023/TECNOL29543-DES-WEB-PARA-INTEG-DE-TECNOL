using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SakilaApp.Models;

namespace SakilaApp.Controllers;

[Authorize]
public class ActorsController : Controller
{
    private readonly SakilaContext _context;
    private const int PageSize = 20;

    public ActorsController(SakilaContext context) => _context = context;

    public async Task<IActionResult> Index(string? search, int page = 1)
    {
        ViewData["Search"] = search;
        var query = _context.Actors.Where(a => a.IsActive);

        if (!string.IsNullOrWhiteSpace(search))
            query = query.Where(a =>
                a.FirstName.Contains(search) ||
                a.LastName.Contains(search));

        query = query.OrderBy(a => a.LastName).ThenBy(a => a.FirstName);

        int total = await query.CountAsync();
        int totalPages = (int)Math.Ceiling(total / (double)PageSize);
        page = Math.Max(1, Math.Min(page, totalPages == 0 ? 1 : totalPages));

        var actors = await query
            .Skip((page - 1) * PageSize)
            .Take(PageSize)
            .ToListAsync();

        ViewData["Page"] = page;
        ViewData["TotalPages"] = totalPages;
        ViewData["Total"] = total;
        return View(actors);
    }

    public async Task<IActionResult> Details(int id)
    {
        var actor = await _context.Actors
            .Include(a => a.FilmActors)
            .ThenInclude(fa => fa.Film)
            .FirstOrDefaultAsync(a => a.ActorId == id && a.IsActive);
        if (actor == null) return NotFound();
        return View(actor);
    }

    [HttpGet]
    public IActionResult Create() => View();

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Actor actor)
    {
        if (!ModelState.IsValid) return View(actor);
        actor.LastUpdate = DateTime.Now;
        actor.IsActive = true;
        _context.Actors.Add(actor);
        await _context.SaveChangesAsync();
        TempData["Success"] = "Actor creado exitosamente";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var actor = await _context.Actors.FindAsync(id);
        if (actor == null || !actor.IsActive) return NotFound();
        return View(actor);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Actor actor)
    {
        if (id != actor.ActorId) return BadRequest();
        if (!ModelState.IsValid) return View(actor);
        actor.LastUpdate = DateTime.Now;
        actor.IsActive = true;
        _context.Actors.Update(actor);
        await _context.SaveChangesAsync();
        TempData["Success"] = "Actor actualizado exitosamente";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Delete(int id)
    {
        var actor = await _context.Actors.FindAsync(id);
        if (actor == null || !actor.IsActive) return NotFound();
        return View(actor);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var actor = await _context.Actors.FindAsync(id);
        if (actor != null)
        {
            actor.IsActive = false;
            actor.LastUpdate = DateTime.Now;
            _context.Actors.Update(actor);
            await _context.SaveChangesAsync();
            TempData["Success"] = "Actor eliminado exitosamente";
        }
        return RedirectToAction(nameof(Index));
    }
}
