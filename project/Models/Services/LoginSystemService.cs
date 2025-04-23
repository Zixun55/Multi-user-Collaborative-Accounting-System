using Microsoft.Extensions.Configuration;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Data;
using Npgsql;
using System.Linq;
using System.Web;

namespace project.Models.Services
{
    public class LoginSystemService
    {
        private readonly IConfiguration _configuration;

        public LoginSystemService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        private string GetDBConnectionString()
        {
            return _configuration.GetConnectionString("DBConn");
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

            using (NpgsqlConnection conn = new NpgsqlConnection(this.GetDBConnectionString()))
            {
                conn.Open();
                using (NpgsqlCommand cmd = new NpgsqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@EMAIL", email);
                    cmd.Parameters.AddWithValue("@PASSWORDS", password);
                    var result = cmd.ExecuteScalar();

                    return result != null ? Convert.ToInt32(result) : (int?)null;
                }
            }
        }

        /// <summary>
        /// 檢查有無重複的Email
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        public bool CheckEmail(string email)
        {
            string sql = @"SELECT COUNT(*) FROM USERS WHERE EMAIL = @EMAIL";

            using (NpgsqlConnection conn = new NpgsqlConnection(this.GetDBConnectionString()))
            {
                conn.Open();
                using (NpgsqlCommand cmd = new NpgsqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@EMAIL", email);

                    int count = Convert.ToInt32(cmd.ExecuteScalar());
                    return count > 0;
                }
            }
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

            using (NpgsqlConnection conn = new NpgsqlConnection(this.GetDBConnectionString()))
            {
                conn.Open();
                NpgsqlCommand cmd = new NpgsqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@USER_NAME", arg.UserName);
                cmd.Parameters.AddWithValue("@EMAIL", arg.Email);
                cmd.Parameters.AddWithValue("@PASSWORDS", arg.Password);
                cmd.ExecuteNonQuery();
                conn.Close();
            }
        }
    }
}
