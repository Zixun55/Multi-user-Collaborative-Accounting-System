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
    }
}
