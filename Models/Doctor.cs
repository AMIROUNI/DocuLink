namespace DocuLink.Models
{
    public class Doctor
    {

        public int Id { get; set; }
        public string Name { get; set; }
        public string Specialty { get; set; }
        public string Phone { get; set; }
        public string Location { get; set; }

        public ICollection<Appointment> Appointments { get; set; }
    }
}
