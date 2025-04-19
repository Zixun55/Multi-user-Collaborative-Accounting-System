using System.ComponentModel;

namespace project.Models
{
    /// <summary>
    /// 交易列表
    /// </summary>
    public class TransactionList
    {
        [DisplayName("交易編號")]
        public int TransactionId { get; set; }

        [DisplayName("交易日期")]
        public DateTime Date { get; set; }

        [DisplayName("交易類別")]
        public string Category { get; set; }

        [DisplayName("帳本描述")]
        public string Description { get; set; }

        [DisplayName("交易金額")]
        public int Amount { get; set; }

        [DisplayName("交易幣別")]
        public string Currency { get; set; }

        [DisplayName("帳本編號")]
        public int AccountBookId { get; set; }
    }
}

