using System;
using System.Collections.Generic;
using System.Data;
using project.Models.Interfaces;

namespace project.Models.Services
{
    public class LoginSystemService
    {
        private readonly IDatabaseHelper _dbHelper;

        public LoginSystemService(IDatabaseHelper dbHelper)
        {
            _dbHelper = dbHelper;
        }

        /// <summary>
        /// 取得使用者編號
        /// </summary>
        /// <param name="email"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public int? GetUserId(string email, string password)
        {
            string sql = @"SELECT USER_ID FROM USERS WHERE EMAIL = @EMAIL AND PASSWORDS = @PASSWORDS";

            var parameters = new Dictionary<string, object>
            {
                { "@EMAIL", email },
                { "@PASSWORDS", password }
            };

            object result = _dbHelper.ExecuteScalar(sql, parameters);

            return result != null ? Convert.ToInt32(result) : (int?)null;
        }

        /// <summary>
        /// 檢查有無重複的Email
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        public bool CheckEmail(string email)
        {
            string sql = @"SELECT COUNT(*) FROM USERS WHERE EMAIL = @EMAIL";

            var parameters = new Dictionary<string, object>
            {
                { "@EMAIL", email }
            };

            object result = _dbHelper.ExecuteScalar(sql, parameters);
            int count = result != null ? Convert.ToInt32(result) : 0;

            return count > 0;
        }

        /// <summary>
        /// 建立新使用者(註冊)
        /// </summary>
        /// <param name="arg"></param>
        public void CreateNewUser(UserData arg)
        {
            string sql = @"INSERT INTO USERS 
                            (USER_NAME, EMAIL, PASSWORDS) 
                            VALUES (@USER_NAME, @EMAIL, @PASSWORDS)";

            var parameters = new Dictionary<string, object>
            {
                { "@USER_NAME", arg.UserName },
                { "@EMAIL", arg.Email },
                { "@PASSWORDS", arg.Password }
            };

            _dbHelper.ExecuteNonQuery(sql, parameters);
        }
    }
}
