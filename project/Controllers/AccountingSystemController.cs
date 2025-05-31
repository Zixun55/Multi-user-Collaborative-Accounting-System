using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Mvc;
using project.Models;
using project.Models.Services;

namespace project.Controllers
{
    public class AccountingSystemController : Controller
    {
        private readonly AccountBookService _service;

        public AccountingSystemController(AccountBookService service)
        {
            _service = service;
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

            ViewBag.UserId = userId.ToString();

            var searchArg = new AccountBookList { UserId = userId.ToString() };
            List<AccountBookList> listResult = _service.GetAccountBookList(searchArg);

            return View(listResult);
        }

        /// <summary>
        /// 獲得帳本資料
        /// </summary>
        /// <returns></returns>
        public ActionResult AccountBookData(int id)
        {
            var searchArg = new TransactionList { AccountBookId = id };
            List<TransactionList> accountBookDataResult = _service.GetTransactionList(searchArg);
            ViewBag.AccountBookId = id;

            return View(accountBookDataResult);
        }

        /// <summary>
        /// 獲得交易資料(交易編輯頁面)
        /// </summary>
        /// <returns></returns>
        public ActionResult TransactionUpdate(int id)
        {
            var searchArg = new TransactionData { TransactionId = id };
            TransactionData accountBookDataResult = _service.GetTransactionData(searchArg);

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
            if (ModelState.IsValid)
            {
                _service.UpdateTransactionData(data);
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
            if (ModelState.IsValid)
            {
                _service.InsertTransactionData(data);
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
            _service.DeleteTransactionData(transactionId, accountBookId);

            TempData["DeleteSuccess"] = "刪除成功";
            return RedirectToAction("AccountBookData", new { id = accountBookId });
        }

        /// <summary>
        /// 顯示新增帳本頁面
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public ActionResult AccountBookInsert(string userId)
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
            if (ModelState.IsValid)
            {
                _service.InsertAccountBook(data);
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
                _service.DeleteAccountBookData(accountBookId);

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
        public IActionResult ShowReport(int accountBookID, int budgetID)
        {
            var budget = _service.GetBudgetById(budgetID);
            if (budget == null) return NotFound();

            decimal totalBudget = budget.Amount;
            decimal totalExpenses = _service.GetIncludedExpenseSum(accountBookID);

            // 修正剩餘預算計算邏輯
            decimal remainingBudget = Math.Max(0, totalBudget - totalExpenses);
            decimal overBudget = totalExpenses > totalBudget ? totalExpenses - totalBudget : 0;

            // 計算使用百分比
            decimal usagePercentage = totalBudget > 0 ? (totalExpenses / totalBudget) * 100 : 0;

            // 判斷預算狀態
            string budgetStatus = GetBudgetStatus(totalBudget, totalExpenses, usagePercentage);

            ViewBag.AccountBookId = accountBookID;
            ViewBag.TotalBudget = totalBudget;
            ViewBag.TotalSpent = totalExpenses;
            ViewBag.RemainingBudget = remainingBudget;
            ViewBag.OverBudget = overBudget;
            ViewBag.UsagePercentage = usagePercentage;
            ViewBag.BudgetStatus = budgetStatus;
            ViewBag.accountBookID = accountBookID;
            ViewBag.budgetID = budgetID;

            var includedTransactions = _service.GetTransactionsIncludedInBudget(accountBookID);
            return View(includedTransactions);
        }

        // 新增輔助方法：判斷預算狀態
        private string GetBudgetStatus(decimal totalBudget, decimal totalExpenses, decimal usagePercentage)
        {
            if (totalBudget == 0)
            {
                return totalExpenses > 0 ? "無預算但有支出" : "無預算";
            }

            if (totalExpenses >= totalBudget)
            {
                return "預算使用完畢";
            }
            else if (usagePercentage >= 90)
            {
                return "預算即將用完";
            }
            else if (usagePercentage >= 75)
            {
                return "預算使用良好";
            }
            else
            {
                return "預算充足";
            }
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

        /// <summary>
        /// 獲得帳本資料(帳本編輯頁面)
        /// </summary>
        /// <returns></returns>
        public ActionResult AccountBookUpdate(int id)
        {
            var searchArg = new AccountBookList { AccountBookId = id };
            AccountBookList accountBookDataResult = _service.GetAccountBookData(searchArg);

            return View(accountBookDataResult);
        }

        /// <summary>
        /// 儲存修改的帳本資料
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AccountBookUpdateSave(AccountBookList data)
        {
            if (ModelState.IsValid)
            {
                var emailList = data.Owners?.Split(',')?.Select(e => e.Trim()).Where(e => !string.IsNullOrEmpty(e)).ToList() ?? new List<string>();

                // 檢查 email 是否存在
                var nonExistEmails = _service.GetNonExistentUserEmails(emailList);
                if (nonExistEmails.Any())
                {
                    ModelState.AddModelError("Owners", "無此使用者：" + string.Join(", ", nonExistEmails));
                    return View("AccountBookUpdate", data);
                }

                // 取得對應 user_id 並存入
                var userIds = _service.GetUserIdsByEmails(emailList);
                data.Owners = string.Join(",", userIds);

                _service.UpdateAccountBookData(data);
                return RedirectToAction("AccountBookList", "AccountingSystem", new { id = data.AccountBookId });
            }
            return View("AccountBookUpdate", data);
        }
    }
}
