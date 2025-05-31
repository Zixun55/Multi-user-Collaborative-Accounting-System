namespace project.Models
{
    public class BudgetList
    {
        public int BudgetID { get; set; }                 // 預算唯一識別碼
        public int AccountBookID { get; set; }            // 所屬帳本ID
        public string BudgetName { get; set; }       // 帳簿名稱（導航屬性）
        public int Amount { get; set; }                   // 預算金額
        public string Description { get; set; }           // 預算說明（可選）
        public string Currency { get; set; }              // 幣別（如TWD、USD，可選）
        public DateTime StartDate { get; set; }          // 預算開始日期
        public DateTime EndDate { get; set; }            // 預算結束日期
    }
    public class BudgetViewModel
    {
        public List<BudgetList> Budgets { get; set; }
        public int AccountBookID { get; set; }
        public string BudgetName { get; set; }
    }

    public class BudgetDetailsViewModel
    {
        public int BudgetID { get; set; }
        public int AccountBookID { get; set; }
        public decimal TotalBudget { get; set; }
        public decimal TotalSpent { get; set; }
        public decimal RemainingBudget { get; set; }
        public decimal OverBudget { get; set; }
        public decimal UsagePercentage { get; set; }
        public string BudgetStatus { get; set; }
        public List<TransactionData> IncludedTransactions { get; set; }
    }

}
