using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;


namespace DocuLink.Models.ViewModels
{
    public class RegisterViewModel
    {
        [Required]
        public string Email { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        public bool IsDoctor { get; set; }

        public string? Specialty { get; set; }
        public string? Location { get; set; }

        public int Phone { get; set; }

        public IFormFile? Diploma { get; set; }
    }
}
