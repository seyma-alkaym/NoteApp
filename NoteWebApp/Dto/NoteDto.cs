using NoteWebApp.Models;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace NoteWebApp.Dto
{
    public class NoteDto
    {
        public Guid Id { get; set; }
        public string? Title { get; set; }

        public string? Description { get; set; }

        public string? ImageUrl { get; set; }

        public DateTime DateCreated { get; set; }

        public string? AppUser { get; set; }

    }
}
