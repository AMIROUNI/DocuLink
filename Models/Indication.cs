namespace DocuLink.Models
{
    public class Indication
    {
        public int Id { get; set; }
        public string ConditionName { get; set; }
        public string Description { get; set; }

        public int MedicationId { get; set; }
        public Medication Medication { get; set; }
    }
}
