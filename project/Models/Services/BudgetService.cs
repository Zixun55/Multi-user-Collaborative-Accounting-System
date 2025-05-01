using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Linq;

namespace project.Models.Services
{
    public class BudgetService
    {
        private readonly IConfiguration _configuration;
        public BudgetService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public void SetMonthlyBudget(MonthlyBudget budget)
        {
            // 儲存預算到資料庫（略，請依專案DB設計實作）
        }

        public MonthlyBudget GetMonthlyBudget(int userId, int year, int month)
        {
            // 從資料庫取得該月預算（略，請依專案DB設計實作）
            return new MonthlyBudget();
        }

        //public BudgetStatistics GetBudgetStatistics(int userId, int year, int month)
        //{
        //    var budget = GetMonthlyBudget(userId, year, month);
        //    var expenses = GetExpenses(userId, year, month); // 假設有此方法取得消費紀錄

        //    var stats = new BudgetStatistics
        //    {
        //        TotalBudget = budget.TotalBudget,
        //        TotalSpent = expenses.Sum(e => e.Amount),
        //        CategoryStats = new List<CategoryBudgetStatistics>()
        //    };

        //    foreach (var cat in budget.CategoryBudgets)
        //    {
        //        var spent = expenses.Where(e => e.Category == cat.CategoryName).Sum(e => e.Amount);
        //        stats.CategoryStats.Add(new CategoryBudgetStatistics
        //        {
        //            CategoryName = cat.CategoryName,
        //            CategoryBudget = cat.CategoryBudgetAmount,
        //            CategorySpent = spent
        //        });
        //    }

        //    return stats;
        //}

        // 假設有這個方法
        //private List<Expense> GetExpenses(int userId, int year, int month)
        //{
        //    // 從資料庫查詢消費紀錄（略）
        //    return new List<Expense>();
        //}
    }
}
