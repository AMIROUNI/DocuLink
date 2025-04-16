using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using DocuLink.Models;
using DocuLink.Models.ViewModels;

namespace DocuLink.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;

    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }

    public IActionResult Index()
    {
        return View(new LandingPageViewModel());
    }

    [HttpPost]
    public IActionResult Contact(LandingPageViewModel model)
    {
        if (ModelState.IsValid)
        {
            // Handle form submission (e.g., send email, save to DB)
            TempData["SuccessMessage"] = "Thank you for your message! We'll get back to you soon.";
            return RedirectToAction("Index");
        }
        return View("Index", model);
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
