namespace DocuLink.Models
{
    public class Review
    {
        public int Id { get; set; }
        public int Rating { get; set; }
        public string Comment { get; set; }
        public DateTime Date { get; set; }

        public int UserId { get; set; }
        public User User { get; set; }

        public int MedicationId { get; set; }
        public Medication Medication { get; set; }

    }
}
