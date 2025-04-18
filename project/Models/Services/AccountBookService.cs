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
    public class AccountBookService
    {
        private readonly IConfiguration _configuration;

        public AccountBookService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        private string GetDBConnectionString()
        {
            return _configuration.GetConnectionString("DBConn");
        }

        /// <summary>
        /// 帳本列表
        /// </summary>
        /// <param name="arg">使用者編號</param>
        /// <returns></returns>
        public List<Models.AccountBookList> SearchBookData(Models.AccountBookList arg)
        {
            DataTable dt = new DataTable();
            string sql = @"SELECT OWNER, ACCOUNT_BOOK_ID, ACCOUNT_BOOK_NAME FROM ACCOUNT_BOOK WHERE OWNER = @OWNER";
            using (NpgsqlConnection conn = new NpgsqlConnection(this.GetDBConnectionString()))
            {
                conn.Open();
                NpgsqlCommand cmd = new NpgsqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@OWNER", arg.UserId);
                NpgsqlDataAdapter sqlAdapter = new NpgsqlDataAdapter(cmd);
                sqlAdapter.Fill(dt);
                conn.Close();
            }
            List<Models.AccountBookList> ListResult = new List<AccountBookList>();
            foreach (DataRow row in dt.Rows)
            {
                ListResult.Add(new AccountBookList()
                {
                    UserId = Convert.ToInt32(row["OWNER"]),
                    AccountBookId = Convert.ToInt32(row["ACCOUNT_BOOK_ID"]),
                    AccountBookName = row["ACCOUNT_BOOK_NAME"].ToString()
                });
            }

            return ListResult;
        }
    }
}
