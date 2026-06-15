using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using AdvancedVotingSystem.Models;
using AdvancedVotingSystem.Data;
using Microsoft.EntityFrameworkCore;
using AdvancedVotingSystem.Models.Enums;

namespace AdvancedVotingSystem.Controllers;

public class HomeController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<HomeController> _logger;

    public HomeController(ApplicationDbContext context, ILogger<HomeController> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<IActionResult> Index()
    {
        var activeElection = await _context.Elections
            .Include(e => e.Committees)
            .FirstOrDefaultAsync(e => e.Status == ElectionStatus.Active);

        if (activeElection == null)
        {
            return View("~/Views/Kiosk/NoActiveElection.cshtml");
        }

        return View(activeElection);
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
