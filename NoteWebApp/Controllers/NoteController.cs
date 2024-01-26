using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NoteWebApp.Data;
using NoteWebApp.Models;
using NoteWebApp.ViewModels;

namespace NoteWebApp.Controllers
{
    [Authorize]
    public class NoteController(AppDbContext context,
        IWebHostEnvironment webHostEnvironment)
        : Controller
    {
        private readonly AppDbContext context = context;
        private readonly IWebHostEnvironment webHostEnvironment = webHostEnvironment;

        public IActionResult GetAllNotes()
        {
            var user = context.Users
                .Include(u => u!.Notes!.Where(n => !n!.IsDeleted))
                .FirstOrDefault(u => u.UserName == User!.Identity!.Name);

            return View(user?.Notes?.OrderByDescending(u => u!.DateCreated));
        }

        public IActionResult CreateNewNote()
        {
            return View();
        }

        [HttpPost]
        public IActionResult CreateNewNote(NoteVM model)
        {
            if (ModelState.IsValid)
            {
                var currentUser = context.Users
                    .Include(u => u.Notes)
                    .FirstOrDefault(u => u.UserName == User!.Identity!.Name);
                
                string webRootPath = webHostEnvironment.WebRootPath;
                string imagePath;
                string _ImageUrl;

                if (model.Image != null)
                {
                    imagePath = Path.Combine(webRootPath, "Images", model.Image.FileName);

                    using (var fileStream = new FileStream(imagePath, FileMode.Create))
                    {
                        model.Image.CopyTo(fileStream);
                    }

                    _ImageUrl = "/Images/" + model.Image.FileName;
                }
                else
                {
                    _ImageUrl = "/Images/" + "Purple.jpg";
                }


                var newNote = new Note
                {
                    Id = Guid.NewGuid(),
                    Title = model.Title,
                    Description = model.Description,
                    ImageUrl = _ImageUrl,
                    DateCreated = DateTime.Now,
                    AppUser = currentUser,
                    IsDeleted = false,
                    IsPublic = false
                };

                context.Notes.Add(newNote);
                context.SaveChanges();

                return RedirectToAction("GetAllNotes", "Note");
            }

            return View(model);
        }

        [HttpGet]
        public IActionResult UpdateNote(Guid id)
        {
            var note = context.Notes.FirstOrDefault(n => n.Id == id);

            if (note == null)
            {
                return NotFound();
            }

            var noteViewModel = new NoteVM
            {
                Id = note.Id,
                Title = note.Title,
                Description = note.Description,
            };

            return View(noteViewModel);
        }

        [HttpPost]
        public IActionResult UpdateNote(NoteVM model)
        {
            if (ModelState.IsValid)
            {
                var currentUser = context.Users
                    .Include(u => u.Notes)
                    .FirstOrDefault(u => u.UserName == User!.Identity!.Name);

                var existingNote = currentUser!.Notes!.FirstOrDefault(n => n!.Id == model.Id);

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

                    context.SaveChanges();

                    return RedirectToAction("GetAllNotes", "Note");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Note not found.");
                }
            }

            return View(model);
        }

        [HttpGet]
        public IActionResult SoftDeleteNote(Guid id)
        {
            var note = context.Notes.FirstOrDefault(n => n.Id == id);

            if (note == null)
            {
                return NotFound();
            }

            note.IsDeleted = true;

            context.SaveChanges();

            return RedirectToAction("GetAllNotes");
        }

    }
}
