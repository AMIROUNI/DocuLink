namespace DocuLink.Models
{
    public class Consultation
    {
        public int Id { get; set; }
        public string Notes { get; set; }
        public DateTime Date { get; set; }

        public int AppointmentId { get; set; }
        public Appointment Appointment { get; set; }
    }
}
