using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace project.Models
{
    /// <summary>
    /// 登入用使用者資料
    /// </summary>
    public class LoginData
    {       
        [Required(ErrorMessage = "Email未填寫")]
        [DisplayName("Email")]
        public string Email { get; set; }

        [Required(ErrorMessage = "密碼未填寫")]
        [DisplayName("密碼")]
        public string Password { get; set; }
    }
}
