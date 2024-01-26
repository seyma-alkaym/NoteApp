using NoteWebApp.Models;

namespace NoteWebApp.Service
{
    public interface IEmailService
    {
        Task SendEmailForForgotPassword(UserEmailOptions options);
        Task SendTestEmail(UserEmailOptions userEmailOptions);
    }
}