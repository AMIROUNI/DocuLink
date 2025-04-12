namespace DocuLink.Models
{
    public class Appointment
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public string Status { get; set; }
        public string Reason { get; set; }

        public int UserId { get; set; }
        public User User { get; set; }

        public int DoctorId { get; set; }
        public Doctor Doctor { get; set; }

        public Consultation Consultation { get; set; }
        public Payment Payment { get; set; }
    }
}
