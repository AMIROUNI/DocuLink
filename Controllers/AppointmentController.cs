using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DocuLink.Models;
using DocuLink.Data;

namespace DocuLink.Controllers
{
    [Authorize(Roles = "Admin, Doctor")]
    public class AppointmentController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ILogger<AppointmentController> _logger;
        private static string _patientRoleId;
        private static string _doctorRoleId;

        public AppointmentController(
            AppDbContext context,
            UserManager<User> userManager,
            RoleManager<IdentityRole> roleManager,
            ILogger<AppointmentController> logger)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
            _logger = logger;
        }

        private async Task InitializeRoleIds()
        {
            if (string.IsNullOrEmpty(_patientRoleId))
            {
                _logger.LogDebug("Initializing Patient role ID");
                var patientRole = await _roleManager.FindByNameAsync("Patient");
                if (patientRole == null)
                {
                    _logger.LogError("Patient role not found in database");
                    throw new Exception("Patient role not configured in system");
                }
                _patientRoleId = patientRole.Id;
                _logger.LogDebug($"Patient role ID: {_patientRoleId}");
            }

            if (string.IsNullOrEmpty(_doctorRoleId))
            {
                _logger.LogDebug("Initializing Doctor role ID");
                var doctorRole = await _roleManager.FindByNameAsync("Doctor");
                if (doctorRole == null)
                {
                    _logger.LogError("Doctor role not found in database");
                    throw new Exception("Doctor role not configured in system");
                }
                _doctorRoleId = doctorRole.Id;
                _logger.LogDebug($"Doctor role ID: {_doctorRoleId}");
            }
        }

        // GET: Appointment/Index
        public async Task<IActionResult> Index()
        {
            _logger.LogInformation("Loading appointments list");
            try
            {
                var appointments = await _context.Appointments
                    .Include(a => a.User)
                    .Include(a => a.Doctor)
                    .AsNoTracking()
                    .ToListAsync();

                _logger.LogDebug($"Found {appointments.Count} appointments");
                return View(appointments);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading appointments");
                TempData["ErrorMessage"] = "Error loading appointments";
                return View(Array.Empty<Appointment>());
            }
        }

        // GET: Appointment/Create
        public async Task<IActionResult> Create()
        {
            _logger.LogInformation("Loading appointment creation form");
            try
            {
                await InitializeRoleIds();
                await PopulateDropdownsAsync();
                return View(new Appointment());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading create form");
                TempData["ErrorMessage"] = "System configuration error. Please contact admin.";
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: Appointment/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("UserId,DoctorId,Date,Reason,Status")] Appointment appointment)
        {
            _logger.LogInformation("Attempting to create new appointment");

            try
            {
                await InitializeRoleIds();

                if (!ModelState.IsValid)
                {
                    _logger.LogWarning("Model validation failed");
                    foreach (var error in ModelState)
                    {
                        foreach (var err in error.Value.Errors)
                        {
                            _logger.LogWarning($"Validation error in {error.Key}: {err.ErrorMessage}");
                        }
                    }
                    await PopulateDropdownsAsync();
                    return View(appointment);
                }

                // Verify patient role
                var isPatient = await _context.UserRoles
                    .AnyAsync(ur => ur.UserId == appointment.UserId && ur.RoleId == _patientRoleId);

                if (!isPatient)
                {
                    _logger.LogWarning($"User {appointment.UserId} is not a patient");
                    ModelState.AddModelError("UserId", "Selected user is not a patient");
                    await PopulateDropdownsAsync();
                    return View(appointment);
                }

                // Verify doctor role
                var isDoctor = await _context.UserRoles
                    .AnyAsync(ur => ur.UserId == appointment.DoctorId && ur.RoleId == _doctorRoleId);

                if (!isDoctor)
                {
                    _logger.LogWarning($"User {appointment.DoctorId} is not a doctor");
                    ModelState.AddModelError("DoctorId", "Selected user is not a doctor");
                    await PopulateDropdownsAsync();
                    return View(appointment);
                }

                // Set default status if not provided
                appointment.Status ??= "Scheduled";

                // Create appointment
                _context.Appointments.Add(appointment);
                await _context.SaveChangesAsync();

                _logger.LogInformation($"Appointment created successfully. ID: {appointment.Id}");
                TempData["SuccessMessage"] = "Appointment created successfully!";
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateException dbEx)
            {
                _logger.LogError(dbEx, "Database error creating appointment");
                TempData["ErrorMessage"] = "Database error. Please try again.";
                await PopulateDropdownsAsync();
                return View(appointment);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error creating appointment");
                TempData["ErrorMessage"] = "Unexpected error creating appointment";
                await PopulateDropdownsAsync();
                return View(appointment);
            }
        }

        private async Task PopulateDropdownsAsync()
        {
            _logger.LogDebug("Populating dropdown lists");

            try
            {
                // Get patients in a single query
                var patients = await _context.UserRoles
                    .Where(ur => ur.RoleId == _patientRoleId)
                    .Join(_context.Users,
                        ur => ur.UserId,
                        u => u.Id,
                        (ur, u) => new SelectListItem
                        {
                            Value = u.Id,
                            Text = u.Name
                        })
                    .AsNoTracking()
                    .ToListAsync();

                ViewBag.Users = patients;
                _logger.LogDebug($"Found {patients.Count} patients");

                // Get doctors in a single query
                var doctors = await _context.UserRoles
                    .Where(ur => ur.RoleId == _doctorRoleId)
                    .Join(_context.Users,
                        ur => ur.UserId,
                        u => u.Id,
                        (ur, u) => new SelectListItem
                        {
                            Value = u.Id,
                            Text = u.Name
                        })
                    .AsNoTracking()
                    .ToListAsync();

                ViewBag.Doctors = doctors;
                _logger.LogDebug($"Found {doctors.Count} doctors");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error populating dropdowns");
                throw;
            }
        }
    }
}