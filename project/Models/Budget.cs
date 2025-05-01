namespace project.Models
{
    public class Budget
    {
        public int BudgetID { get; set; }
        public int Amount { get; set; }
        public int Period { get; set; }
        public int AccountBookID { get; set; }
        public string Category { get; set; }

        // 導航屬性
        public string AccountBookName { get; set; }
    }
    public class BudgetViewModel
    {
        public List<Budget> Budgets { get; set; }
        public int AccountBookID { get; set; }
        public string AccountBookName { get; set; }
    }
    public class MonthlyBudget
    {
        public int BudgetId { get; set; }
        public int UserId { get; set; }
        public int Year { get; set; }
        public int Month { get; set; }
        public decimal TotalBudget { get; set; }
        public virtual List<CategoryBudget> CategoryBudgets { get; set; }
    }

    public class CategoryBudget
    {
        public int CategoryBudgetId { get; set; }
        public int BudgetId { get; set; }
        public string CategoryName { get; set; }
        public decimal CategoryBudgetAmount { get; set; }
    }
    public class BudgetStatistics
    {
        public decimal TotalBudget { get; set; }
        public decimal TotalSpent { get; set; }
        public decimal RemainingBudget => TotalBudget - TotalSpent;
        public decimal UsedPercentage => TotalBudget == 0 ? 0 : (TotalSpent / TotalBudget) * 100;
        public List<CategoryBudgetStatistics> CategoryStats { get; set; }
    }

    public class CategoryBudgetStatistics
    {
        public string CategoryName { get; set; }
        public decimal CategoryBudget { get; set; }
        public decimal CategorySpent { get; set; }
        public decimal CategoryRemaining => CategoryBudget - CategorySpent;
        public decimal UsedPercentage => CategoryBudget == 0 ? 0 : (CategorySpent / CategoryBudget) * 100;
    }
}
