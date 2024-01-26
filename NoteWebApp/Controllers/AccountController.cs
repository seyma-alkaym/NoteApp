using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using NoteWebApp.Models;
using NoteWebApp.Repository;
using NoteWebApp.ViewModels;

namespace NoteWebApp.Controllers
{
    public class AccountController(SignInManager<AppUser> signInManager, 
        UserManager<AppUser> userManager, 
        IAccountRepository accountRepository) : Controller
    {
        private readonly SignInManager<AppUser> signInManager = signInManager;
        private readonly UserManager<AppUser> userManager = userManager;
        private readonly IAccountRepository accountRepository = accountRepository;

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginVM model)
        {
            if (ModelState.IsValid)
            {
                var result = await signInManager.PasswordSignInAsync(model.Username!, model.Password!, model.RememberMe, false);

                if (result.Succeeded)
                {
                    return RedirectToAction("GetAllNotes", "Note");
                }
                ModelState.AddModelError("", "Invalid login attempt");
                return View(model);
            }
            return View(model);
        }

        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterVM model)
        {
            if (ModelState.IsValid)
            {
                var _user = await userManager.FindByEmailAsync(model.Email!);
                if (_user != null)
                {
                    TempData["Error"] = "This email address is already in use.";
                    return View(model);
                }

                AppUser user = new()
                {
                    Name = model.Name,
                    UserName = model.Email,
                    Email = model.Email,
                };

                var result = await userManager.CreateAsync(user, model.Password!);

                if (result.Succeeded)
                {
                    await signInManager.SignInAsync(user, false);

                    return RedirectToAction("GetAllNotes", "Note");
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }

            return View(model);
        }

        public async Task<IActionResult> Logout()
        {
            await signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        [HttpGet("fotgot-password")]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost("fotgot-password")]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordVM model)
        {
            if (ModelState.IsValid)
            {
                var _user = await userManager.FindByEmailAsync(model.Email!);
                if (_user != null)
                {
                    await accountRepository.GenerateForgotPasswordTokenAsync(_user);
                }

                ModelState.Clear();
                model.EmailSent = true;
            }
            return View(model);
        }

        [HttpGet("reset-password")]
        public IActionResult ResetPassword(string uid, string token)
        {
            RestPasswordVM resetPasswordVM = new RestPasswordVM
            {
                Token = token,
                UserId = uid
            };
            return View(resetPasswordVM);
        }

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword(RestPasswordVM model)
        {
            if (ModelState.IsValid)
            {
                model.Token = model.Token!.Replace(' ', '+');
                var result = await accountRepository.ResetPasswordAsync(model);
                
                if (result.Succeeded)
                {
                    ModelState.Clear();
                    model.IsSuccess = true;
                    return View(model);
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }
            return View(model);
        }
    }
}
