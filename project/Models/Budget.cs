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

}
