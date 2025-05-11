namespace DocuLink.Models
{
    public class Medication
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Dosage { get; set; }
        public string Form { get; set; }
        public float Price { get; set; }

        public ICollection<PrescribedMedication>? PrescribedMedications { get; set; }
        public ICollection<SideEffect>? SideEffects { get; set; }
        public ICollection<Indication>? Indications { get; set; }
        public ICollection<Review>? Reviews { get; set; }
    }
}
