using AdvancedVotingSystem.Data;
using AdvancedVotingSystem.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System.IO;

namespace AdvancedVotingSystem.Controllers
{
    [Authorize(Roles = "Admin")]
    public class ElectionsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _hostEnvironment;

        public ElectionsController(ApplicationDbContext context, IWebHostEnvironment hostEnvironment)
        {
            _context = context;
            _hostEnvironment = hostEnvironment;
        }

        public async Task<IActionResult> Index()
        {
            return View(await _context.Elections.ToListAsync());
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Election election, IFormFile? logoFile)
        {
            if (ModelState.IsValid)
            {
                if (logoFile != null && logoFile.Length > 0)
                {
                    var uploadsFolder = Path.Combine(_hostEnvironment.WebRootPath, "uploads", "logos");
                    if (!Directory.Exists(uploadsFolder))
                        Directory.CreateDirectory(uploadsFolder);
                    
                    var fileName = Guid.NewGuid().ToString() + Path.GetExtension(logoFile.FileName);
                    var filePath = Path.Combine(uploadsFolder, fileName);
                    
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await logoFile.CopyToAsync(stream);
                    }
                    
                    election.LogoPath = "/uploads/logos/" + fileName;
                }

                _context.Add(election);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(election);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var election = await _context.Elections.FindAsync(id);
            if (election == null) return NotFound();

            return View(election);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Election election, IFormFile? logoFile)
        {
            if (id != election.Id) return NotFound();

            if (ModelState.IsValid)
            {
                if (logoFile != null && logoFile.Length > 0)
                {
                    var uploadsFolder = Path.Combine(_hostEnvironment.WebRootPath, "uploads", "logos");
                    if (!Directory.Exists(uploadsFolder))
                        Directory.CreateDirectory(uploadsFolder);
                    
                    var fileName = Guid.NewGuid().ToString() + Path.GetExtension(logoFile.FileName);
                    var filePath = Path.Combine(uploadsFolder, fileName);
                    
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await logoFile.CopyToAsync(stream);
                    }
                    
                    election.LogoPath = "/uploads/logos/" + fileName;
                }

                _context.Update(election);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(election);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangeStatus(int id, AdvancedVotingSystem.Models.Enums.ElectionStatus newStatus)
        {
            var election = await _context.Elections.FindAsync(id);
            if (election == null) return NotFound();

            election.Status = newStatus;
            _context.Update(election);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        [HttpGet]
        public async Task<IActionResult> Structure(int id)
        {
            var election = await _context.Elections
                .Include(e => e.Positions)
                .ThenInclude(p => p.Candidates)
                .FirstOrDefaultAsync(e => e.Id == id);
            
            if (election == null) return NotFound();
            
            return View(election);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UploadBylaw(int electionId, IFormFile bylawFile)
        {
            var election = await _context.Elections.FindAsync(electionId);
            if (election == null) return NotFound();

            if (bylawFile != null && bylawFile.Length > 0)
            {
                var uploadsFolder = Path.Combine(_hostEnvironment.WebRootPath, "uploads", "bylaws");
                if (!Directory.Exists(uploadsFolder)) Directory.CreateDirectory(uploadsFolder);
                
                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(bylawFile.FileName);
                var filePath = Path.Combine(uploadsFolder, fileName);
                
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await bylawFile.CopyToAsync(stream);
                }
                
                election.BylawPdfPath = "/uploads/bylaws/" + fileName;
                _context.Update(election);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "تم رفع اللائحة بنجاح.";
            }

            return RedirectToAction(nameof(Structure), new { id = electionId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddPosition(int electionId, string name, int requiredCount)
        {
            var election = await _context.Elections.FindAsync(electionId);
            if (election == null) return NotFound();

            if (!string.IsNullOrEmpty(name) && requiredCount > 0)
            {
                var position = new Position
                {
                    Name = name,
                    RequiredCount = requiredCount,
                    ElectionId = electionId
                };
                _context.Positions.Add(position);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "تم إضافة المقعد بنجاح.";
            }
            return RedirectToAction(nameof(Structure), new { id = electionId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddCandidate(int positionId, int electionId, string name, string? title, string? nickname, int? candidateNumber, IFormFile? candidateImage)
        {
            var position = await _context.Positions.FindAsync(positionId);
            if (position == null) return NotFound();

            if (!string.IsNullOrEmpty(name))
            {
                var candidate = new Candidate
                {
                    Name = name,
                    Title = title,
                    Nickname = nickname,
                    CandidateNumber = candidateNumber,
                    PositionId = positionId
                };

                if (candidateImage != null && candidateImage.Length > 0)
                {
                    var uploadsFolder = Path.Combine(_hostEnvironment.WebRootPath, "uploads", "candidates");
                    if (!Directory.Exists(uploadsFolder)) Directory.CreateDirectory(uploadsFolder);
                    
                    var fileName = Guid.NewGuid().ToString() + Path.GetExtension(candidateImage.FileName);
                    var filePath = Path.Combine(uploadsFolder, fileName);
                    
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await candidateImage.CopyToAsync(stream);
                    }
                    
                    candidate.ImagePath = "/uploads/candidates/" + fileName;
                }

                _context.Candidates.Add(candidate);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "تم إضافة المرشح بنجاح.";
            }
            
            return RedirectToAction(nameof(Structure), new { id = electionId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditCandidate(int id, int electionId, string name, string? title, string? nickname, int? candidateNumber, IFormFile? candidateImage)
        {
            var candidate = await _context.Candidates.FindAsync(id);
            if (candidate == null) return NotFound();

            if (!string.IsNullOrEmpty(name))
            {
                candidate.Name = name;
                candidate.Title = title;
                candidate.Nickname = nickname;
                candidate.CandidateNumber = candidateNumber;

                if (candidateImage != null && candidateImage.Length > 0)
                {
                    var uploadsFolder = Path.Combine(_hostEnvironment.WebRootPath, "uploads", "candidates");
                    if (!Directory.Exists(uploadsFolder)) Directory.CreateDirectory(uploadsFolder);
                    
                    var fileName = Guid.NewGuid().ToString() + Path.GetExtension(candidateImage.FileName);
                    var filePath = Path.Combine(uploadsFolder, fileName);
                    
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await candidateImage.CopyToAsync(stream);
                    }
                    
                    candidate.ImagePath = "/uploads/candidates/" + fileName;
                }

                _context.Update(candidate);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "تم تحديث بيانات المرشح بنجاح.";
            }
            
            return RedirectToAction(nameof(Structure), new { id = electionId });
        }

        [HttpGet]
        public IActionResult DownloadExcelTemplate()
        {
            using (var workbook = new ClosedXML.Excel.XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("المرشحين");
                worksheet.Cell(1, 1).Value = "الاسم";
                worksheet.Cell(1, 2).Value = "اللقب";
                worksheet.Cell(1, 3).Value = "اسم الشهرة";
                worksheet.Cell(1, 4).Value = "رقم المرشح";
                worksheet.Cell(1, 5).Value = "اسم ملف الصورة";

                var headerRow = worksheet.Row(1);
                headerRow.Style.Font.Bold = true;
                headerRow.Style.Fill.BackgroundColor = ClosedXML.Excel.XLColor.LightBlue;

                // Adjust column widths
                worksheet.Column(1).Width = 30;
                worksheet.Column(2).Width = 20;
                worksheet.Column(3).Width = 20;
                worksheet.Column(4).Width = 15;
                worksheet.Column(5).Width = 30;

                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    var content = stream.ToArray();
                    return File(content, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "CandidatesTemplate.xlsx");
                }
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ImportCandidatesExcel(int positionId, int electionId, IFormFile excelFile, List<IFormFile> candidateImages)
        {
            var position = await _context.Positions.FindAsync(positionId);
            if (position == null) return NotFound();

            if (excelFile == null || excelFile.Length == 0)
            {
                TempData["ErrorMessage"] = "يجب اختيار ملف الإكسيل.";
                return RedirectToAction(nameof(Structure), new { id = electionId });
            }

            try
            {
                using (var stream = new MemoryStream())
                {
                    await excelFile.CopyToAsync(stream);
                    using (var workbook = new ClosedXML.Excel.XLWorkbook(stream))
                    {
                        var worksheet = workbook.Worksheet(1);
                        var rows = worksheet.RangeUsed().RowsUsed().Skip(1); // Skip header row

                        int addedCount = 0;

                        foreach (var row in rows)
                        {
                            var name = row.Cell(1).GetString()?.Trim();
                            if (string.IsNullOrEmpty(name)) continue;

                            var title = row.Cell(2).GetString()?.Trim();
                            var nickname = row.Cell(3).GetString()?.Trim();
                            var candidateNumberString = row.Cell(4).GetString()?.Trim();
                            var imageFileName = row.Cell(5).GetString()?.Trim();
                            
                            int? candidateNumber = null;
                            if (int.TryParse(candidateNumberString, out int parsedNum))
                            {
                                candidateNumber = parsedNum;
                            }

                            var candidate = new Candidate
                            {
                                Name = name,
                                Title = title,
                                Nickname = nickname,
                                CandidateNumber = candidateNumber,
                                PositionId = positionId
                            };

                            if (!string.IsNullOrEmpty(imageFileName) && candidateImages != null && candidateImages.Any())
                            {
                                // Find image by exact name or without extension
                                var matchedImage = candidateImages.FirstOrDefault(i => 
                                    i.FileName.Equals(imageFileName, StringComparison.OrdinalIgnoreCase) ||
                                    Path.GetFileNameWithoutExtension(i.FileName).Equals(imageFileName, StringComparison.OrdinalIgnoreCase));

                                if (matchedImage != null && matchedImage.Length > 0)
                                {
                                    var uploadsFolder = Path.Combine(_hostEnvironment.WebRootPath, "uploads", "candidates");
                                    if (!Directory.Exists(uploadsFolder)) Directory.CreateDirectory(uploadsFolder);
                                    
                                    var newFileName = Guid.NewGuid().ToString() + Path.GetExtension(matchedImage.FileName);
                                    var filePath = Path.Combine(uploadsFolder, newFileName);
                                    
                                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                                    {
                                        await matchedImage.CopyToAsync(fileStream);
                                    }
                                    
                                    candidate.ImagePath = "/uploads/candidates/" + newFileName;
                                }
                            }

                            _context.Candidates.Add(candidate);
                            addedCount++;
                        }

                        await _context.SaveChangesAsync();
                        TempData["SuccessMessage"] = $"تم استيراد {addedCount} مرشح بنجاح.";
                    }
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "حدث خطأ أثناء قراءة ملف الإكسيل: " + ex.Message;
            }

            return RedirectToAction(nameof(Structure), new { id = electionId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var election = await _context.Elections
                .Include(e => e.Committees)
                .FirstOrDefaultAsync(e => e.Id == id);
                
            if (election == null) return NotFound();

            // Check if any committee has TotalVotesCast > 0
            bool hasVotes = election.Committees.Any(c => c.TotalVotesCast > 0);

            if (hasVotes)
            {
                TempData["ErrorMessage"] = "لا يمكن حذف هذه الانتخابات لوجود أصوات مسجلة عليها فعلياً.";
            }
            else
            {
                _context.Elections.Remove(election);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "تم حذف الانتخابات بنجاح.";
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
