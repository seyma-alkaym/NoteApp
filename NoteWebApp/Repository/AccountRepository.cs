using Microsoft.AspNetCore.Identity;
using NoteWebApp.Models;
using NoteWebApp.Service;
using NoteWebApp.ViewModels;

namespace NoteWebApp.Repository
{
    public class AccountRepository(UserManager<AppUser> userManager, 
        IConfiguration configuration,
        IEmailService emailService) : IAccountRepository
    {
        private readonly UserManager<AppUser> userManager = userManager;
        private readonly IConfiguration configuration = configuration;
        private readonly IEmailService emailService = emailService;

        public async Task GenerateForgotPasswordTokenAsync(AppUser user)
        {
            var token = await userManager.GeneratePasswordResetTokenAsync(user);
            if (!string.IsNullOrEmpty(token))
            {
                await SendForgotPasswordEmail(user, token);
            }
        }


        public async Task<IdentityResult> ResetPasswordAsync(RestPasswordVM model)
        {
            return await userManager.ResetPasswordAsync(
                await userManager.FindByIdAsync(model.UserId), 
                model.Token!, 
                model.NewPassword!);
        }

        private async Task SendForgotPasswordEmail(AppUser user, string token)
        {
            string appDomain = configuration.GetSection("Application:AppDomain").Value!;
            string confirmationLink = configuration.GetSection("Application:ForgotPassword").Value!;

            UserEmailOptions options = new UserEmailOptions
            {
                ToEmails = new List<string>() { user.Email! },
                PlaceHolders = new List<KeyValuePair<string, string>>()
                {
                    new KeyValuePair<string, string>("{{UserName}}", user.Name!),
                    new KeyValuePair<string, string>("{{Link}}",
                        string.Format(appDomain + confirmationLink, user.Id, token))
                }
            };

            await emailService.SendEmailForForgotPassword(options);
        }

    }
}
