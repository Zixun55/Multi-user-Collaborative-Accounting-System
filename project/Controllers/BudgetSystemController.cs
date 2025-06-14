﻿using Microsoft.AspNetCore.Mvc;
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
            List<BudgetList> budgets = _service.GetBudgetsForAccountBook(accountBookID);

            var accountBook = _service.SearchAccountBook(accountBookID);
            string accountBookName = accountBook?.AccountBookName ?? "未命名帳簿";

            var viewModel = new BudgetViewModel
            {
                Budgets = budgets,
                AccountBookID = accountBookID,
                BudgetName = accountBookName
            };

            ViewBag.AccountBookId = accountBookID;
            return View(viewModel);
        }

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

        // 顯示創建預算表單
        public ActionResult Create(int accountbookid)
        {
            var model = new Budget();
            model.AccountBookID = accountbookid;
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateBudget(Budget budget)
        {
            if (ModelState.IsValid)
            {
                _service.InsertBudget(budget);
                return RedirectToAction("Index", "Budget", new { accountBookId = budget.AccountBookID });
            }
            return View("Create", budget);
        }

        public ActionResult Edit(int budgetID, int accountBookID)
        {
            var budget = _service.GetBudgetById(budgetID);
            if (budget == null) return NotFound();

            ViewBag.Categories = GetCategoryList();
            ViewBag.AccountBookId = accountBookID;
            return View(budget);
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

        public IActionResult ShowReport(int accountBookID, int budgetID)
        {
            var budget = _service.GetBudgetById(budgetID);
            if (budget == null) return NotFound();

            decimal totalBudget = budget.Amount;
            decimal totalExpenses = _service.GetIncludedExpenseSum(accountBookID);

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
