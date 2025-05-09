namespace DocuLink.Models
{
    public class Consultation
    {
        public int Id { get; set; } // Clé primaire ajoutée
        public string Notes { get; set; }
        public DateTime Date { get; set; }

        // Relation avec Appointment
        public int AppointmentId { get; set; } // Clé étrangère
        public Appointment Appointment { get; set; }
    }
}