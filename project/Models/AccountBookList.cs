using System.ComponentModel;

namespace project.Models
{
    /// <summary>
    /// 帳本列表
    /// </summary>
    public class AccountBookList
    {
        [DisplayName("使用者編號")]
        public string UserId { get; set; }

        [DisplayName("帳本編號")]
        public int AccountBookId { get; set; }

        [DisplayName("帳本名稱")]
        public string AccountBookName { get; set; }
        
        [DisplayName("帳本描述")]
        public string Description { get; set; }

        [DisplayName("貨幣單位")]
        public string BaseCurrency { get; set; }

        [DisplayName("編輯人員")]
        public string Owners { get; set; }
    }
}
