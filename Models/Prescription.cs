namespace DocuLink.Models
{
    public class Prescription
    {
        public int Id { get; set; }
        public string Notes { get; set; }
        public string Diagnosis { get; set; }

        public int ConsultationId { get; set; }
        public Consultation Consultation { get; set; }

        public ICollection<PrescribedMedication> PrescribedMedications { get; set; }
    }
}
