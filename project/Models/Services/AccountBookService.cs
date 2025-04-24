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
        /// 取得帳本列表
        /// </summary>
        /// <param name="arg">使用者編號</param>
        /// <returns></returns>
        public List<Models.AccountBookList> GetAccountBookList(Models.AccountBookList arg)
        {
            DataTable dt = new DataTable();
            string sql = @"SELECT OWNER, ACCOUNT_BOOK_ID, ACCOUNT_BOOK_NAME, DESCRIPTION FROM ACCOUNT_BOOK WHERE OWNER = @OWNER";
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
                    AccountBookName = row["ACCOUNT_BOOK_NAME"].ToString(),
                    Description = Convert.ToString(row["Description"])
                });
            }

            return ListResult;
        }

        /// <summary>
        /// 取得帳本資料
        /// </summary>
        /// <param name="arg">帳本編號</param>
        /// <returns></returns>
        public List<Models.TransactionList> GetAccountBookData(Models.TransactionList arg)
        {
            DataTable dt = new DataTable();
            string sql = @"SELECT TRANSACTION_ID, ACCOUNT_BOOK_ID, DATE, AMOUNT, DESCRIPTION, TRANSACTION_CURRENCY, CATEGORY FROM TRANSACTION WHERE ACCOUNT_BOOK_ID = @ACCOUNT_BOOK_ID";
            using (NpgsqlConnection conn = new NpgsqlConnection(this.GetDBConnectionString()))
            {
                conn.Open();
                NpgsqlCommand cmd = new NpgsqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@ACCOUNT_BOOK_ID", arg.AccountBookId);
                NpgsqlDataAdapter sqlAdapter = new NpgsqlDataAdapter(cmd);
                sqlAdapter.Fill(dt);
                conn.Close();
            }
            List<Models.TransactionList> ListResult = new List<TransactionList>();
            foreach (DataRow row in dt.Rows)
            {
                ListResult.Add(new TransactionList()
                {
                    TransactionId = Convert.ToInt32(row["TRANSACTION_ID"]),
                    AccountBookId = Convert.ToInt32(row["ACCOUNT_BOOK_ID"]),
                    Date = Convert.ToDateTime(row["DATE"]),
                    Description = Convert.ToString(row["DESCRIPTION"]),
                    Category = Convert.ToString(row["CATEGORY"]),
                    Amount = Convert.ToInt32(row["AMOUNT"]),
                    Currency = Convert.ToString(row["TRANSACTION_CURRENCY"])
                });
            }

            return ListResult;
        }

        /// <summary>
        /// 取得交易資料
        /// </summary>
        /// <param name="arg">交易編號</param>
        /// <returns></returns>
        public TransactionData GetTransactionData(Models.TransactionData arg)
        {
            DataTable dt = new DataTable();
            string sql = @"SELECT TRANSACTION_ID, ACCOUNT_BOOK_ID, DATE, AMOUNT, DESCRIPTION, TRANSACTION_CURRENCY, CATEGORY FROM TRANSACTION WHERE TRANSACTION_ID=@TRANSACTION_ID";
            using (NpgsqlConnection conn = new NpgsqlConnection(this.GetDBConnectionString()))
            {
                conn.Open();
                NpgsqlCommand cmd = new NpgsqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@TRANSACTION_ID", arg.TransactionId);
                NpgsqlDataAdapter sqlAdapter = new NpgsqlDataAdapter(cmd);
                sqlAdapter.Fill(dt);
                conn.Close();
            }
            TransactionData TransactionResult = new TransactionData();
            foreach (DataRow row in dt.Rows)
            {
                TransactionResult.TransactionId = Convert.ToInt32(row["TRANSACTION_ID"]);
                TransactionResult.AccountBookId = Convert.ToInt32(row["ACCOUNT_BOOK_ID"]);
                TransactionResult.Date = Convert.ToDateTime(row["DATE"]);
                TransactionResult.Description = Convert.ToString(row["DESCRIPTION"]);
                TransactionResult.Category = Convert.ToString(row["CATEGORY"]);
                TransactionResult.Amount = Convert.ToInt32(row["AMOUNT"]);
                TransactionResult.Currency = Convert.ToString(row["TRANSACTION_CURRENCY"]);
            }

            return TransactionResult;
        }

        /// <summary>
        /// 編輯交易紀錄
        /// </summary>
        /// <param name="data"></param>
        public void UpdateTransactionData(TransactionData arg)
        {
            string sql = @"UPDATE TRANSACTION
                   SET DATE = @DATE,
                       AMOUNT = @AMOUNT,
                       DESCRIPTION = @DESCRIPTION,
                       TRANSACTION_CURRENCY = @CURRENCY,
                       CATEGORY = @CATEGORY
                   WHERE TRANSACTION_ID = @TRANSACTION_ID AND ACCOUNT_BOOK_ID = @ACCOUNT_BOOK_ID";

            using (NpgsqlConnection conn = new NpgsqlConnection(this.GetDBConnectionString()))
            {
                conn.Open();
                NpgsqlCommand cmd = new NpgsqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@DATE", arg.Date);
                cmd.Parameters.AddWithValue("@AMOUNT", arg.Amount);
                cmd.Parameters.AddWithValue("@DESCRIPTION", arg.Description);
                cmd.Parameters.AddWithValue("@CURRENCY", arg.Currency);
                cmd.Parameters.AddWithValue("@CATEGORY", arg.Category);
                cmd.Parameters.AddWithValue("@TRANSACTION_ID", arg.TransactionId);
                cmd.Parameters.AddWithValue("@ACCOUNT_BOOK_ID", arg.AccountBookId);
                cmd.ExecuteNonQuery();
                conn.Close();
            }
        }

        /// <summary>
        /// 新增交易紀錄
        /// </summary>
        /// <param name="data"></param>
        public void InsertTransactionData(TransactionData arg)
        {
            string sql = @"INSERT INTO TRANSACTION 
                            (ACCOUNT_BOOK_ID, DATE, AMOUNT, DESCRIPTION, TRANSACTION_CURRENCY, CATEGORY) 
                            VALUES (@ACCOUNT_BOOK_ID, @DATE, @AMOUNT, @DESCRIPTION, @CURRENCY, @CATEGORY)";

            using (NpgsqlConnection conn = new NpgsqlConnection(this.GetDBConnectionString()))
            {
                conn.Open();
                NpgsqlCommand cmd = new NpgsqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@DATE", arg.Date);
                cmd.Parameters.AddWithValue("@AMOUNT", arg.Amount);
                cmd.Parameters.AddWithValue("@DESCRIPTION", arg.Description);
                cmd.Parameters.AddWithValue("@CURRENCY", arg.Currency);
                cmd.Parameters.AddWithValue("@CATEGORY", arg.Category);
                cmd.Parameters.AddWithValue("@ACCOUNT_BOOK_ID", arg.AccountBookId);
                cmd.ExecuteNonQuery();
                conn.Close();
            }
        }

        /// <summary>
        /// 刪除交易紀錄
        /// </summary>
        /// <param name="transactionId"></param>
        public void DeleteTransactionData(int transactionId, int accountBookId)
        {
            string sql = @"DELETE FROM TRANSACTION WHERE TRANSACTION_ID = @TRANSACTION_ID AND ACCOUNT_BOOK_ID = @ACCOUNT_BOOK_ID";

            using (NpgsqlConnection conn = new NpgsqlConnection(this.GetDBConnectionString()))
            {
                conn.Open();
                NpgsqlCommand cmd = new NpgsqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@TRANSACTION_ID", transactionId);
                cmd.Parameters.AddWithValue("@ACCOUNT_BOOK_ID", accountBookId);
                cmd.ExecuteNonQuery();
                conn.Close();
            }
        }

        /// <summary>
        /// 新增帳本
        /// </summary>
        /// <param name="arg"></param>
        public void InsertAccountBook(AccountBookData arg)
        {
            string sql = @"INSERT INTO ACCOUNT_BOOK 
                            (ACCOUNT_BOOK_NAME, DESCRIPTION, BASE_CURRENCY, OWNER) 
                            VALUES (@ACCOUNT_BOOK_NAME, @DESCRIPTION, @BASE_CURRENCY, @OWNER)";

            using (NpgsqlConnection conn = new NpgsqlConnection(this.GetDBConnectionString()))
            {
                conn.Open();
                NpgsqlCommand cmd = new NpgsqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@ACCOUNT_BOOK_NAME", arg.AccountBookName);
                cmd.Parameters.AddWithValue("@DESCRIPTION", arg.Description);
                cmd.Parameters.AddWithValue("@BASE_CURRENCY", arg.BaseCurrency);
                cmd.Parameters.AddWithValue("@OWNER", arg.UserId);
                cmd.ExecuteNonQuery();
                conn.Close();
            }
        }

        /// <summary>
        /// 刪除帳本(會同時刪除ACCOUNT_BOOK和TRANSACTION的資料)
        /// </summary>
        /// <param name="accountBookId"></param>
        public void DeleteAccountBookData(int accountBookId)
        {
            string deleteAccountBookSql = @"DELETE FROM ACCOUNT_BOOK WHERE ACCOUNT_BOOK_ID = @ACCOUNT_BOOK_ID";
            string deleteTransactionsSql = @"DELETE FROM TRANSACTION WHERE ACCOUNT_BOOK_ID = @ACCOUNT_BOOK_ID";

            using (NpgsqlConnection conn = new NpgsqlConnection(this.GetDBConnectionString()))
            {
                conn.Open();
                using (var transaction = conn.BeginTransaction())
                {
                    try
                    {
                        // 刪除交易紀錄
                        using (var cmd1 = new NpgsqlCommand(deleteTransactionsSql, conn))
                        {
                            cmd1.Transaction = transaction;
                            cmd1.Parameters.AddWithValue("@ACCOUNT_BOOK_ID", accountBookId);
                            cmd1.ExecuteNonQuery();
                        }

                        // 刪除帳本
                        using (var cmd2 = new NpgsqlCommand(deleteAccountBookSql, conn))
                        {
                            cmd2.Transaction = transaction;
                            cmd2.Parameters.AddWithValue("@ACCOUNT_BOOK_ID", accountBookId);
                            cmd2.ExecuteNonQuery();
                        }

                        transaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        throw new Exception("刪除帳本失敗：" + ex.Message);
                    }
                }
            }
        }
    }
}
