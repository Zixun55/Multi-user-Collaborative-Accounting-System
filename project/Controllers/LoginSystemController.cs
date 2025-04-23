using Microsoft.AspNetCore.Mvc;
using project.Models;
using project.Models.Services;

namespace project.Controllers
{
    public class LoginSystemController : Controller
    {
        private readonly IConfiguration _configuration;

        public LoginSystemController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        /// <summary>
        /// 顯示登入頁面
        /// </summary>
        /// <returns></returns>
        public IActionResult Login()
        {
            return View();
        }

        /// <summary>
        /// 登入驗證
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Login([Bind("Email,Password")] LoginData user)
        {
            if (ModelState.IsValid)
            {
                LoginSystemService service = new LoginSystemService(_configuration);
                int? userId = service.GetUserId(user.Email, user.Password);

                if (userId != null)
                {
                    HttpContext.Session.SetInt32("UserId", userId.Value); // 存進 Session
                    TempData["LoginSuccess"] = "登入成功";
                    return RedirectToAction("AccountBookList", "AccountingSystem");
                }
                else
                {
                    ModelState.AddModelError("", "Email或密碼錯誤");
                    return View(user);
                }
            }
            else
            {
                return View(user);
            }
            
        }

        /// <summary>
        /// 登出
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public IActionResult Logout()
        {
            // 檢查有無登入
            if (HttpContext.Session.GetInt32("UserId") == null)
            {
                return RedirectToAction("Login", "LoginSystem");
            }

            HttpContext.Session.Clear(); // 清空 session
            TempData["LogoutSuccess"] = "登出成功";
            return RedirectToAction("Login", "LoginSystem");
        }

        /// <summary>
        /// 顯示註冊頁面
        /// </summary>
        /// <returns></returns>
        public IActionResult Register()
        {
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Register(UserData data)
        {
            LoginSystemService service = new LoginSystemService(_configuration);
            if (ModelState.IsValid)
            {
                if (!service.CheckEmail(data.Email))
                {
                    service.CreateNewUser(data);
                    TempData["RegisterSuccess"] = "註冊成功";
                    return RedirectToAction("Login", "LoginSystem");
                }
                else
                {
                    ModelState.AddModelError("", "此Email已註冊過");
                    return View(data);
                }
            }
            return View(data);
        }
    }
}
