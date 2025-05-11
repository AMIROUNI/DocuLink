using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DocuLink.Models
{
    public class Appointment
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Please select a user")]
        
        [Display(Name = "User")]
        public string UserId { get; set; }

        [Required(ErrorMessage = "Please select a doctor")]
        [Display(Name = "Doctor")]


        public string DoctorId { get; set; }

        [Required(ErrorMessage = "Please select a date and time")]
        [DataType(DataType.DateTime)]
        public DateTime Date { get; set; }

        [Required(ErrorMessage = "Please enter a reason")]
        [StringLength(500, MinimumLength = 5, ErrorMessage = "Reason must be between 10-500 characters")]
        public string Reason { get; set; }


        public string? Status { get; set; }

        // Navigation properties

        [ForeignKey("UserId")]
        public User User { get; set; }


        [ForeignKey("DoctorId")]
        public User Doctor { get; set; }

        // Relation 1-to-1 with Consultation
        public Consultation? Consultation { get; set; }

        // Relation 1-to-1 with Payment
        public Payment? Payment { get; set; }
    }
}
