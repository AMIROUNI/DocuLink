namespace DocuLink.Models
{
    public class Consultation
    {
        public int Id { get; set; }
        public string Notes { get; set; }
        public string Diagnosis { get; set; }

        public int AppointmentId { get; set; }
        public Appointment Appointment { get; set; }

        public Prescription Prescription { get; set; }
    }
}
