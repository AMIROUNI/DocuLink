using DocuLink.Data;
using Microsoft.AspNetCore.Mvc;

namespace DocuLink.Controllers
{
    public class AccountController : Controller
    {
        public readonly AppDbContext _context;

        public AccountController(AppDbContext context)
        {
            _context = context;
        }

        // Afficher la liste des utilisateurs
        public IActionResult UserManagement()
        {
            //var users = _userManager.Users.ToList();
            var users = _context.Users.ToList();
            return View(users);
        }
    }
}
