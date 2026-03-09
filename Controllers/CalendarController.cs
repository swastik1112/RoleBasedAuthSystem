using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RoleBasedAuthSystem.Data;          // ← make sure this is included
using System;
using System.Linq;
using System.Threading.Tasks;

[Authorize]
public class CalendarController : Controller
{
    private readonly ApplicationDbContext _context;

    // Constructor injection
    public CalendarController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> GetEvents(DateTime start, DateTime end)
    {
        var events = await _context.Events
            .Where(e => e.Start >= start && e.Start <= end)
            .Select(e => new
            {
                id = e.Id,
                title = e.Title,
                start = e.Start,
                end = e.End,
                allDay = e.AllDay,
                backgroundColor = e.BackgroundColor,
                extendedProps = new { type = e.Type, description = e.Description }
            })
            .ToListAsync();

        return Json(events);
    }

    public IActionResult Index()
    {
        return View();
    }
}