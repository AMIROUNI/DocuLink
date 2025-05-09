using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DocuLink.Models;

namespace DocuLink.Controllers
{
    public class AdminController : Controller
    {
        private readonly AppDbContext _context;

        public AdminController(AppDbContext context)
        {
            _context = context;
        }

        public IActionResult Dashboard()
        {
            return View();
        }
        
        public async Task<IActionResult> Index()
        {
            return View();
        }

        public async Task<IActionResult> ManageDoctors()
        {
            var doctors = await _context.Doctors.ToListAsync();
            return View(doctors);
        }

        public async Task<IActionResult> ManageUsers()
        {
            var users = await _context.Users.ToListAsync();
            return View(users);
        }
    }
}
