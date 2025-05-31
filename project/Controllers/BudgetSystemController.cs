using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using project.Models;
using project.Models.Services;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace project.Controllers
{
    public class BudgetController : Controller
    {
        private readonly AccountBookService _service;

        public BudgetController(AccountBookService service)
        {
            _service = service;
        }
        // 顯示指定賬簿的所有預算
        public ActionResult Index(int accountBookID)
        {
            // 1. 使用新的服務方法獲取特定帳本的預算列表
            List<BudgetList> budgets = _service.GetBudgetsForAccountBook(accountBookID);

            // 2. 獲取帳本名稱
            var accountBook = _service.SearchAccountBook(accountBookID);
            string accountBookName = accountBook?.AccountBookName ?? "未命名帳簿";

            // 3. 建立視圖模型
            var viewModel = new BudgetViewModel
            {
                Budgets = budgets, // budgets 現在是從 Budget 資料表來的
                AccountBookID = accountBookID,
                BudgetName = accountBookName
            };

            ViewBag.AccountBookId = accountBookID; // 方便 View 中使用
            return View(viewModel);
        }

        //// 顯示指定賬簿的所有預算
        //public ActionResult Index(int accountBookID)
        //{
        //    var arg = new TransactionList { AccountBookId = accountBookID };
        //    var transactions = _service.GetAccountBookData(arg);

        //    // 轉換為 Budget 列表
        //    var budgets = transactions.Select(t => new Budget
        //    {
        //        BudgetID = t.TransactionId,
        //        Amount = t.Amount,
        //        AccountBookID = accountBookID
        //    }).ToList();

        //    // 建立視圖模型
        //    var viewModel = new BudgetViewModel
        //    {
        //        Budgets = budgets,
        //        AccountBookID = accountBookID,
        //        AccountBookName = _service.SearchAccountBook(accountBookID).AccountBookName
        //    };
        //    ViewBag.AccountBookId = accountBookID;

        //    return View(viewModel);  // 或 return View("BudgetList", viewModel);
        //}


        // 顯示單個預算詳情
        public ActionResult Details(
        int budgetID,
        int accountBookID,
        string category = null,
        string start = null,
        string end = null)
        {
            var budget = _service.GetBudgetById(budgetID);
            if (budget == null) return NotFound();

            // 轉換日期參數
            DateTime? startDate = string.IsNullOrEmpty(start) ? (DateTime?)null : DateTime.Parse(start);
            DateTime? endDate = string.IsNullOrEmpty(end) ? (DateTime?)null : DateTime.Parse(end);

            // 呼叫 Service 層過濾方法
            var includedTransactions = _service.GetTransactionsIncludedInBudget(
                accountBookID,
                category,
                startDate,
                endDate
            );

            // 計算預算摘要
            decimal totalBudget = budget.Amount;
            decimal totalExpenses = includedTransactions.Sum(t => t.Amount);
            decimal remainingBudget = Math.Max(0, totalBudget - totalExpenses);
            decimal overBudget = totalExpenses > totalBudget ? totalExpenses - totalBudget : 0;
            decimal usagePercentage = totalBudget > 0 ? (totalExpenses / totalBudget) * 100 : 0;
            string budgetStatus = GetBudgetStatus(totalBudget, totalExpenses, usagePercentage);

            var viewModel = new BudgetDetailsViewModel
            {
                BudgetID = budgetID,
                AccountBookID = accountBookID,
                TotalBudget = totalBudget,
                TotalSpent = totalExpenses,
                RemainingBudget = remainingBudget,
                OverBudget = overBudget,
                UsagePercentage = usagePercentage,
                BudgetStatus = budgetStatus,
                StartDate = budget.StartDate,  // 新增：預算開始日期
                EndDate = budget.EndDate,      // 新增：預算結束日期
                IncludedTransactions = includedTransactions
            };

            return View(viewModel);
        }

        [HttpPost]
        public JsonResult GetFilteredTransactions(int budgetID, int accountBookID, string category = null, string start = null, string end = null)
        {
            var budget = _service.GetBudgetById(budgetID);
            if (budget == null) return Json(new { success = false });

            DateTime? startDate = string.IsNullOrEmpty(start) ? (DateTime?)null : DateTime.Parse(start);
            DateTime? endDate = string.IsNullOrEmpty(end) ? (DateTime?)null : DateTime.Parse(end);

            var includedTransactions = _service.GetTransactionsIncludedInBudget(accountBookID, category, startDate, endDate);

            decimal totalBudget = budget.Amount;
            decimal totalExpenses = includedTransactions.Sum(t => t.Amount);
            decimal remainingBudget = Math.Max(0, totalBudget - totalExpenses);
            decimal overBudget = totalExpenses > totalBudget ? totalExpenses - totalBudget : 0;
            decimal usagePercentage = totalBudget > 0 ? (totalExpenses / totalBudget) * 100 : 0;
            string budgetStatus = GetBudgetStatus(totalBudget, totalExpenses, usagePercentage);

            return Json(new
            {
                success = true,
                totalBudget = totalBudget,
                totalSpent = totalExpenses,
                remainingBudget = remainingBudget,
                overBudget = overBudget,
                usagePercentage = usagePercentage,
                budgetStatus = budgetStatus,
                transactions = includedTransactions.Select(t => new
                {
                    transactionId = t.TransactionId,
                    date = t.Date.ToString("yyyy/MM/dd"),
                    category = t.Category,
                    description = t.Description,
                    amount = t.Amount,
                    currency = t.Currency,
                    includeInBudget = t.IncludeInBudget
                }).ToList()
            });
        }


        //public ActionResult Details(int budgetID, int accountBookID)
        //{
        //    var budget = _service.GetBudgetById(budgetID);
        //    if (budget == null) return NotFound();

        //    decimal totalBudget = budget.Amount;
        //    decimal totalExpenses = _service.GetIncludedExpenseSum(accountBookID);
        //    decimal remainingBudget = Math.Max(0, totalBudget - totalExpenses);
        //    decimal overBudget = totalExpenses > totalBudget ? totalExpenses - totalBudget : 0;
        //    decimal usagePercentage = totalBudget > 0 ? (totalExpenses / totalBudget) * 100 : 0;
        //    string budgetStatus = GetBudgetStatus(totalBudget, totalExpenses, usagePercentage);

        //    var includedTransactions = _service.GetTransactionsIncludedInBudget(accountBookID);

        //    var viewModel = new BudgetDetailsViewModel
        //    {
        //        BudgetID = budgetID,
        //        AccountBookID = accountBookID,
        //        TotalBudget = totalBudget,
        //        TotalSpent = totalExpenses,
        //        RemainingBudget = remainingBudget,
        //        OverBudget = overBudget,
        //        UsagePercentage = usagePercentage,
        //        BudgetStatus = budgetStatus,
        //        IncludedTransactions = includedTransactions
        //    };

        //    return View(viewModel);
        //}

        //public ActionResult Details(int budgetID, int accountBookID)
        //{
        //    // 1. 獲取預算詳情
        //    var budget = _service.GetBudgetById(budgetID);
        //    if (budget == null) return NotFound();

        //    // 2. 獲取包含在預算中的交易
        //    List<TransactionData> includedTransactions =
        //        _service.GetTransactionsIncludedInBudget(accountBookID);

        //    ViewBag.AccountBookId = accountBookID; // 方便 View 中使用
        //    ViewBag.budgetID = budgetID; // 方便 View 中使用
        //    return View(includedTransactions);
        //}

        //public ActionResult Details(int budgetID, int accountBookID)
        //{
        //    var searchArg = new TransactionList { AccountBookId = accountBookID};
        //    List<TransactionList> accountBookDataResult = _service.GetAccountBookData(searchArg);
        //    ViewBag.AccountBookId = accountBookID;

        //    return View(accountBookDataResult);
        //    var transaction = _service.GetAccountBookData(new TransactionData
        //    {
        //        TransactionId = transaction.AccountBookId,
        //        AccountBookId = accountBookID
        //    });

        //    if (transaction == null)
        //    {
        //        return NotFound();
        //    }

        //    // 取得帳本名稱
        //    var accountBook = _service.SearchAccountBook(accountBookID);

        //    var budget = new BudgetList
        //    {
        //        BudgetID = transaction.TransactionId,
        //        Amount = transaction.Amount,
        //        AccountBookID = accountBookID,
        //        AccountBookName = accountBook?.AccountBookName ?? "未命名帳簿"
        //    };
        //    ViewBag.AccountBookId = accountBookID;

        //    return View(budget);
        //}

        // 顯示創建預算表單
        public ActionResult Create(int accountbookid)
        {
            var model = new Budget();
            model.AccountBookID = accountbookid;
            return View(model);
        }
        //public ActionResult Create(int accountBookID)
        //{
        //    ViewBag.Categories = GetCategoryList();
        //    return View(new BudgetList { AccountBookID = accountBookID });
        //}

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateBudget(Budget budget)
        {
            if (ModelState.IsValid)
            {
                _service.InsertBudget(budget); // 這才會寫入 Budget table
                return RedirectToAction("Index", "Budget", new { accountBookId = budget.AccountBookID });
            }
            return View("Create", budget);
        }

        public ActionResult Edit(int budgetID, int accountBookID)
        {
            var budget = _service.GetBudgetById(budgetID); // 這裡要回傳完整的 Budget 或 BudgetList
            if (budget == null) return NotFound();

            ViewBag.Categories = GetCategoryList();
            ViewBag.AccountBookId = accountBookID;
            return View(budget); // 這裡 budget 物件要有所有欄位
        }

        // 處理更新預算請求
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UpdateBudget(int budgetID, Budget budget)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Categories = GetCategoryList();
                ViewBag.AccountBookID = budget.AccountBookID;
                return View("Edit", budget); // 明確指定返回 Edit 視圖
            }

            _service.UpdateBudget(budget);

            return RedirectToAction("Index", new { accountBookID = budget.AccountBookID });
        }


        // 顯示刪除預算確認頁面
        public ActionResult Delete(int budgetID, int accountBookID)
        {
            var budget = _service.GetBudgetById(budgetID);
            if (budget == null) return NotFound();

            return View("Delete", budget);
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int budgetID, int accountBookID)
        {
            _service.DeleteBudget(budgetID, accountBookID);
            return RedirectToAction("Index", new { accountBookID });
        }


        // 輔助方法：獲取類別列表
        private List<SelectListItem> GetCategoryList()
        {
            return new List<SelectListItem>
            {
                new SelectListItem { Value = "食品", Text = "食品" },
                new SelectListItem { Value = "交通", Text = "交通" },
                new SelectListItem { Value = "住宿", Text = "住宿" },
                new SelectListItem { Value = "娛樂", Text = "娛樂" },
                new SelectListItem { Value = "其他", Text = "其他" }
            };
        }

        //public IActionResult ShowReport(int accountBookID, int budgetID)
        //{
        //    var searchArg = new TransactionList { AccountBookId = accountBookID };
        //    List<TransactionList> transactions = _service.GetAccountBookData(searchArg);

        //    // 計算總預算（假設收入為正數）
        //    decimal totalIncome = transactions.Where(t => t.Amount > 0).Sum(t => t.Amount);
        //    decimal totalExpenses = transactions.Where(t => t.Amount < 0).Sum(t => -t.Amount);
        //    decimal remainingBudget = totalIncome - totalExpenses;

        //    ViewBag.AccountBookId = accountBookID;
        //    ViewBag.TotalBudget = totalIncome;
        //    ViewBag.TotalSpent = totalExpenses;
        //    ViewBag.RemainingBudget = remainingBudget;

        //    ViewBag.accountBookID = accountBookID;
        //    ViewBag.budgetID = budgetID;

        //    return View(transactions);
        //}
        public IActionResult ShowReport(int accountBookID, int budgetID)
        {
            var budget = _service.GetBudgetById(budgetID);
            if (budget == null) return NotFound();

            // 確保 budget.Amount 不是 null
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

        // 輔助方法：判斷預算狀態
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


    }
}
