using System.ComponentModel.DataAnnotations;

namespace NoteWebApp.ViewModels
{
    public class NoteVM
    {
        public Guid Id { get; set; }

        [Required(ErrorMessage = "Please enter the title of your note")]
        [MaxLength(100)]
        public string? Title { get; set; }

        [Required(ErrorMessage = "Please enter the title of your note")]
        [MaxLength(300)]
        public string? Description { get; set; }

        [Display(Name = "Choose the cover photo of your note")]
        public IFormFile? Image { get; set; }

        public DateTime DateCreated { get; set; }

        public bool IsDeleted { get; set; }

        public string? AppUseId { get; set; }
    }
}
