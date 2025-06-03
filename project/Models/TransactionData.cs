using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace project.Models
{
    /// <summary>
    /// 交易資料
    /// </summary>
    public class TransactionData
    {
        [DisplayName("交易編號")]
        public int TransactionId { get; set; }

        [Required(ErrorMessage = "日期未填寫")]
        [DisplayName("交易日期")]
        public DateTime Date { get; set; }

        [Required(ErrorMessage = "類別未填寫")]
        [DisplayName("交易類別")]
        public string Category { get; set; }

        [Required(ErrorMessage = "描述未填寫")]
        [DisplayName("交易描述")]
        public string Description { get; set; }

        [Required(ErrorMessage = "金額未填寫")]
        [DisplayName("交易金額")]
        public int Amount { get; set; }

        [Required(ErrorMessage = "幣別未填寫")]
        [DisplayName("交易幣別")]
        public string Currency { get; set; }

        [DisplayName("帳本編號")]
        public int AccountBookId { get; set; }

        [DisplayName("是否加入預算")]
        public bool IncludeInBudget { get; set; }


    }
}
