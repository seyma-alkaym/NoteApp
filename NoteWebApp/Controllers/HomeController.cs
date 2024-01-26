using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NoteWebApp.Data;

namespace NoteWebApp.Controllers
{
    public class HomeController(AppDbContext context) : Controller
    {
        private readonly AppDbContext context = context;

        [Authorize]
        public IActionResult Index()
        {
            var user = context.Users
                .Include(u => u!.Notes!.Where(n => !n.IsDeleted))
                .FirstOrDefault(u => u.UserName == User!.Identity!.Name);

            return View(user?.Notes?.OrderByDescending(u => u!.DateCreated));
        }
    }
}
