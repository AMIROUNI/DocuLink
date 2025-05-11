using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace DocuLink.Models
{
    public class User : IdentityUser
    {
        [Required]
        [Display(Name = "Full Name")]
        public string? Name { get; set; }

        // Doctor-specific fields (optional)
        [Display(Name = "Specialty")]
        public string? Specialty { get; set; }

        [Display(Name = "Location")]
        public string? Location { get; set; }

        [Display(Name = "Phone Number")]
        public int Phone { get; set; }

        [Display(Name = "Register as Doctor")]
        public bool IsDoctor { get; set; }

        [Display(Name = "Diploma File")]
        public string? DiplomaFilePath { get; set; }

        [NotMapped]
        [Display(Name = "Diploma File")]
        public IFormFile? DiplomaFile { get; set; }
        public ICollection<Appointment> Appointments { get; set; }
        public ICollection<Review> Reviews { get; set; }
    }
}
