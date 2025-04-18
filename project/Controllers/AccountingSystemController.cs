using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Mvc;
using project.Models;
using project.Models.Services;

namespace project.Controllers
{
    public class AccountingSystemController : Controller
    {
        private readonly IConfiguration _configuration;

        public AccountingSystemController(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public ActionResult AccountBookList()
        {
            AccountBookService service = new AccountBookService(_configuration);
            var searchArg = new AccountBookList { UserId = 1 };
            List<AccountBookList> listResult = service.SearchBookData(searchArg);

            return View(listResult);
        }
    }
}
