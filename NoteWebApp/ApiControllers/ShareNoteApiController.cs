using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NoteWebApp.Data;
using NoteWebApp.Models;
using System.Text.Json.Serialization;
using System.Text.Json;

namespace NoteWebApp.ApiControllers
{
    public class ShareNoteApiController(AppDbContext context) : AppApiController
    {
        private readonly AppDbContext context = context;

        [HttpGet]
        [AllowAnonymous] // Allow access without authentication
        public IActionResult PublicNotes()
        {
            var options = new JsonSerializerOptions
            {
                ReferenceHandler = ReferenceHandler.Preserve,
                WriteIndented = true
            };

            var notes = context.Notes
                .Where(n => !n.IsDeleted && n.IsPublic)
                .OrderByDescending(u => u.DateCreated)
                .Include(n => n.AppUser)
                .ToList();

            var json = JsonSerializer.Serialize(notes, options);

            return Content(json, "application/json");
        }

        [HttpGet("{id}")]
        [AllowAnonymous] // Allow access without authentication
        public IActionResult PublicView(Guid id)
        {
            var options = new JsonSerializerOptions
            {
                ReferenceHandler = ReferenceHandler.Preserve
            };

            var note = context.Notes
                .Include(n => n.AppUser)
                .FirstOrDefault(n => n.Id == id);

            if (note == null || note.IsDeleted)
            {
                return NotFound();
            }

            note.IsPublic = true;

            context.SaveChanges();

            var json = JsonSerializer.Serialize(note, options);

            return Content(json, "application/json");
        }
    }
}
