using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SakilaApp.Models;

namespace SakilaApp.Controllers
{
    [Authorize]
    public class FilmsController : Controller
    {
        private readonly SakilaContext _context;

        public FilmsController(SakilaContext context)
        {
            _context = context;
        }

        private const int PageSize = 20;

        public async Task<IActionResult> Index(string? search, int page = 1)
        {
            ViewData["Search"] = search;

            var query = _context.Films
                .Where(f => f.IsActive);

            if (!string.IsNullOrWhiteSpace(search))
                query = query.Where(f =>
                    f.Title.Contains(search) ||
                    (f.Rating != null && f.Rating.Contains(search)) ||
                    (f.ReleaseYear != null && f.ReleaseYear.Contains(search)));

            query = query.OrderBy(f => f.Title);

            int total = await query.CountAsync();
            int totalPages = (int)Math.Ceiling(total / (double)PageSize);
            page = Math.Max(1, Math.Min(page, totalPages == 0 ? 1 : totalPages));

            var films = await query
                .Skip((page - 1) * PageSize)
                .Take(PageSize)
                .ToListAsync();

            ViewData["Page"] = page;
            ViewData["TotalPages"] = totalPages;
            ViewData["Total"] = total;

            return View(films);
        }

        public async Task<IActionResult> Details(int id)
        {
            var film = await _context.Films
                .Include(f => f.FilmActors)
                .ThenInclude(fa => fa.Actor)
                .FirstOrDefaultAsync(f => f.FilmId == id && f.IsActive);
            if (film == null) return NotFound();
            return View(film);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Film film)
        {
            if (ModelState.IsValid)
            {
                film.LastUpdate = DateTime.Now;
                film.IsActive = true;
                _context.Films.Add(film);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Película creada exitosamente";
                return RedirectToAction(nameof(Index));
            }
            return View(film);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var film = await _context.Films.FindAsync(id);
            if (film == null || !film.IsActive) return NotFound();
            return View(film);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Film film)
        {
            if (id != film.FilmId) return BadRequest();
            if (ModelState.IsValid)
            {
                film.LastUpdate = DateTime.Now;
                film.IsActive = true;
                _context.Films.Update(film);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Película actualizada";
                return RedirectToAction(nameof(Index));
            }
            return View(film);
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var film = await _context.Films
                .FirstOrDefaultAsync(f => f.FilmId == id && f.IsActive);
            if (film == null) return NotFound();
            return View(film);
        }

        // Eliminación lógica: marca como inactiva en lugar de borrar
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var film = await _context.Films.FindAsync(id);
            if (film != null)
            {
                film.IsActive = false;
                film.LastUpdate = DateTime.Now;
                _context.Films.Update(film);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Película eliminada correctamente";
            }
            return RedirectToAction(nameof(Index));
        }
    }
}