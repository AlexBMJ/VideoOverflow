using Microsoft.AspNetCore.Mvc;

namespace Videooverflow.Server.Controllers;

public class  : Controller
{
    // GET
    public IActionResult Index()
    {
        return View();
    }
}