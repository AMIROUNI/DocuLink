using System.ComponentModel.DataAnnotations;

namespace DocuLink.Models.ViewModels
{
    public class UserEditViewModel
    {
        [Required(ErrorMessage = "Id is required")]
        public string? Id { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email address")]
        public string? Email { get; set; }

        [Required(ErrorMessage = "Name is required")]
        public string? Name { get; set; }

        [Phone(ErrorMessage = "Invalid phone number")]
        public long? Phone { get; set; }

        public bool IsDoctor { get; set; }

        [RequiredIfDoctor(ErrorMessage = "Specialty is required for doctors")]
        public string? Specialty { get; set; }

        [RequiredIfDoctor(ErrorMessage = "Location is required for doctors")]
        public string? Location { get; set; }

        public string? DiplomaFilePath { get; set; }

        [DataType(DataType.Password)]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$",
            ErrorMessage = "New password must be at least 8 characters long, with at least one uppercase letter, one lowercase letter, one number, and one special character.")]
        public string? NewPassword { get; set; }

        public IFormFile? DiplomaFile { get; set; }
    }
}

