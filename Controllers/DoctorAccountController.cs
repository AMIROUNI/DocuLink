using DocuLink.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DocuLink.Controllers
{
    public class DoctorAccountController : Controller
    {
        private readonly AppDbContext _context;

        public DoctorAccountController(AppDbContext context)
        {
            _context = context;
        }

        // GET: DoctorAccount/Register
        public IActionResult Register()
        {
            return View();
        }

        // POST: DoctorAccount/Register
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(Doctor doctor)
        {
            if (ModelState.IsValid)
            {
                // Vérifie que l’email n’est pas déjà utilisé
                var existing = await _context.Doctors.FirstOrDefaultAsync(d => d.Email == doctor.Email);
                if (existing != null)
                {
                    ModelState.AddModelError("Email", "Cet email est déjà utilisé.");
                    return View(doctor);
                }

                _context.Doctors.Add(doctor);
                await _context.SaveChangesAsync();

                // Stocker l'ID du docteur dans la session
                HttpContext.Session.SetInt32("DoctorId", doctor.Id);
                return RedirectToAction("Profile");
            }

            return View(doctor);
        }

        // GET: DoctorAccount/Login
        public IActionResult Login()
        {
            return View();
        }

        // POST: DoctorAccount/Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(string email, string password)
        {
            var doctor = await _context.Doctors
                .FirstOrDefaultAsync(d => d.Email == email && d.Password == password);

            if (doctor != null)
            {
                HttpContext.Session.SetInt32("DoctorId", doctor.Id);
                return RedirectToAction("Profile");
            }

            ViewBag.Error = "Email ou mot de passe incorrect.";
            return View();
        }

        // GET: DoctorAccount/Profile
        public async Task<IActionResult> Profile()
        {
            var doctorId = HttpContext.Session.GetInt32("DoctorId");
            if (doctorId == null)
            {
                return RedirectToAction("Login");
            }

            var doctor = await _context.Doctors.FindAsync(doctorId);
            if (doctor == null)
            {
                return NotFound();
            }

            return View(doctor);
        }

        // GET: DoctorAccount/Logout
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }
    }
}
