using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NoteWebApp.Data;
using NoteWebApp.Models;

namespace NoteWebApp.Controllers
{
    public class ShareNoteController(AppDbContext context) : Controller
    {
        private readonly AppDbContext context = context;

        [HttpGet]
        [AllowAnonymous] // Allow access without authentication
        public IActionResult PublicView(Guid id)
        {
            var note = context.Notes
                .Include(n => n.AppUser)
                .FirstOrDefault(n => n.Id == id);

            if (note == null || note.IsDeleted)
            {
                return NotFound();
            }

            note.IsPublic = true;

            context.SaveChanges();           

            return View(note);
        }

        [HttpGet]
        [AllowAnonymous] // Allow access without authentication
        public IActionResult PublicNotes(Note model)
        {
            var notes = context.Notes
                .Where(n => !n.IsDeleted && n.IsPublic)
                .OrderByDescending(u => u!.DateCreated)
                .Include(n => n.AppUser)
                .ToList();
            
            return View(notes);
        }
    }
}
