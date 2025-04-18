using System.ComponentModel;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace project.Models
{
    /// <summary>
    /// 使用者資料
    /// </summary>
    public class UserData
    {       
        [DisplayName("使用者編號")]
        public int UserId { get; set; }

        [DisplayName("使用者名稱")]
        public string UserName { get; set; }

        [DisplayName("Email")]
        public string Email { get; set; }

        [DisplayName("密碼")]
        public string Password { get; set; }
    }
}
