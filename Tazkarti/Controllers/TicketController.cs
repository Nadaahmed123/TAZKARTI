using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Tazkarti.Models;
using System.Security.Claims;
using System.Threading.Tasks;

[Route("ticket")]
public class TicketController : Controller
{
    private readonly AppDbContext _context;

    public TicketController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet("book/{matchId}")]
    public async Task<IActionResult> BookTicket(int matchId)
    {
        if (!User.Identity.IsAuthenticated)
        {
            return RedirectToAction("Login", "Account", new { returnUrl = Url.Action("BookTicket", new { matchId }) });
        }

        var match = await _context.Matches.FindAsync(matchId);
        if (match == null) return NotFound();

        var viewModel = new BookTicketViewModel
        {
            MatchId = matchId,
            Match = match 
        };

        return View(viewModel);
    }

    [HttpPost("book/{matchId}")]
    public async Task<IActionResult> BookTicket(int matchId, decimal price)
    {
        if (!User.Identity.IsAuthenticated)
        {
            return RedirectToAction("Login", "Account", new { returnUrl = Url.Action("BookTicket", new { matchId }) });
        }

        var userName = User.Identity.Name; 
        if (ModelState.IsValid)
        {
            var ticket = new Ticket
            {
                MatchId = matchId,
                UserName = userName,
                Price = price
            };

            _context.Tickets.Add(ticket);
            await _context.SaveChangesAsync();

            return RedirectToAction("TicketDetails", new { id = ticket.Id });
        }
        var existingMatch = await _context.Matches.FindAsync(matchId);
        var viewModel = new BookTicketViewModel
        {
            MatchId = matchId,
            UserName = userName, 
            Price = price,
            Match = existingMatch 
        };

        return View(viewModel);
    }

    [HttpGet("details/{id}")]
    public async Task<IActionResult> TicketDetails(int id)
    {
        var ticket = await _context.Tickets.Include(t => t.Match).FirstOrDefaultAsync(t => t.Id == id);
        if (ticket == null) return NotFound();

        return View(ticket);
    }
}
