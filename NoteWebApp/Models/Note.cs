using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NoteWebApp.Models
{
    public class Note
    {
        [Key]
        public Guid Id { get; set; }

        [MaxLength(100)]
        public string? Title { get; set; }

        [MaxLength(300)]
        public string? Description { get; set; }

        public string? ImageUrl { get; set; }

        public DateTime DateCreated { get; set; }

        public bool IsDeleted { get; set; }

        [ForeignKey("AppUser")]
        public string? AppUserId { get; set; }

        public AppUser? AppUser { get; set; }

        public bool IsPublic {  get; set; }
    }
}
