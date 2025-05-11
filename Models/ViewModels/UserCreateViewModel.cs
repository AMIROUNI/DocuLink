using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;


    namespace DocuLink.Models.ViewModels
    {
        public class UserCreateViewModel
        {
            [Required(ErrorMessage = "Email is required")]
            [EmailAddress(ErrorMessage = "Invalid email address")]
            public string? Email { get; set; }

            [Required(ErrorMessage = "Name is required")]
            public string? Name { get; set; }

            [Required(ErrorMessage = "Password is required")]
            [DataType(DataType.Password)]
            [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$",
                ErrorMessage = "Password must be at least 8 characters long, with at least one uppercase letter, one lowercase letter, one number, and one special character.")]
            public string? Password { get; set; }

            [Phone(ErrorMessage = "Invalid phone number")]
            public int? Phone { get; set; }

            public bool IsDoctor { get; set; }

            [RequiredIfDoctor(ErrorMessage = "Specialty is required for doctors")]
            public string? Specialty { get; set; }

            [RequiredIfDoctor(ErrorMessage = "Location is required for doctors")]
            public string? Location { get; set; }

            public IFormFile? DiplomaFile { get; set; }
        }

     
    
}   