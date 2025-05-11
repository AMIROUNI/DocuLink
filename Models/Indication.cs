using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DocuLink.Models
{
    public class Indication
    {
        public int Id { get; set; }

        [Required]
        public string ConditionName { get; set; }

        public string Description { get; set; }

        [Display(Name = "Medication")]
        public int MedicationId { get; set; }

        [ForeignKey("MedicationId")]
        public Medication Medication { get; set; }
    }
}
