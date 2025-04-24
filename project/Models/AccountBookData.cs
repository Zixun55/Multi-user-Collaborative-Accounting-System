using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace project.Models
{
    public class AccountBookData
    {
        [DisplayName("使用者編號")]
        public int UserId { get; set; }

        [DisplayName("帳本名稱")]
        [Required(ErrorMessage = "帳本名稱未填寫")]
        public string AccountBookName { get; set; }

        [DisplayName("帳本描述")]
        [Required(ErrorMessage = "帳本描述未填寫")]
        public string Description { get; set; }


        [DisplayName("貨幣單位")]
        [Required(ErrorMessage = "貨幣單位未填寫")]
        public string BaseCurrency { get; set; }
    }
}
