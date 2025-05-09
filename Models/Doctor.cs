using System.ComponentModel.DataAnnotations;

namespace DocuLink.Models
{
    public class Doctor
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string Specialty { get; set; }

        public string Phone { get; set; }

        [Required]
        public string Location { get; set; }

        // ➕ Ajout pour authentification
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        public ICollection<Appointment> Appointments { get; set; }
    }
}
