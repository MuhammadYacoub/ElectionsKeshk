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
            var totalElections = await _context.Elections.CountAsync();
            var activeElections = await _context.Elections.CountAsync(e => e.Status == Models.Enums.ElectionStatus.Active);
            var totalCommittees = await _context.Committees.CountAsync();
            var totalVoters = await _context.Voters.CountAsync();

            var recentVoters = await _context.Voters
                .Include(v => v.Committee)
                .Where(v => v.HasVoted)
                .OrderByDescending(v => v.Id)
                .Take(7)
                .Select(v => new VoterDashboardDto
                {
                    VoterName = v.FullName,
                    NationalId = v.NationalId ?? v.LoginIdentifier,
                    CommitteeName = v.Committee != null ? v.Committee.Name : "غير محدد"
                })
                .ToListAsync();

            var committees = await _context.Committees
                .Include(c => c.Election)
                .Select(c => new CommitteeDashboardDto
                {
                    Name = c.Name,
                    ElectionName = c.Election != null ? c.Election.Name : "غير محدد",
                    TotalVoters = c.RegisteredVotersCount,
                    VotesCast = c.TotalVotesCast,
                    Status = c.Status.ToString()
                })
                .ToListAsync();

            var model = new DashboardViewModel
            {
                TotalElections = totalElections,
                ActiveElections = activeElections,
                TotalCommittees = totalCommittees,
                TotalVoters = totalVoters,
                RecentVoters = recentVoters,
                Committees = committees
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
