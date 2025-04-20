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

        /// <summary>
        /// 獲得帳本列表
        /// </summary>
        /// <returns></returns>
        public ActionResult AccountBookList()
        {
            AccountBookService service = new AccountBookService(_configuration);
            var searchArg = new AccountBookList { UserId = 1 };
            List<AccountBookList> listResult = service.GetAccountBookList(searchArg);

            return View(listResult);
        }

        /// <summary>
        /// 獲得帳本資料
        /// </summary>
        /// <returns></returns>
        public ActionResult AccountBookData(int id)
        {
            AccountBookService service = new AccountBookService(_configuration);
            var searchArg = new TransactionList { AccountBookId = id };
            List<TransactionList> accountBookDataResult = service.GetAccountBookData(searchArg);

            if (accountBookDataResult == null || accountBookDataResult.Count == 0)
            {
                return NotFound();
            }

            return View(accountBookDataResult);
        }

        /// <summary>
        /// 獲得交易資料
        /// </summary>
        /// <returns></returns>
        public ActionResult TransactionUpdate(int id)
        {
            AccountBookService service = new AccountBookService(_configuration);
            var searchArg = new TransactionData { TransactionId = id };
            TransactionData accountBookDataResult = service.GetTransactionData(searchArg);

            return View(accountBookDataResult);
        }

        /// <summary>
        /// 儲存修改的交易紀錄
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult TransactionUpdateSave(TransactionData data)
        {
            AccountBookService service = new AccountBookService(_configuration);
            if (ModelState.IsValid)
            {
                service.UpdateTransactionData(data);
                return RedirectToAction("AccountBookData", "AccountingSystem", new { id = data.AccountBookId });
            }
            return View("TransactionUpdate", data);
        }
    }
}
