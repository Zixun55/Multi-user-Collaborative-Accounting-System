using Microsoft.AspNetCore.Mvc;
using project.Models;

namespace project.Controllers
{
    public class LoginSystemController : Controller
    {
        public IActionResult Login()
        {
            return View();
        }
    }
}
