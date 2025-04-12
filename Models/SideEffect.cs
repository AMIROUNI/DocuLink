namespace DocuLink.Models
{
    public class SideEffect
    {
        public int Id { get; set; }
        public string Description { get; set; }
        public string SeverityLevel { get; set; }

        public int MedicationId { get; set; }
        public Medication Medication { get; set; }
    }
}
