using AdvancedVotingSystem.Data;
using AdvancedVotingSystem.Models.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AdvancedVotingSystem.Controllers
{
    [Authorize(Roles = "Supervisor")]
    public class SupervisorController : Controller
    {
        private readonly ApplicationDbContext _context;

        public SupervisorController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var username = User.Identity?.Name;
            var committee = await _context.Committees
                .Include(c => c.Election)
                .Include(c => c.Voters)
                .Include(c => c.Head)
                .Include(c => c.Supervisors)
                .FirstOrDefaultAsync(c => (c.Head != null && c.Head.UserName == username) || c.Supervisors.Any(s => s.UserName == username));

            if (committee == null)
            {
                return View("NoCommittee");
            }

            return View(committee);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CloseCommittee(int id)
        {
            var committee = await _context.Committees.FindAsync(id);
            if (committee == null) return NotFound();

            if (committee.Status == CommitteeStatus.Open)
            {
                committee.Status = CommitteeStatus.Closed;
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> PrintResults(int id)
        {
            var committee = await _context.Committees
                .Include(c => c.Election)
                .ThenInclude(e => e.Categories)
                .ThenInclude(cat => cat.Candidates)
                .ThenInclude(can => can.VoteRecords)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (committee == null || committee.Status != CommitteeStatus.Closed)
            {
                return Forbid("Cannot print results unless the committee is closed.");
            }

            // In a real app, filter VoteRecords by the Voters belonging to this committee.
            var committeeVoterIds = await _context.Voters.Where(v => v.CommitteeId == committee.Id).Select(v => v.Id).ToListAsync();

            ViewBag.VoterIds = committeeVoterIds;

            return View(committee);
        }

        public async Task<IActionResult> PrintVoterList(int id)
        {
            var username = User.Identity?.Name;
            var committee = await _context.Committees
                .Include(c => c.Election)
                .Include(c => c.Voters)
                .Include(c => c.Head)
                .Include(c => c.Supervisors)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (committee == null) return NotFound();

            bool isAuthorized = (committee.Head != null && committee.Head.UserName == username) || committee.Supervisors.Any(s => s.UserName == username);
            if (!isAuthorized && !User.IsInRole("Admin"))
            {
                return Forbid();
            }

            return View(committee);
        }
    }
}
