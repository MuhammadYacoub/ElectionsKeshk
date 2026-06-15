using AdvancedVotingSystem.Data;
using AdvancedVotingSystem.Models;
using AdvancedVotingSystem.Models.Enums;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using Microsoft.AspNetCore.Identity;

namespace AdvancedVotingSystem.Controllers
{
    public class KioskController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public KioskController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // KIOSK LIFECYCLE: Unlock Kiosk
        public IActionResult UnlockKiosk()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UnlockKiosk(string nationalId, string password)
        {
            var user = await _userManager.Users.FirstOrDefaultAsync(u => u.NationalId == nationalId);
            if (user != null && await _userManager.CheckPasswordAsync(user, password))
            {
                var committee = await _context.Committees
                    .Include(c => c.Election)
                    .FirstOrDefaultAsync(c => c.HeadId == user.Id && c.Status == CommitteeStatus.Open);

                if (committee != null)
                {
                    HttpContext.Session.SetInt32("KioskCommitteeId", committee.Id);
                    return RedirectToAction(nameof(Index));
                }
            }

            TempData["Error"] = "بيانات الدخول غير صحيحة، أو لا توجد لجنة مفتوحة مخصصة لك.";
            return RedirectToAction(nameof(UnlockKiosk));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult LogoutKiosk()
        {
            // Pause session
            HttpContext.Session.Remove("KioskCommitteeId");
            HttpContext.Session.Remove("VoterId");
            return RedirectToAction(nameof(UnlockKiosk));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CloseCommittee(string confirmPassword)
        {
            var committeeId = HttpContext.Session.GetInt32("KioskCommitteeId");
            if (committeeId == null) return RedirectToAction(nameof(UnlockKiosk));

            var committee = await _context.Committees.FindAsync(committeeId.Value);
            if (committee == null) return RedirectToAction(nameof(UnlockKiosk));

            var user = await _userManager.FindByIdAsync(committee.HeadId);
            if (user != null && await _userManager.CheckPasswordAsync(user, confirmPassword))
            {
                committee.Status = CommitteeStatus.Closed;
                await _context.SaveChangesAsync();
                HttpContext.Session.Remove("KioskCommitteeId");
                HttpContext.Session.Remove("VoterId");
                return RedirectToAction("UnlockKiosk");
            }
            
            TempData["Error"] = "كلمة المرور غير صحيحة. لم يتم إغلاق اللجنة.";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Index()
        {
            var committeeId = HttpContext.Session.GetInt32("KioskCommitteeId");
            if (committeeId == null) return RedirectToAction(nameof(UnlockKiosk));

            var committee = await _context.Committees
                .Include(c => c.Election)
                .FirstOrDefaultAsync(c => c.Id == committeeId.Value);

            if (committee == null || committee.Status == CommitteeStatus.Closed)
            {
                HttpContext.Session.Remove("KioskCommitteeId");
                return RedirectToAction(nameof(UnlockKiosk));
            }

            return View(committee.Election);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Authenticate(string loginIdentifier)
        {
            var committeeId = HttpContext.Session.GetInt32("KioskCommitteeId");
            if (committeeId == null) return RedirectToAction(nameof(UnlockKiosk));

            var voter = await _context.Voters
                .Include(v => v.Committee)
                .FirstOrDefaultAsync(v => v.LoginIdentifier == loginIdentifier && v.CommitteeId == committeeId.Value);

            if (voter == null)
            {
                TempData["Error"] = "رقم البطاقة غير صحيح أو غير تابع لهذه اللجنة.";
                return RedirectToAction(nameof(Index));
            }

            if (voter.Committee.Status == CommitteeStatus.Closed)
            {
                TempData["Error"] = "هذه اللجنة مغلقة.";
                return RedirectToAction(nameof(UnlockKiosk));
            }

            // ROLLING CARDS ALGORITHM
            // If the difference is <= CooldownThreshold, it's on cooldown
            if ((voter.Committee.TotalVotesCast - voter.LastUsedSequence) <= voter.Committee.CooldownThreshold)
            {
                TempData["Error"] = "هذه البطاقة قيد التبريد ولم يمر عليها العدد الكافي من الناخبين. يرجى استخدام بطاقة أخرى.";
                return RedirectToAction(nameof(Index));
            }

            HttpContext.Session.SetInt32("VoterId", voter.Id);
            return RedirectToAction(nameof(Vote));
        }

        public async Task<IActionResult> Vote()
        {
            var voterId = HttpContext.Session.GetInt32("VoterId");
            var committeeId = HttpContext.Session.GetInt32("KioskCommitteeId");
            if (voterId == null || committeeId == null) return RedirectToAction(nameof(Index));

            var voter = await _context.Voters
                .Include(v => v.Committee)
                .ThenInclude(c => c.Election)
                .ThenInclude(e => e.Categories)
                .ThenInclude(cat => cat.Candidates)
                .FirstOrDefaultAsync(v => v.Id == voterId.Value && v.CommitteeId == committeeId.Value);

            if (voter == null) return RedirectToAction(nameof(Index));

            return View(voter.Committee.Election);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SubmitVote([FromBody] VoteSubmissionModel model)
        {
            var voterId = HttpContext.Session.GetInt32("VoterId");
            var committeeId = HttpContext.Session.GetInt32("KioskCommitteeId");
            if (voterId == null || committeeId == null) return BadRequest("Session expired");

            var voter = await _context.Voters.Include(v => v.Committee).ThenInclude(c => c.Election).FirstOrDefaultAsync(v => v.Id == voterId.Value);
            if (voter == null) return BadRequest("Invalid voter");

            // Ensure not bypassing cooldown by resubmitting
            if ((voter.Committee.TotalVotesCast - voter.LastUsedSequence) <= voter.Committee.CooldownThreshold) return BadRequest("Card is on cooldown");

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                foreach (var selection in model.Selections)
                {
                    _context.VoteRecords.Add(new VoteRecord
                    {
                        VoterId = voter.Id,
                        CandidateId = selection.CandidateId,
                        Selection = selection.IsSelected
                    });
                }

                // ROLLING CARDS ALGORITHM
                voter.Committee.TotalVotesCast++;
                voter.LastUsedSequence = voter.Committee.TotalVotesCast;

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                HttpContext.Session.Remove("VoterId");

                return Json(new { success = true, printCard = voter.Committee.Election.PrintCardAfterVote, voterId = voter.Id });
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                return StatusCode(500, "Internal server error during voting transaction.");
            }
        }

        public async Task<IActionResult> PrintCard(int id)
        {
            var voter = await _context.Voters
                .Include(v => v.VoteRecords)
                .ThenInclude(vr => vr.Candidate)
                .ThenInclude(c => c.Position)
                .Include(v => v.Committee)
                .ThenInclude(c => c.Election)
                .FirstOrDefaultAsync(v => v.Id == id);

            if (voter == null) return NotFound();

            return View(voter);
        }
    }

    public class VoteSubmissionModel
    {
        public List<CandidateSelection> Selections { get; set; } = new List<CandidateSelection>();
    }

    public class CandidateSelection
    {
        public int CandidateId { get; set; }
        public bool IsSelected { get; set; }
    }
}
