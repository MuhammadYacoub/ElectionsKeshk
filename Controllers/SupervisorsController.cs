using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using AdvancedVotingSystem.Models;
using AdvancedVotingSystem.Data;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using ClosedXML.Excel;
using System.IO;
using System;
using System.Collections.Generic;

namespace AdvancedVotingSystem.Controllers
{
    // Optionally add [Authorize(Roles = "Admin")] here later
    public class SupervisorsController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ApplicationDbContext _context;

        public SupervisorsController(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, ApplicationDbContext context)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            // Get users with their related Election included
            var allUsers = await _context.Users.Include(u => u.Election).ToListAsync();

            var viewModels = new List<SupervisorViewModel>();
            foreach (var user in allUsers)
            {
                var roles = await _userManager.GetRolesAsync(user);
                if (roles.Contains("Supervisor") || roles.Contains("CommitteeHead"))
                {
                    viewModels.Add(new SupervisorViewModel
                    {
                        Id = user.Id,
                        FullName = user.FullName,
                        Surname = user.Surname,
                        NationalId = user.NationalId,
                        UserName = user.UserName ?? string.Empty,
                        RoleName = roles.FirstOrDefault(r => r == "Supervisor" || r == "CommitteeHead") ?? "Unknown",
                        ElectionName = user.Election?.Name
                    });
                }
            }

            // Also pass distinct elections for the filter
            ViewBag.Elections = await _context.Elections.Select(e => e.Name).Distinct().ToListAsync();

            return View(viewModels);
        }

        public async Task<IActionResult> Create()
        {
            ViewBag.Elections = await _context.Elections.ToListAsync();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateSupervisorViewModel model)
        {
            string generatedUserName = model.NationalId;

            if (string.IsNullOrEmpty(model.NationalId))
            {
                // Clear validation errors for auto-generated fields
                ModelState.Remove("NationalId");
                ModelState.Remove("Password");

                generatedUserName = "SUP_" + Guid.NewGuid().ToString("N").Substring(0, 8);
                model.NationalId = ""; // Leave NationalId empty
                model.Password = generatedUserName; // Make the password the same as the generated username so they can log in
            }

            if (ModelState.IsValid)
            {

                var user = new ApplicationUser
                {
                    UserName = generatedUserName,
                    NationalId = model.NationalId,
                    FullName = model.FullName,
                    Surname = model.Surname,
                    Gender = model.Gender,
                    ElectionId = model.ElectionId,
                    EmailConfirmed = true
                };

                var result = await _userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(user, model.RoleName);
                    return RedirectToAction(nameof(Index));
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            ViewBag.Elections = await _context.Elections.ToListAsync();
            return View(model);
        }

        public async Task<IActionResult> Edit(string id)
        {
            if (string.IsNullOrEmpty(id)) return NotFound();

            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound();

            var roles = await _userManager.GetRolesAsync(user);

            var model = new EditSupervisorViewModel
            {
                Id = user.Id,
                FullName = user.FullName,
                Surname = user.Surname,
                Gender = user.Gender,
                RoleName = roles.FirstOrDefault() ?? string.Empty,
                ElectionId = user.ElectionId,
                NationalId = user.NationalId
            };

            ViewBag.Elections = await _context.Elections.ToListAsync();
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(EditSupervisorViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByIdAsync(model.Id);
                if (user == null) return NotFound();

                user.FullName = model.FullName;
                user.Surname = model.Surname;
                user.Gender = model.Gender;
                user.ElectionId = model.ElectionId;
                
                // If it was a CommitteeHead and is still a CommitteeHead, NationalId can be updated (carefully)
                if (model.RoleName == "CommitteeHead" && !string.IsNullOrEmpty(model.NationalId))
                {
                    user.NationalId = model.NationalId;
                    user.UserName = model.NationalId; // Since UserName = NationalId for CommitteeHead
                }
                
                var result = await _userManager.UpdateAsync(user);

                if (result.Succeeded)
                {
                    // Update Role
                    var currentRoles = await _userManager.GetRolesAsync(user);
                    if (!currentRoles.Contains(model.RoleName))
                    {
                        await _userManager.RemoveFromRolesAsync(user, currentRoles);
                        await _userManager.AddToRoleAsync(user, model.RoleName);
                    }

                    // Update Password if provided (only for CommitteeHead typically)
                    if (!string.IsNullOrEmpty(model.Password))
                    {
                        var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                        await _userManager.ResetPasswordAsync(user, token, model.Password);
                    }

                    return RedirectToAction(nameof(Index));
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            ViewBag.Elections = await _context.Elections.ToListAsync();
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user != null)
            {
                var result = await _userManager.DeleteAsync(user);
                if (!result.Succeeded)
                {
                    TempData["ErrorMessage"] = "فشل الحذف. ربما يكون المستخدم مرتبطاً ببيانات أخرى في النظام.";
                }
                else
                {
                    TempData["SuccessMessage"] = "تم حذف الحساب بنجاح.";
                }
            }
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> BulkDelete(List<string> selectedIds)
        {
            if (selectedIds == null || !selectedIds.Any())
            {
                TempData["ErrorMessage"] = "لم يتم تحديد أي حسابات للحذف.";
                return RedirectToAction(nameof(Index));
            }

            int success = 0;
            foreach (var id in selectedIds)
            {
                var user = await _userManager.FindByIdAsync(id);
                if (user != null)
                {
                    var result = await _userManager.DeleteAsync(user);
                    if (result.Succeeded) success++;
                }
            }

            TempData["SuccessMessage"] = $"تم حذف {success} حساب(ات) بنجاح.";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public IActionResult DownloadTemplate()
        {
            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Supervisors");
                worksheet.Cell(1, 1).Value = "اللقب";
                worksheet.Cell(1, 2).Value = "الاسم";
                worksheet.Cell(1, 3).Value = "النوع";
                worksheet.Cell(1, 4).Value = "الرقم القومي";
                worksheet.Cell(1, 5).Value = "اسم المستخدم";
                worksheet.Cell(1, 6).Value = "كلمة المرور";

                var header = worksheet.Range("A1:F1");
                header.Style.Font.Bold = true;
                header.Style.Fill.BackgroundColor = XLColor.LightBlue;

                worksheet.Columns().AdjustToContents();

                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    var content = stream.ToArray();
                    return File(content, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "SupervisorsTemplate.xlsx");
                }
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ImportExcel(IFormFile excelFile, string excelElectionName, string excelRoleName)
        {
            if (excelFile == null || excelFile.Length == 0 || string.IsNullOrEmpty(excelRoleName))
            {
                TempData["ErrorMessage"] = "برجاء اختيار ملف إكسيل صالح وتحديد الدور المطلوب.";
                return RedirectToAction(nameof(Index));
            }

            int? excelElectionId = null;
            if (!string.IsNullOrEmpty(excelElectionName))
            {
                var el = await _context.Elections.FirstOrDefaultAsync(e => e.Name == excelElectionName);
                if (el != null) excelElectionId = el.Id;
            }

            int successCount = 0;
            int errorCount = 0;
            var errorMessages = new List<string>();

            using (var stream = new MemoryStream())
            {
                await excelFile.CopyToAsync(stream);
                using (var workbook = new XLWorkbook(stream))
                {
                    var worksheet = workbook.Worksheet(1);
                    var rows = worksheet.RangeUsed().RowsUsed().Skip(1); // Skip header

                    foreach (var row in rows)
                    {
                        try
                        {
                            var surname = row.Cell(1).GetString().Trim();
                            var fullName = row.Cell(2).GetString().Trim();
                            var gender = row.Cell(3).GetString().Trim();
                            var nationalId = row.Cell(4).GetString().Trim();
                            var excelUserName = row.Cell(5).GetString().Trim();
                            var excelPassword = row.Cell(6).GetString().Trim();

                            if (string.IsNullOrEmpty(fullName))
                            {
                                errorCount++;
                                continue;
                            }

                            string roleName = excelRoleName == "CommitteeHead" ? "CommitteeHead" : "Supervisor";
                            
                            // Check duplicate NationalId if provided
                            if (!string.IsNullOrEmpty(nationalId))
                            {
                                var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.NationalId == nationalId);
                                if (existingUser != null)
                                {
                                    errorCount++;
                                    errorMessages.Add($"صف {row.RowNumber()}: الرقم القومي ({nationalId}) مسجل مسبقاً.");
                                    continue;
                                }
                            }

                            // Determine Username
                            string userName = !string.IsNullOrEmpty(excelUserName) ? excelUserName : nationalId;
                            if (string.IsNullOrEmpty(userName))
                            {
                                userName = "SUP_" + Guid.NewGuid().ToString("N").Substring(0, 8);
                            }

                            // Check duplicate FullName
                            var existingByName = await _context.Users.FirstOrDefaultAsync(u => u.FullName == fullName);
                            if (existingByName != null)
                            {
                                errorCount++;
                                errorMessages.Add($"صف {row.RowNumber()}: الاسم ({fullName}) مسجل مسبقاً.");
                                continue;
                            }

                            // Check duplicate Username
                            var existingByUserName = await _userManager.FindByNameAsync(userName);
                            if (existingByUserName != null)
                            {
                                errorCount++;
                                errorMessages.Add($"صف {row.RowNumber()}: اسم المستخدم ({userName}) مسجل مسبقاً.");
                                continue;
                            }

                            var user = new ApplicationUser
                            {
                                UserName = userName,
                                NationalId = nationalId,
                                FullName = fullName,
                                Surname = surname,
                                Gender = gender,
                                ElectionId = excelElectionId,
                                EmailConfirmed = true
                            };

                            string password = !string.IsNullOrEmpty(excelPassword) ? excelPassword : userName;
                            
                            var result = await _userManager.CreateAsync(user, password);
                            if (result.Succeeded)
                            {
                                await _userManager.AddToRoleAsync(user, roleName);
                                successCount++;
                            }
                            else
                            {
                                errorCount++;
                                errorMessages.Add($"صف {row.RowNumber()}: {string.Join(", ", result.Errors.Select(e => e.Description))}");
                            }
                        }
                        catch
                        {
                            errorCount++;
                            errorMessages.Add($"صف {row.RowNumber()}: حدث خطأ غير متوقع.");
                        }
                    }
                }
            }

            TempData["SuccessMessage"] = $"تم استيراد {successCount} حساب بنجاح. فشل {errorCount} حساب.";
            if (errorMessages.Any())
            {
                TempData["ErrorList"] = string.Join(" | ", errorMessages.Take(5)) + (errorMessages.Count > 5 ? "..." : "");
            }

            return RedirectToAction(nameof(Index));
        }
    }

    public class SupervisorViewModel
    {
        public string Id { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Surname { get; set; } = string.Empty;
        public string NationalId { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string RoleName { get; set; } = string.Empty;
        public string? ElectionName { get; set; }
    }

    public class CreateSupervisorViewModel
    {
        public string FullName { get; set; } = string.Empty;
        public string Surname { get; set; } = string.Empty;
        public string Gender { get; set; } = string.Empty;
        public string? NationalId { get; set; }
        public string? Password { get; set; }
        public string RoleName { get; set; } = string.Empty;
        public int? ElectionId { get; set; }
    }

    public class EditSupervisorViewModel
    {
        public string Id { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Surname { get; set; } = string.Empty;
        public string Gender { get; set; } = string.Empty;
        public string? NationalId { get; set; }
        public string? Password { get; set; }
        public string RoleName { get; set; } = string.Empty;
        public int? ElectionId { get; set; }
    }
}
