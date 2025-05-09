namespace DocuLink.Models
{
    public class Appointment
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public string Status { get; set; }
        public string Reason { get; set; }

        // Relations
        public int UserId { get; set; }
        public User User { get; set; }

        public int DoctorId { get; set; }
        public Doctor Doctor { get; set; }

        // Relation 1-to-1 avec Consultation
        public Consultation Consultation { get; set; }

        // Relation 1-to-1 avec Payment
        public Payment Payment { get; set; }
    }
}