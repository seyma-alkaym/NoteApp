using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NoteWebApp.Data;
using NoteWebApp.Models;
using NoteWebApp.ViewModels;
using System.Text.Json.Serialization;
using System.Text.Json;
using System.Security.Claims;
using NoteWebApp.Dto;

namespace NoteWebApp.ApiControllers
{
    [Authorize(AuthenticationSchemes = "Bearer")]
    public class NoteApiController(AppDbContext context,
        IWebHostEnvironment webHostEnvironment) : AppApiController
    {
        private readonly AppDbContext context = context;
        private readonly IWebHostEnvironment webHostEnvironment = webHostEnvironment;

        [HttpGet]
        public async Task<IActionResult> GetAllNotes()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var notes = await context.Notes.Where(n => n.AppUserId == userId).ToListAsync();

            //var orderedNotes = user?.Notes?.OrderByDescending(u => u!.DateCreated);

            var jsonSerializerOptions = new JsonSerializerOptions
            {
                ReferenceHandler = ReferenceHandler.Preserve,
                WriteIndented = true
            };

            var serializedNotes = JsonSerializer.Serialize(notes, jsonSerializerOptions);

            return Ok(serializedNotes);
        }

        [HttpPost]
        public async Task<IActionResult> CreateNewNote(IFormCollection form)
        {
            form = await Request.ReadFormAsync();

            var title = form["title"];
            var description = form["description"];
            var imageFile = form.Files["image"];

            if (!string.IsNullOrEmpty(title) && !string.IsNullOrEmpty(description) && imageFile != null)
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var currentUser = await context.Users.FirstOrDefaultAsync(n => n.Id == userId);

                string webRootPath = webHostEnvironment.WebRootPath;
                string imagePath = Path.Combine(webRootPath, "Images", imageFile.FileName);

                using (var fileStream = new FileStream(imagePath, FileMode.Create))
                {
                    await imageFile.CopyToAsync(fileStream);
                }

                var imageUrl = "/Images/" + imageFile.FileName;

                var newNote = new Note
                {
                    Id = Guid.NewGuid(),
                    Title = title,
                    Description = description,
                    ImageUrl = imageUrl,
                    DateCreated = DateTime.Now,
                    AppUser = currentUser,
                    IsDeleted = false,
                    IsPublic = false
                };

                context.Notes.Add(newNote);
                await context.SaveChangesAsync();

                var responeNote = new NoteDto
                {
                    Id = newNote.Id,
                    Title = newNote.Title,
                    Description = newNote.Description,
                    ImageUrl = newNote.ImageUrl,
                    DateCreated = newNote.DateCreated,
                    AppUser = newNote.AppUser!.Name,
                };

                return CreatedAtAction(nameof(GetNoteById), new { id = newNote.Id }, responeNote);
            }

            return BadRequest("Invalid data");
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetNoteById(Guid id)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var currentUser = await context.Users.FirstOrDefaultAsync(n => n.Id == userId);

            var note = context.Notes
                .Include(n => n.AppUser)
                .FirstOrDefault(n => n.Id == id && n.AppUserId == userId && !n.IsDeleted);


            /*var note = context.Notes
                .Include(n => n.AppUser)
                .FirstOrDefault(n => n.Id == id && n.IsDeleted);*/

            if (note == null)
            {
                return NotFound();
            }

            var responeNote = new NoteDto
            {
                Id = note.Id,
                Title = note.Title,
                Description = note.Description,
                ImageUrl = note.ImageUrl,
                DateCreated = note.DateCreated,
                AppUser = note.AppUser!.Name,
            };

            return Ok(responeNote);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateNote(Guid id, [FromForm] NoteVM model)
        {
            if (ModelState.IsValid)
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var currentUser = await context.Users.FirstOrDefaultAsync(n => n.Id == userId);

                var existingNote = context.Notes.FirstOrDefault(n => n.Id == id && n.AppUserId == userId && !n.IsDeleted);

                if (existingNote != null)
                {
                    if (model.Image != null)
                    {
                        string webRootPath = webHostEnvironment.WebRootPath;
                        string imagePath = Path.Combine(webRootPath, "Images", model.Image.FileName);


                        using (var fileStream = new FileStream(imagePath, FileMode.Create))
                        {
                            model.Image.CopyTo(fileStream);
                        }

                        existingNote.ImageUrl = "/Images/" + model.Image.FileName;
                    }

                    existingNote.Title = model.Title;
                    existingNote.Description = model.Description;
                    existingNote.DateCreated = DateTime.Now;

                    await context.SaveChangesAsync();

                    return NoContent(); // 204 No Content
                }
                else
                {
                    return NotFound(); // 404 Not Found
                }
            }

            return BadRequest(ModelState); // 400 Bad Request
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> SoftDeleteNote(Guid id)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var currentUser = await context.Users.FirstOrDefaultAsync(n => n.Id == userId);

            var note = context.Notes.FirstOrDefault(n => n.Id == id && n.AppUserId == userId && !n.IsDeleted);

            //var note = context.Notes.FirstOrDefault(n => n.Id == id);

            if (note == null)
            {
                return NotFound();
            }

            note.IsDeleted = true;
            context.SaveChanges();

            return Ok(new { Message = "Note soft-deleted successfully." });
        }

    }
}
