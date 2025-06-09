using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace project.Models
{
    /// <summary>
    /// User基本欄位：Email, Password
    /// </summary>
    public class BaseUserData
    {
        [Required(ErrorMessage = "Email未填寫")]
        [DisplayName("Email")]
        public string Email { get; set; }

        [Required(ErrorMessage = "密碼未填寫")]
        [DisplayName("密碼")]
        public string Password { get; set; }
    }

    /// <summary>
    /// 註冊用使用者資料
    /// </summary>
    public class UserData: BaseUserData
    {
        [Required(ErrorMessage = "使用者名稱未填寫")]
        [DisplayName("使用者名稱")]
        public string UserName { get; set; }
    }

    /// <summary>
    /// 登入用使用者資料
    /// </summary>
    public class LoginData: BaseUserData
    {
    }
}
