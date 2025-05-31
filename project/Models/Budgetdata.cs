using System.ComponentModel.DataAnnotations;

namespace project.Models
{
    public class Budget
    {
        public int BudgetID { get; set; }                 // 預算唯一識別碼
        public int AccountBookID { get; set; }            // 所屬帳本ID
        [Required(ErrorMessage = "預算名稱必填")]
        public string BudgetName { get; set; }       // 帳簿名稱（導航屬性）
        [Required(ErrorMessage = "金額必填")]
        public int Amount { get; set; }                   // 預算金額
        public string Description { get; set; } = "無說明";            // 預算說明（可選）
        [Required(ErrorMessage = "幣別必填")]
        public string Currency { get; set; } = "TWD";              // 幣別（如TWD、USD，可選）
        [Required(ErrorMessage = "開始日期必填")]
        public DateTime StartDate { get; set; } = DateTime.Now;        // 預算開始日期
        [Required(ErrorMessage = "結束日期必填")]
        public DateTime EndDate { get; set; } = DateTime.Now.AddMonths(1);           // 預算結束日期
    }
}
