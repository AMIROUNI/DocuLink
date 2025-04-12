namespace DocuLink.Models
{
    public class PrescribedMedication
    {
        public int Id { get; set; }
        public string Dosage { get; set; }
        public string Duration { get; set; }

        public int PrescriptionId { get; set; }
        public Prescription Prescription { get; set; }

        public int MedicationId { get; set; }
        public Medication Medication { get; set; }
    }
}
