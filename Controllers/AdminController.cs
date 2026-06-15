using AdvancedVotingSystem.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using AdvancedVotingSystem.Models.ViewModels;

namespace AdvancedVotingSystem.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AdminController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var model = new DashboardViewModel
            {
                TotalElections = await _context.Elections.CountAsync(),
                ActiveElections = await _context.Elections.CountAsync(e => e.Status == Models.Enums.ElectionStatus.Active),
                TotalCommittees = await _context.Committees.CountAsync(),
                TotalVoters = await _context.Voters.CountAsync()
            };
            
            return View(model);
        }

        public async Task<IActionResult> GlobalReports()
        {
            var elections = await _context.Elections
                .Include(e => e.Committees)
                .Include(e => e.Categories)
                .ThenInclude(c => c.Candidates)
                .ThenInclude(c => c.VoteRecords)
                .ToListAsync();

            return View(elections);
        }
    }
}
