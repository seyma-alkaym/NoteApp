
using Microsoft.AspNetCore.Identity;
using NoteWebApp.Models;
using NoteWebApp.ViewModels;

namespace NoteWebApp.Repository
{
    public interface IAccountRepository
    {
        Task GenerateForgotPasswordTokenAsync(AppUser user);
        Task<IdentityResult> ResetPasswordAsync(RestPasswordVM model);
    }
}
