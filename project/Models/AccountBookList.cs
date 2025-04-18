using System.ComponentModel;

namespace project.Models
{
    /// <summary>
    /// 帳本列表
    /// </summary>
    public class AccountBookList
    {
        [DisplayName("使用者編號")]
        public int UserId { get; set; }

        [DisplayName("帳本編號")]
        public int AccountBookId { get; set; }

        [DisplayName("帳本名稱")]
        public string AccountBookName { get; set; }
    }
}
