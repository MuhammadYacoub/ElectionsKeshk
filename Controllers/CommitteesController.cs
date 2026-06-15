using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ClosedXML.Excel;
using AdvancedVotingSystem.Data;
using AdvancedVotingSystem.Models;
using Microsoft.AspNetCore.Identity;

namespace AdvancedVotingSystem.Controllers
{
    public class CommitteesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public CommitteesController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Committees
        public async Task<IActionResult> Index(int? electionId)
        {
            ViewBag.Elections = await _context.Elections.ToListAsync();
            ViewBag.CurrentElectionId = electionId;

            var query = _context.Committees
                .Include(c => c.Election)
                .Include(c => c.Head)
                .Include(c => c.Supervisors)
                .AsQueryable();

            if (electionId.HasValue && electionId > 0)
            {
                query = query.Where(c => c.ElectionId == electionId);
            }

            var committees = await query.ToListAsync();
            return View(committees);
        }

        // GET: Committees/Create
        public async Task<IActionResult> Create()
        {
            ViewBag.Elections = await _context.Elections.ToListAsync();
            
            var allHeads = await _userManager.GetUsersInRoleAsync("CommitteeHead");
            var assignedHeads = await _context.Committees.Where(c => c.HeadId != null).Select(c => c.HeadId).ToListAsync();
            ViewBag.Heads = allHeads.Where(h => !assignedHeads.Contains(h.Id)).ToList();

            var allSupervisors = await _userManager.GetUsersInRoleAsync("Supervisor");
            ViewBag.Supervisors = allSupervisors.Where(s => s.CommitteeId == null).ToList();

            return View();
        }

        [HttpGet]
        public async Task<IActionResult> GetStaffByElection(int electionId)
        {
            var allHeads = await _userManager.GetUsersInRoleAsync("CommitteeHead");
            // Heads belonging to this election and not assigned
            var assignedHeads = await _context.Committees.Where(c => c.HeadId != null).Select(c => c.HeadId).ToListAsync();
            var heads = allHeads
                .Where(h => h.ElectionId == electionId && !assignedHeads.Contains(h.Id))
                .Select(h => new { h.Id, h.FullName, h.NationalId })
                .ToList();

            var allSupervisors = await _userManager.GetUsersInRoleAsync("Supervisor");
            var supervisors = allSupervisors
                .Where(s => s.ElectionId == electionId && s.CommitteeId == null)
                .Select(s => new { s.Id, s.FullName, s.NationalId })
                .ToList();

            return Json(new { heads, supervisors });
        }

        // POST: Committees/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Committee committee, List<string> supervisorIds, IFormFile? votersExcel, int? ManualRegisteredCount)
        {
            if (ModelState.IsValid)
            {
                var election = await _context.Elections.FindAsync(committee.ElectionId);
                if (election == null) return NotFound();

                using var transaction = await _context.Database.BeginTransactionAsync();
                try
                {
                    _context.Committees.Add(committee);
                    await _context.SaveChangesAsync();

                    if (supervisorIds != null && supervisorIds.Any())
                    {
                        foreach (var supId in supervisorIds)
                        {
                            var sup = await _userManager.FindByIdAsync(supId);
                            if (sup != null)
                            {
                                sup.CommitteeId = committee.Id;
                            }
                        }
                    }

                    // Ensure CooldownThreshold is saved (has a default of 3 but might be updated)
                    if (election.EntryMethod != AdvancedVotingSystem.Models.Enums.EntryMethod.UnknownCode)
                    {
                        committee.CooldownThreshold = 0; // Or whatever logic you want if it's only applicable to UnknownCode. Let's keep the user's input.
                    }

                    // Handle Voters
                    if (votersExcel != null && votersExcel.Length > 0)
                    {
                        using (var stream = new MemoryStream())
                        {
                            await votersExcel.CopyToAsync(stream);
                            using (var workbook = new XLWorkbook(stream))
                            {
                                var worksheet = workbook.Worksheet(1);
                                var rows = worksheet.RangeUsed().RowsUsed().Skip(1); // skip header
                                foreach (var row in rows)
                                {
                                    if (election.EntryMethod == AdvancedVotingSystem.Models.Enums.EntryMethod.UnknownCode)
                                    {
                                        // Case UnknownCode: Read Col A (LoginCodes from physical cards). No Names.
                                        string loginId = row.Cell(1).GetString().Trim();
                                        if (string.IsNullOrEmpty(loginId)) continue;

                                        _context.Voters.Add(new Voter
                                        {
                                            FullName = "غير معرف", // Can't be null
                                            LoginIdentifier = loginId,
                                            CommitteeId = committee.Id
                                        });
                                        committee.RegisteredVotersCount++;
                                    }
                                    else if (election.EntryMethod == AdvancedVotingSystem.Models.Enums.EntryMethod.KnownCode)
                                    {
                                        // Case KnownCode: Read Col A (Name). Generate 6-digit LoginCode.
                                        string name = row.Cell(1).GetString().Trim();
                                        if (string.IsNullOrEmpty(name)) continue;

                                        _context.Voters.Add(new Voter
                                        {
                                            FullName = name,
                                            LoginIdentifier = GenerateUniqueCode(),
                                            CommitteeId = committee.Id
                                        });
                                        committee.RegisteredVotersCount++;
                                    }
                                    else
                                    {
                                        // Case NationalId: Read Col A (Name) and Col B (NationalId).
                                        string name = row.Cell(1).GetString().Trim();
                                        string natId = row.Cell(2).GetString().Trim();
                                        if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(natId)) continue;

                                        _context.Voters.Add(new Voter
                                        {
                                            FullName = name,
                                            NationalId = natId,
                                            LoginIdentifier = natId,
                                            CommitteeId = committee.Id
                                        });
                                        committee.RegisteredVotersCount++;
                                    }
                                }
                            }
                        }
                    }

                    if (election.EntryMethod == AdvancedVotingSystem.Models.Enums.EntryMethod.UnknownCode)
                    {
                        committee.RegisteredVotersCount = ManualRegisteredCount ?? 0;
                    }

                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();

                    TempData["SuccessMessage"] = "تم إنشاء اللجنة وحفظ فريق العمل والناخبين بنجاح.";
                    return RedirectToAction(nameof(Create));
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    TempData["ErrorMessage"] = "حدث خطأ أثناء معالجة البيانات: " + ex.Message;
                }
            }

            ViewBag.Elections = await _context.Elections.ToListAsync();
            return View(committee);
        }

        [HttpGet]
        public async Task<IActionResult> DownloadExcelTemplate(int electionId)
        {
            var election = await _context.Elections.FindAsync(electionId);
            if (election == null) return NotFound();

            using var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("Template");

            if (election.EntryMethod == AdvancedVotingSystem.Models.Enums.EntryMethod.NationalId)
            {
                worksheet.Cell(1, 1).Value = "الاسم";
                worksheet.Cell(1, 2).Value = "الرقم القومي";
            }
            else if (election.EntryMethod == AdvancedVotingSystem.Models.Enums.EntryMethod.KnownCode)
            {
                worksheet.Cell(1, 1).Value = "الاسم";
            }
            else // UnknownCode
            {
                worksheet.Cell(1, 1).Value = "كود البطاقة المطبوعة";
            }

            var headerRow = worksheet.Row(1);
            headerRow.Style.Font.Bold = true;
            headerRow.Style.Fill.BackgroundColor = XLColor.LightGray;

            worksheet.Columns().AdjustToContents();

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"Template_{election.Id}.xlsx");
        }

        [HttpGet]
        public async Task<IActionResult> GetCommitteesByElection(int electionId)
        {
            var committees = await _context.Committees
                .Where(c => c.ElectionId == electionId)
                .Select(c => new { id = c.Id, name = c.Name })
                .ToListAsync();
            return Json(committees);
        }

        [HttpGet]
        public async Task<IActionResult> VotersList(int? electionId, int? committeeId)
        {
            ViewBag.Elections = await _context.Elections.ToListAsync();
            ViewBag.CurrentElectionId = electionId;
            ViewBag.CurrentCommitteeId = committeeId;

            Committee selectedCommittee = null;
            if (committeeId.HasValue)
            {
                selectedCommittee = await _context.Committees
                    .Include(c => c.Election)
                    .Include(c => c.Voters)
                    .FirstOrDefaultAsync(c => c.Id == committeeId);
            }
            return View(selectedCommittee);
        }

        [HttpGet]
        public async Task<IActionResult> ExportVotersToExcel(int committeeId)
        {
            var committee = await _context.Committees
                .Include(c => c.Election)
                .Include(c => c.Voters)
                .FirstOrDefaultAsync(c => c.Id == committeeId);

            if (committee == null) return NotFound();

            using var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add(committee.Name.Length > 31 ? committee.Name.Substring(0, 31) : committee.Name);
            worksheet.RightToLeft = true;

            var headerRow = worksheet.Row(1);
            headerRow.Style.Font.Bold = true;
            headerRow.Style.Font.FontColor = XLColor.White;
            headerRow.Style.Fill.BackgroundColor = XLColor.FromHtml("#1E3A8A");
            headerRow.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

            if (committee.Election.EntryMethod == AdvancedVotingSystem.Models.Enums.EntryMethod.NationalId)
            {
                worksheet.Cell(1, 1).Value = "م";
                worksheet.Cell(1, 2).Value = "اسم الناخب";
                worksheet.Cell(1, 3).Value = "الرقم القومي";
                worksheet.Cell(1, 4).Value = "الباركود";
            }
            else if (committee.Election.EntryMethod == AdvancedVotingSystem.Models.Enums.EntryMethod.KnownCode)
            {
                worksheet.Cell(1, 1).Value = "م";
                worksheet.Cell(1, 2).Value = "اسم الناخب";
                worksheet.Cell(1, 3).Value = "كود الدخول السري";
                worksheet.Cell(1, 4).Value = "الباركود";
            }
            else
            {
                worksheet.Cell(1, 1).Value = "م";
                worksheet.Cell(1, 2).Value = "رقم البطاقة (الكود)";
                worksheet.Cell(1, 3).Value = "الباركود";
            }

            int rowIdx = 2;
            int counter = 1;
            foreach (var voter in committee.Voters)
            {
                worksheet.Cell(rowIdx, 1).Value = counter;
                worksheet.Row(rowIdx).Height = 45;
                
                if (committee.Election.EntryMethod == AdvancedVotingSystem.Models.Enums.EntryMethod.NationalId)
                {
                    worksheet.Cell(rowIdx, 2).Value = voter.FullName;
                    worksheet.Cell(rowIdx, 3).Value = "'" + voter.NationalId; // Prevent excel scientific notation
                    
                    var barcodeCell = worksheet.Cell(rowIdx, 4);
                    barcodeCell.Value = "*" + voter.LoginIdentifier + "*"; 
                    barcodeCell.Style.Font.FontName = "Free 3 of 9";
                    barcodeCell.Style.Font.FontSize = 36;
                    barcodeCell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                }
                else if (committee.Election.EntryMethod == AdvancedVotingSystem.Models.Enums.EntryMethod.KnownCode)
                {
                    worksheet.Cell(rowIdx, 2).Value = voter.FullName;
                    worksheet.Cell(rowIdx, 3).Value = "'" + voter.LoginIdentifier;
                    
                    var barcodeCell = worksheet.Cell(rowIdx, 4);
                    barcodeCell.Value = "*" + voter.LoginIdentifier + "*";
                    barcodeCell.Style.Font.FontName = "Free 3 of 9";
                    barcodeCell.Style.Font.FontSize = 36;
                    barcodeCell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                }
                else
                {
                    worksheet.Cell(rowIdx, 2).Value = "'" + voter.LoginIdentifier;
                    
                    var barcodeCell = worksheet.Cell(rowIdx, 3);
                    barcodeCell.Value = "*" + voter.LoginIdentifier + "*";
                    barcodeCell.Style.Font.FontName = "Free 3 of 9";
                    barcodeCell.Style.Font.FontSize = 36;
                    barcodeCell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                }

                rowIdx++;
                counter++;
            }

            // Styling data rows
            var dataRange = worksheet.Range(1, 1, rowIdx - 1, committee.Election.EntryMethod == AdvancedVotingSystem.Models.Enums.EntryMethod.UnknownCode ? 3 : 4);
            dataRange.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            dataRange.Style.Border.InsideBorder = XLBorderStyleValues.Thin;
            dataRange.Style.Border.OutsideBorderColor = XLColor.LightGray;
            dataRange.Style.Border.InsideBorderColor = XLColor.LightGray;
            dataRange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

            worksheet.Columns().AdjustToContents();
            
            // Override barcode column width so it's not too squished or too wide
            int barcodeColIdx = committee.Election.EntryMethod == AdvancedVotingSystem.Models.Enums.EntryMethod.UnknownCode ? 3 : 4;
            worksheet.Column(barcodeColIdx).Width = 25;

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            var safeName = string.Join("_", committee.Name.Split(Path.GetInvalidFileNameChars()));
            return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"Voters_{safeName}.xlsx");
        }

        private string GenerateUniqueCode()
        {
            var random = new Random();
            return random.Next(100000, 999999).ToString();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var committee = await _context.Committees
                .Include(c => c.Voters)
                .Include(c => c.Supervisors)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (committee == null) return NotFound();

            if (committee.TotalVotesCast > 0)
            {
                TempData["ErrorMessage"] = "لا يمكن حذف هذه اللجنة لوجود أصوات مسجلة عليها فعلياً.";
                return RedirectToAction(nameof(Index));
            }

            foreach (var sup in committee.Supervisors)
            {
                sup.CommitteeId = null;
            }

            _context.Voters.RemoveRange(committee.Voters);
            _context.Committees.Remove(committee);
            
            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "تم حذف اللجنة بنجاح.";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var committee = await _context.Committees
                .Include(c => c.Supervisors)
                .Include(c => c.Election)
                .FirstOrDefaultAsync(c => c.Id == id);
                
            if (committee == null) return NotFound();

            ViewBag.Elections = await _context.Elections.ToListAsync();
            
            var allHeads = await _userManager.GetUsersInRoleAsync("CommitteeHead");
            var assignedHeads = await _context.Committees.Where(c => c.HeadId != null && c.Id != id).Select(c => c.HeadId).ToListAsync();
            ViewBag.Heads = allHeads.Where(h => !assignedHeads.Contains(h.Id) && h.ElectionId == committee.ElectionId).ToList();

            var allSupervisors = await _userManager.GetUsersInRoleAsync("Supervisor");
            ViewBag.Supervisors = allSupervisors.Where(s => (s.CommitteeId == null || s.CommitteeId == id) && s.ElectionId == committee.ElectionId).ToList();

            return View(committee);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Committee committee, List<string> supervisorIds, int? ManualRegisteredCount)
        {
            if (id != committee.Id) return NotFound();

            var existingCommittee = await _context.Committees
                .Include(c => c.Supervisors)
                .Include(c => c.Election)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (existingCommittee == null) return NotFound();

            if (ModelState.IsValid)
            {
                using var transaction = await _context.Database.BeginTransactionAsync();
                try
                {
                    existingCommittee.Name = committee.Name;
                    existingCommittee.HeadId = committee.HeadId;

                    if (existingCommittee.Election.EntryMethod == AdvancedVotingSystem.Models.Enums.EntryMethod.UnknownCode)
                    {
                        existingCommittee.CooldownThreshold = committee.CooldownThreshold;
                        existingCommittee.RegisteredVotersCount = ManualRegisteredCount ?? existingCommittee.RegisteredVotersCount;
                    }

                    // Update Supervisors
                    foreach (var sup in existingCommittee.Supervisors.ToList())
                    {
                        if (supervisorIds == null || !supervisorIds.Contains(sup.Id))
                        {
                            sup.CommitteeId = null;
                        }
                    }

                    if (supervisorIds != null)
                    {
                        foreach (var supId in supervisorIds)
                        {
                            if (!existingCommittee.Supervisors.Any(s => s.Id == supId))
                            {
                                var sup = await _userManager.FindByIdAsync(supId);
                                if (sup != null)
                                {
                                    sup.CommitteeId = existingCommittee.Id;
                                }
                            }
                        }
                    }

                    _context.Update(existingCommittee);
                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();

                    TempData["SuccessMessage"] = "تم تعديل بيانات اللجنة بنجاح.";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    TempData["ErrorMessage"] = "حدث خطأ أثناء تعديل البيانات: " + ex.Message;
                }
            }

            ViewBag.Elections = await _context.Elections.ToListAsync();
            
            var allHeads = await _userManager.GetUsersInRoleAsync("CommitteeHead");
            var assignedHeads = await _context.Committees.Where(c => c.HeadId != null && c.Id != id).Select(c => c.HeadId).ToListAsync();
            ViewBag.Heads = allHeads.Where(h => !assignedHeads.Contains(h.Id) && h.ElectionId == existingCommittee.ElectionId).ToList();

            var allSupervisors = await _userManager.GetUsersInRoleAsync("Supervisor");
            ViewBag.Supervisors = allSupervisors.Where(s => (s.CommitteeId == null || s.CommitteeId == id) && s.ElectionId == existingCommittee.ElectionId).ToList();

            return View(existingCommittee);
        }
    }
}
