using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using DocuLink.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.AspNetCore.Http;
using System.Text.Encodings.Web;
using System.Text;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Logging;
using System;
using DocuLink.Models.ViewModels;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace DocuLink.Controllers
{
    [Authorize(Roles = "Admin")]
    public class UserController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IEmailSender _emailSender;
        private readonly ILogger<UserController> _logger;

        public UserController(
            UserManager<User> userManager,
            RoleManager<IdentityRole> roleManager,
            IWebHostEnvironment webHostEnvironment,
            IEmailSender emailSender,
            ILogger<UserController> logger)
        {
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
            _roleManager = roleManager ?? throw new ArgumentNullException(nameof(roleManager));
            _webHostEnvironment = webHostEnvironment ?? throw new ArgumentNullException(nameof(webHostEnvironment));
            _emailSender = emailSender ?? throw new ArgumentNullException(nameof(emailSender));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        // GET: User/Index
        public async Task<IActionResult> Index()
        {
            try
            {
                _logger.LogInformation("Loading user list");
                var users = await _userManager.Users.ToListAsync();
                var userRoles = new Dictionary<string, IList<string>>();

                foreach (var user in users)
                {
                    userRoles[user.Id] = await _userManager.GetRolesAsync(user);
                }

                ViewBag.UserRoles = userRoles;
                return View(users);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading user list");
                TempData["ErrorMessage"] = "An error occurred while loading the user list.";
                return View(new List<User>());
            }
        }

        // GET: User/Create
        public async Task<IActionResult> Create()
        {
            try
            {
                ViewBag.Roles = new SelectList(await GetRolesAsync(), "Name", "Name");
                return View(new UserCreateViewModel());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading create user page");
                TempData["ErrorMessage"] = "An error occurred while loading the create user page.";
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: User/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(UserCreateViewModel model)
        {
            try
            {
                ViewBag.Roles = new SelectList(await GetRolesAsync(), "Name", "Name");

                if (!ModelState.IsValid)
                {
                    var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
                    _logger.LogWarning("Model state is invalid for create user. Errors: {Errors}", string.Join("; ", errors));
                    return View(model);
                }

                string? diplomaPath = null;
                if (model.IsDoctor && model.DiplomaFile != null && model.DiplomaFile.Length > 0)
                {
                    if (model.DiplomaFile.Length > 5 * 1024 * 1024) // 5MB limit
                    {
                        ModelState.AddModelError("DiplomaFile", "File size cannot exceed 5MB");
                        return View(model);
                    }
                    if (Path.GetExtension(model.DiplomaFile.FileName).ToLower() != ".pdf")
                    {
                        ModelState.AddModelError("DiplomaFile", "Only PDF files are allowed");
                        return View(model);
                    }

                    var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "diplomas");
                    if (!Directory.Exists(uploadsFolder))
                    {
                        Directory.CreateDirectory(uploadsFolder);
                    }

                    var uniqueFileName = Guid.NewGuid().ToString() + "_" + model.DiplomaFile.FileName;
                    var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await model.DiplomaFile.CopyToAsync(fileStream);
                    }

                    diplomaPath = "/diplomas/" + uniqueFileName;
                }
                else if (model.IsDoctor && model.DiplomaFile == null)
                {
                    ModelState.AddModelError("DiplomaFile", "Diploma file is required for doctors");
                    return View(model);
                }

                var user = new User
                {
                    UserName = model.Email,
                    Email = model.Email,
                    Name = model.Name,
                    PhoneNumber = model.Phone?.ToString(),
                    Phone = (int)(model.Phone ?? 0),
                    IsDoctor = model.IsDoctor,
                    Specialty = model.IsDoctor ? model.Specialty : null,
                    Location = model.IsDoctor ? model.Location : null,
                    DiplomaFilePath = diplomaPath,
                    EmailConfirmed = true
                };

                var result = await _userManager.CreateAsync(user, model.Password!);

                if (result.Succeeded)
                {
                    _logger.LogInformation("User {Email} created successfully", model.Email);
                    var role = model.IsDoctor ? "Doctor" : "Patient";
                    await _userManager.AddToRoleAsync(user, role);

                    TempData["SuccessMessage"] = "User created successfully";
                    return RedirectToAction(nameof(Index));
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                    if (error.Code.Contains("Password"))
                    {
                        ViewBag.PasswordError = error.Description;
                    }
                }
                _logger.LogWarning("Failed to create user {Email}. Errors: {Errors}", model.Email, string.Join("; ", result.Errors.Select(e => e.Description)));
                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating user {Email}", model.Email);
                ModelState.AddModelError("", "An error occurred while creating the user");
                return View(model);
            }
        }

        // GET: User/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            try
            {
                if (string.IsNullOrEmpty(id)) return NotFound();

                var user = await _userManager.FindByIdAsync(id);
                if (user == null) return NotFound();

                var userRoles = await _userManager.GetRolesAsync(user);
                ViewBag.Roles = new SelectList(await GetRolesAsync(), "Name", "Name", userRoles.FirstOrDefault());

                var model = new UserEditViewModel
                {
                    Id = user.Id,
                    Email = user.Email,
                    Name = user.Name,
                    Phone = user.Phone,
                    IsDoctor = user.IsDoctor,
                    Specialty = user.Specialty,
                    Location = user.Location,
                    DiplomaFilePath = user.DiplomaFilePath
                };

                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading edit user page for ID {Id}", id);
                TempData["ErrorMessage"] = "An error occurred while loading the edit user page.";
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: User/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, UserEditViewModel model)
        {
            try
            {
                if (id != model.Id) return NotFound();

                ViewBag.Roles = new SelectList(await GetRolesAsync(), "Name", "Name");

                if (!ModelState.IsValid)
                {
                    var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
                    _logger.LogWarning("Model state is invalid for edit user {Id}. Errors: {Errors}", id, string.Join("; ", errors));
                    return View(model);
                }

                var user = await _userManager.FindByIdAsync(id);
                if (user == null) return NotFound();

                // Handle password change
                if (!string.IsNullOrEmpty(model.NewPassword))
                {
                    var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                    var result = await _userManager.ResetPasswordAsync(user, token, model.NewPassword);
                    if (!result.Succeeded)
                    {
                        foreach (var error in result.Errors)
                        {
                            ModelState.AddModelError(string.Empty, error.Description);
                        }
                        _logger.LogWarning("Failed to reset password for user {Id}. Errors: {Errors}", id, string.Join("; ", result.Errors.Select(e => e.Description)));
                        return View(model);
                    }
                }

                // Handle file upload
                if (model.IsDoctor && model.DiplomaFile != null && model.DiplomaFile.Length > 0)
                {
                    if (model.DiplomaFile.Length > 5 * 1024 * 1024)
                    {
                        ModelState.AddModelError("DiplomaFile", "File size cannot exceed 5MB");
                        return View(model);
                    }
                    if (Path.GetExtension(model.DiplomaFile.FileName).ToLower() != ".pdf")
                    {
                        ModelState.AddModelError("DiplomaFile", "Only PDF files are allowed");
                        return View(model);
                    }

                    var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "diplomas");
                    if (!Directory.Exists(uploadsFolder))
                    {
                        Directory.CreateDirectory(uploadsFolder);
                    }

                    // Delete old file if exists
                    if (!string.IsNullOrEmpty(user.DiplomaFilePath))
                    {
                        var oldFilePath = Path.Combine(_webHostEnvironment.WebRootPath, user.DiplomaFilePath.TrimStart('/'));
                        if (System.IO.File.Exists(oldFilePath))
                        {
                            System.IO.File.Delete(oldFilePath);
                        }
                    }

                    var uniqueFileName = Guid.NewGuid().ToString() + "_" + model.DiplomaFile.FileName;
                    var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await model.DiplomaFile.CopyToAsync(fileStream);
                    }

                    user.DiplomaFilePath = "/diplomas/" + uniqueFileName;
                }
                else if (model.IsDoctor && string.IsNullOrEmpty(user.DiplomaFilePath) && model.DiplomaFile == null)
                {
                    ModelState.AddModelError("DiplomaFile", "Diploma file is required for doctors");
                    return View(model);
                }
                else if (!model.IsDoctor && !string.IsNullOrEmpty(user.DiplomaFilePath))
                {
                    var filePath = Path.Combine(_webHostEnvironment.WebRootPath, user.DiplomaFilePath.TrimStart('/'));
                    if (System.IO.File.Exists(filePath))
                    {
                        System.IO.File.Delete(filePath);
                    }
                    user.DiplomaFilePath = null;
                }

                // Update properties
                user.Email = model.Email;
                user.UserName = model.Email;
                user.Name = model.Name;
                user.Phone = (int)(model.Phone ?? 0);
                user.IsDoctor = model.IsDoctor;
                user.Specialty = model.IsDoctor ? model.Specialty : null;
                user.Location = model.IsDoctor ? model.Location : null;

                var updateResult = await _userManager.UpdateAsync(user);
                if (updateResult.Succeeded)
                {
                    var currentRoles = await _userManager.GetRolesAsync(user);
                    await _userManager.RemoveFromRolesAsync(user, currentRoles);

                    var role = model.IsDoctor ? "Doctor" : "Patient";
                    await _userManager.AddToRoleAsync(user, role);

                    TempData["SuccessMessage"] = "User updated successfully";
                    return RedirectToAction(nameof(Index));
                }

                foreach (var error in updateResult.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
                _logger.LogWarning("Failed to update user {Id}. Errors: {Errors}", id, string.Join("; ", updateResult.Errors.Select(e => e.Description)));
                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating user {Id}", id);
                ModelState.AddModelError("", "An error occurred while updating the user");
                return View(model);
            }
        }

        // GET: User/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            try
            {
                if (string.IsNullOrEmpty(id)) return NotFound();

                var user = await _userManager.FindByIdAsync(id);
                if (user == null) return NotFound();

                ViewBag.UserRoles = await _userManager.GetRolesAsync(user);
                return View(user);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading delete user page for ID {Id}", id);
                TempData["ErrorMessage"] = "An error occurred while loading the delete user page.";
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: User/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(id);
                if (user == null) return NotFound();

                if (!string.IsNullOrEmpty(user.DiplomaFilePath))
                {
                    var filePath = Path.Combine(_webHostEnvironment.WebRootPath, user.DiplomaFilePath.TrimStart('/'));
                    if (System.IO.File.Exists(filePath))
                    {
                        System.IO.File.Delete(filePath);
                    }
                }

                var result = await _userManager.DeleteAsync(user);
                if (result.Succeeded)
                {
                    TempData["SuccessMessage"] = "User deleted successfully";
                    return RedirectToAction(nameof(Index));
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
                _logger.LogWarning("Failed to delete user {Id}. Errors: {Errors}", id, string.Join("; ", result.Errors.Select(e => e.Description)));
                return View("Delete", user);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting user {Id}", id);
                TempData["ErrorMessage"] = "An error occurred while deleting the user.";
                return RedirectToAction(nameof(Index));
            }
        }

        private async Task<List<IdentityRole>> GetRolesAsync()
        {
            try
            {
                return await _roleManager.Roles.ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching roles");
                return new List<IdentityRole>();
            }
        }
    }
}