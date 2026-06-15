using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using AdvancedVotingSystem.Models;
using AdvancedVotingSystem.Models.ViewModels;
using System.Threading.Tasks;

namespace AdvancedVotingSystem.Controllers
{
    public class AuthController : Controller
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;

        public AuthController(SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager)
        {
            _signInManager = signInManager;
            _userManager = userManager;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // Identity SignIn using NationalId (which is stored as UserName)
            var result = await _signInManager.PasswordSignInAsync(model.Username, model.Password, isPersistent: model.RememberMe, lockoutOnFailure: false);
            
            if (result.Succeeded)
            {
                var user = await _userManager.FindByNameAsync(model.Username);
                if (user != null)
                {
                    var roles = await _userManager.GetRolesAsync(user);

                    if (roles.Contains("Admin"))
                    {
                        return RedirectToAction("Index", "Admin");
                    }
                    else if (roles.Contains("Supervisor") || roles.Contains("CommitteeHead"))
                    {
                        return RedirectToAction("Index", "Supervisor"); // Assuming Supervisor dashboard exists
                    }
                }

                return RedirectToAction("Index", "Home");
            }

            ViewBag.Error = "اسم المستخدم أو كلمة المرور غير صحيحة"; // Localized error message
            return View(model);
        }

        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }
    }
}
