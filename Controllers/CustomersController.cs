using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SakilaApp.Models;

namespace SakilaApp.Controllers;

[Authorize]
public class CustomersController : Controller
{
    private readonly SakilaContext _context;
    private const int PageSize = 20;

    public CustomersController(SakilaContext context) => _context = context;

    public async Task<IActionResult> Index(string? search, int page = 1)
    {
        ViewData["Search"] = search;
        var query = _context.Customers.Where(c => c.Active);

        if (!string.IsNullOrWhiteSpace(search))
            query = query.Where(c =>
                c.FirstName.Contains(search) ||
                c.LastName.Contains(search) ||
                (c.Email != null && c.Email.Contains(search)));

        query = query.OrderBy(c => c.LastName).ThenBy(c => c.FirstName);

        int total = await query.CountAsync();
        int totalPages = (int)Math.Ceiling(total / (double)PageSize);
        page = Math.Max(1, Math.Min(page, totalPages == 0 ? 1 : totalPages));

        var customers = await query
            .Skip((page - 1) * PageSize)
            .Take(PageSize)
            .ToListAsync();

        ViewData["Page"] = page;
        ViewData["TotalPages"] = totalPages;
        ViewData["Total"] = total;
        return View(customers);
    }

    public async Task<IActionResult> Details(int id)
    {
        var customer = await _context.Customers.FindAsync(id);
        if (customer == null || !customer.Active) return NotFound();
        return View(customer);
    }

    [HttpGet]
    public IActionResult Create() => View();

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Customer customer)
    {
        if (!ModelState.IsValid) return View(customer);
        customer.CreateDate = DateTime.Now;
        customer.LastUpdate = DateTime.Now;
        customer.Active = true;
        _context.Customers.Add(customer);
        await _context.SaveChangesAsync();
        TempData["Success"] = "Cliente creado exitosamente";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var customer = await _context.Customers.FindAsync(id);
        if (customer == null || !customer.Active) return NotFound();
        return View(customer);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Customer customer)
    {
        if (id != customer.CustomerId) return BadRequest();
        if (!ModelState.IsValid) return View(customer);
        customer.LastUpdate = DateTime.Now;
        customer.Active = true;
        _context.Customers.Update(customer);
        await _context.SaveChangesAsync();
        TempData["Success"] = "Cliente actualizado exitosamente";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Delete(int id)
    {
        var customer = await _context.Customers.FindAsync(id);
        if (customer == null || !customer.Active) return NotFound();
        return View(customer);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var customer = await _context.Customers.FindAsync(id);
        if (customer != null)
        {
            customer.Active = false;
            customer.LastUpdate = DateTime.Now;
            _context.Customers.Update(customer);
            await _context.SaveChangesAsync();
            TempData["Success"] = "Cliente desactivado exitosamente";
        }
        return RedirectToAction(nameof(Index));
    }
}
