using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using project.Models;
using project.Models.Services;

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
            var arg = new TransactionList { AccountBookId = accountBookID };
            var transactions = _service.GetAccountBookData(arg);

            // 轉換為 Budget 列表
            var budgets = transactions.Select(t => new Budget
            {
                BudgetID = t.TransactionId,
                Amount = t.Amount,
                Category = t.Category,
                Period = 1,
                AccountBookID = accountBookID
            }).ToList();

            // 建立視圖模型
            var viewModel = new BudgetViewModel
            {
                Budgets = budgets,
                AccountBookID = accountBookID,
                AccountBookName = _service.SearchAccountBook(accountBookID).AccountBookName
            };
            ViewBag.AccountBookId = accountBookID;

            return View(viewModel);  // 或 return View("BudgetList", viewModel);
        }


        // 顯示單個預算詳情
        public ActionResult Details(int budgetID, int accountBookID)
        {
            var transaction = _service.GetTransactionData(new TransactionData
            {
                TransactionId = budgetID,
                AccountBookId = accountBookID
            });

            if (transaction == null)
            {
                return NotFound();
            }

            // 取得帳本名稱
            var accountBook = _service.SearchAccountBook(accountBookID);

            var budget = new Budget
            {
                BudgetID = transaction.TransactionId,
                Amount = transaction.Amount,
                Category = transaction.Category,
                AccountBookID = accountBookID,
                Period = 1,
                // 設置帳簿名稱
                AccountBookName = accountBook?.AccountBookName ?? "未命名帳簿"
            };
            ViewBag.AccountBookId = accountBookID;

            return View(budget);
        }


        // 顯示創建預算表單
        public ActionResult Create(int accountBookID)
        {
            ViewBag.Categories = GetCategoryList();
            return View(new Budget { AccountBookID = accountBookID });
        }

        // 處理創建預算請求
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateBudget(Budget budget)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Categories = GetCategoryList();
                return View(budget);
            }

            var transactionData = new TransactionData
            {
                AccountBookId = budget.AccountBookID,
                Amount = budget.Amount,
                Category = budget.Category,
                Date = DateTime.Now,
                Currency = "TWD"
            };

            _service.InsertTransactionData(transactionData);

            return RedirectToAction("Index", new { accountBookID = budget.AccountBookID });
        }

        // 顯示編輯預算表單
        public ActionResult Edit(int budgetID, int accountBookID)
        {
            var transaction = _service.GetTransactionData(new TransactionData
            {
                TransactionId = budgetID,
                AccountBookId = accountBookID
            });

            if (transaction == null)
            {
                return NotFound();
            }

            ViewBag.Categories = GetCategoryList();
            ViewBag.AccountBookId = accountBookID;

            return View(new Budget
            {
                BudgetID = transaction.TransactionId,
                Amount = transaction.Amount,
                Category = transaction.Category,
                AccountBookID = accountBookID,
                Period = 1
            });
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

            var transactionData = new TransactionData
            {
                TransactionId = budgetID,
                AccountBookId = budget.AccountBookID,
                Amount = budget.Amount,
                Category = budget.Category,
                Date = DateTime.Now,
                Currency = "TWD"
            };

            _service.UpdateTransactionData(transactionData);

            return RedirectToAction("Index", new { accountBookID = budget.AccountBookID });
        }


        // 顯示刪除預算確認頁面
        public ActionResult Delete(int budgetID, int accountBookID)
        {
            // 取得交易資料
            var transaction = _service.GetTransactionData(new TransactionData
            {
                TransactionId = budgetID,
                AccountBookId = accountBookID
            });

            if (transaction == null)
            {
                return NotFound();
            }

            // 取得帳本資訊
            var arg2 = new AccountBookList { AccountBookId = accountBookID };
            var accountBookList = _service.GetAccountBookList(arg2);

            // 建立完整的預算模型
            var budget = new Budget
            {
                BudgetID = transaction.TransactionId,
                Amount = transaction.Amount,
                Category = transaction.Category,
                AccountBookID = accountBookID,
                Period = 1 // 或從其他地方取得
            };

            // 傳遞單一預算對象，而非集合
            return View("BudgetDelete", budget);
        }


        // 處理刪除預算請求
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteBudget(int budgetID, int accountBookID)
        {
            _service.DeleteTransactionData(budgetID, accountBookID);
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

        public IActionResult ShowReport(int accountBookID)
        {
            var searchArg = new TransactionList { AccountBookId = accountBookID };
            List<TransactionList> transactions = _service.GetAccountBookData(searchArg);

            // 計算總預算（假設收入為正數）
            decimal totalIncome = transactions.Where(t => t.Amount > 0).Sum(t => t.Amount);
            decimal totalExpenses = transactions.Where(t => t.Amount < 0).Sum(t => -t.Amount);
            decimal remainingBudget = totalIncome - totalExpenses;

            ViewBag.AccountBookId = accountBookID;
            ViewBag.TotalBudget = totalIncome;
            ViewBag.TotalSpent = totalExpenses;
            ViewBag.RemainingBudget = remainingBudget;

            return View(transactions);
        }

    }
}
