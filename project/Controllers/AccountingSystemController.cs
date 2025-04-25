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
            int? userId = HttpContext.Session.GetInt32("UserId");

            if (userId == null)
            {
                return RedirectToAction("Login", "LoginSystem");
            }

            ViewBag.UserId = userId;

            AccountBookService service = new AccountBookService(_configuration);
            var searchArg = new AccountBookList { UserId = userId.Value };
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
            ViewBag.AccountBookId = id;

            return View(accountBookDataResult);
        }

        /// <summary>
        /// 獲得交易資料(交易編輯頁面)
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

        /// <summary>
        /// 新增交易頁面
        /// </summary>
        /// <param name="accountBookId"></param>
        /// <returns></returns>
        public ActionResult TransactionInsert(int accountBookId)
        {
            var model = new TransactionData();
            model.AccountBookId = accountBookId;
            model.Date = DateTime.Now;
            return View(model);
        }

        /// <summary>
        /// 儲存新增的交易紀錄
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult TransactionInsertSave(TransactionData data)
        {
            AccountBookService service = new AccountBookService(_configuration);
            if (ModelState.IsValid)
            {
                service.InsertTransactionData(data);
                return RedirectToAction("AccountBookData", "AccountingSystem", new { id = data.AccountBookId });
            }
            return View("TransactionInsert", data);
        }

        /// <summary>
        /// 刪除交易紀錄
        /// </summary>
        /// <param name="transactionId"></param>
        /// <param name="accountBookId"></param>
        /// <returns></returns>
        public IActionResult TransactionDelete(int transactionId, int accountBookId)
        {
            AccountBookService service = new AccountBookService(_configuration);
            service.DeleteTransactionData(transactionId, accountBookId);

            TempData["DeleteSuccess"] = "刪除成功";
            return RedirectToAction("AccountBookData", new { id = accountBookId });
        }

        /// <summary>
        /// 顯示新增帳本頁面
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public ActionResult AccountBookInsert(int userId)
        {
            var model = new AccountBookData();
            model.UserId = userId;
            return View(model);
        }

        /// <summary>
        /// 儲存新增的帳本
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AccountBookInsertSave(AccountBookData data)
        {
            AccountBookService service = new AccountBookService(_configuration);
            if (ModelState.IsValid)
            {
                service.InsertAccountBook(data);
                return RedirectToAction("AccountBookList", "AccountingSystem", new { id = data.UserId });
            }
            return View("AccountBookInsert", data);
        }

        /// <summary>
        /// 刪除帳本
        /// </summary>
        /// <param name="accountBookId"></param>
        /// <returns></returns>
        public IActionResult AccountBookDelete(int accountBookId)
        {
            try
            {
                AccountBookService service = new AccountBookService(_configuration);
                service.DeleteAccountBookData(accountBookId);

                TempData["DeleteAccountBookSuccess"] = "刪除帳本成功";
            }
            catch (Exception ex)
            {
                Console.WriteLine("刪除帳本錯誤：" + ex.Message);

                TempData["DeleteAccountBookError"] = "刪除帳本失敗，請稍後再試";
            }
            return RedirectToAction("AccountBookList");
        }

        /// 顯示財務報表
        /// </summary>
        /// <param name="accountBookId"></param>
        /// <returns></returns>
        public IActionResult ShowReport(int accountBookId)
        {
            AccountBookService service = new AccountBookService(_configuration);
            var searchArg = new TransactionList { AccountBookId = accountBookId };
            List<TransactionList> transactions = service.GetAccountBookData(searchArg);
            ViewBag.AccountBookId = accountBookId;
            return View(transactions);
        }


        /// <summary>
        /// 取得匯率資料（轉發 tw.rter.info 的 API 給前端）
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetExchangeRates()
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    var response = await client.GetAsync("https://tw.rter.info/capi.php");
                    response.EnsureSuccessStatusCode();

                    var json = await response.Content.ReadAsStringAsync();
                    return Content(json, "application/json");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "匯率抓取失敗", message = ex.Message });
            }
        }
    }
}
