using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace project.Models
{
    /// <summary>
    /// 註冊用使用者資料
    /// </summary>
    public class UserData
    {
        [Required(ErrorMessage = "使用者名稱未填寫")]
        [DisplayName("使用者名稱")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Email未填寫")]
        [DisplayName("Email")]
        public string Email { get; set; }

        [Required(ErrorMessage = "密碼未填寫")]
        [DisplayName("密碼")]
        public string Password { get; set; }
    }
}
