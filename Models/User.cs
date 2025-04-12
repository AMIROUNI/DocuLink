using Microsoft.AspNetCore.Mvc.ViewEngines;

namespace DocuLink.Models
{
    public class User
    {


        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }

        public ICollection<Appointment> Appointments { get; set; }
        public ICollection<Review> Reviews { get; set; }
    }
}
