using System.ComponentModel;

namespace project.Models
{
    public class AccountBookData
    {
        [DisplayName("使用者編號")]
        public int UserId { get; set; }

        [DisplayName("帳本名稱")]
        public string AccountBookName { get; set; }

        [DisplayName("帳本描述")]
        public string Description { get; set; }


        [DisplayName("貨幣單位")]
        public string BaseCurrency { get; set; }
    }
}
