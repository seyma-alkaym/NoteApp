using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using NoteWebApp.Models;
using NoteWebApp.Repository;
using NoteWebApp.ViewModels;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace NoteWebApp.ApiControllers
{
    public class AccountApiController(
        SignInManager<AppUser> signInManager,
        UserManager<AppUser> userManager,
        IAccountRepository accountRepository,
        IConfiguration configuration
        ) : AppApiController
    {
        private readonly SignInManager<AppUser> signInManager = signInManager;
        private readonly UserManager<AppUser> userManager = userManager;
        private readonly IAccountRepository accountRepository = accountRepository;
        private readonly IConfiguration configuration = configuration;

        [HttpPost]
        public async Task<IActionResult> Login(LoginVM model)
        {
            var user = await userManager.FindByEmailAsync(model.Username!);

            if(user == null)
            {
                return BadRequest(new { Message = "Invalid login attempt" });
            }

            var result = await userManager.CheckPasswordAsync(user, model.Password!);

            if (!result)
            {
                return BadRequest(new { Message = "Invalid login attempt" });
            }

            var claims = new Claim[]
                    {
                        new Claim(JwtRegisteredClaimNames.Sub, user.Id),
                        new Claim(JwtRegisteredClaimNames.Email, user.Email!),
                        new Claim(JwtRegisteredClaimNames.Name, user.Name!)
                    };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["AuthSettings:Key"]!));

            var token = new JwtSecurityToken(
                issuer: configuration["AuthSettings:Issuer"],
                audience: configuration["AuthSettings:Aduience"],
                claims: claims,
                expires: DateTime.Now.AddDays(1),
                signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256));

            string tokenAsString = new JwtSecurityTokenHandler().WriteToken(token);

            return Ok(new { Message = tokenAsString });
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterVM model)
        {
            if (ModelState.IsValid)
            {
                var existingUser = await userManager.FindByEmailAsync(model.Email!);
                if (existingUser != null)
                {
                    return BadRequest(new { Message = "This email address is already in use." });
                }

                var user = new AppUser
                {
                    Name = model.Name,
                    UserName = model.Email,
                    Email = model.Email,
                };

                var result = await userManager.CreateAsync(user, model.Password!);

                if (result.Succeeded)
                {
                    await signInManager.SignInAsync(user, false);

                    return Ok(new { Message = "Registration successful" });
                }

                return BadRequest(new { Errors = result.Errors.Select(e => e.Description) });
            }

            return BadRequest(new { Message = "Invalid request" });
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await signInManager.SignOutAsync();
            return Ok(new { Message = "Logout successful" });
        }

        [HttpPost(("forgot-password"))]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordVM model)
        {
            if (ModelState.IsValid)
            {
                var user = await userManager.FindByEmailAsync(model.Email!);
                if (user != null)
                {
                    await accountRepository.GenerateForgotPasswordTokenAsync(user);
                }

                ModelState.Clear();
                return Ok(new { Message = "Password reset email sent successfully" });
            }

            return BadRequest(ModelState);
        }

        [HttpGet("reset-password")]
        public IActionResult ResetPassword(string uid, string token)
        {
            RestPasswordVM resetPasswordVM = new RestPasswordVM
            {
                Token = token,
                UserId = uid
            };
            return Ok(resetPasswordVM);
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
                    return Ok(model);
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }

            return BadRequest(ModelState);
        }
    }
}
