using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace NoteWebApp.Models
{
    public class AppUser : IdentityUser
    {
        [StringLength(100)]
        [MaxLength(100)]
        [Required]
        public string? Name { get; set; }

        public ICollection<Note?>? Notes { get; set; }
    }
}
