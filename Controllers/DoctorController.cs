using Microsoft.AspNetCore.Mvc;

namespace DocuLink.Controllers
{
    public class DoctorController : Controller
    {
        public IActionResult Dashboard()
        {
            return View();
        }
    }
}
