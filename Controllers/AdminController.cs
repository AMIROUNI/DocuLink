using Microsoft.AspNetCore.Mvc;

namespace DocuLink.Controllers
{
    public class AdminController : Controller
    {
        public IActionResult Dashboard()
        {
            return View();
        }
    }
}
