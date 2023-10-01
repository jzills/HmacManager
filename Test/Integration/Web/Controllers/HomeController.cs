using System.Diagnostics;
using HmacManagement.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Web.Models;

namespace Web.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;

    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }

    public IActionResult Index()
    {
        return View();
    }

    [Authorize(AuthenticationSchemes = HmacAuthenticationDefaults.AuthenticationScheme)]
    public IActionResult Protected()
    {
        return Ok("You've accessed a protected endpoint.");
    }
}
