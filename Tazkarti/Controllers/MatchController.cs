using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using Tazkarti.Models;

public class MatchController : Controller
{
    private readonly AppDbContext _context;
    private readonly string _uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images");

    public MatchController(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var matches = await _context.Matches
            .Where(m => m.MatchDate > DateTime.Now)
            .ToListAsync();
        return View(matches);
    }

    [HttpGet]
    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Create(Match match, IFormFile? teamAImage, IFormFile? teamBImage)
    {
        if (ModelState.IsValid)
        {
            if (teamAImage != null && teamAImage.Length > 0)
            {
                var teamAImagePath = Path.Combine(_uploadsFolder, teamAImage.FileName);
                using (var stream = new FileStream(teamAImagePath, FileMode.Create))
                {
                    await teamAImage.CopyToAsync(stream);
                }
                match.TeamAImage = $"/images/{teamAImage.FileName}";
            }

            if (teamBImage != null && teamBImage.Length > 0)
            {
                var teamBImagePath = Path.Combine(_uploadsFolder, teamBImage.FileName);
                using (var stream = new FileStream(teamBImagePath, FileMode.Create))
                {
                    await teamBImage.CopyToAsync(stream);
                }
                match.TeamBImage = $"/images/{teamBImage.FileName}";
            }

            _context.Matches.Add(match);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        return View(match);
    }
}
