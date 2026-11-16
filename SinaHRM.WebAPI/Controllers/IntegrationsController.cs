using Microsoft.AspNetCore.Mvc;

namespace SinaHRM.API.Controllers;

public class IntegrationsController : Controller
{
    public IActionResult Index()
    {
        return View();
    }
}
