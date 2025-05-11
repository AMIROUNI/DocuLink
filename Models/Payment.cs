namespace DocuLink.Models
{
    public class Payment
    {
        public int Id { get; set; }
        public float Amount { get; set; }
        public string Method { get; set; }
        public DateTime Date { get; set; }

        public int AppointmentId { get; set; }
        public Appointment Appointment { get; set; }
    }
}
