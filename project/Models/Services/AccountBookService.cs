using System;
using System.Collections.Generic;
using System.Data;
using project.Models.Interfaces;

namespace project.Models.Services
{
    public class AccountBookService
    {
        private readonly IDatabaseHelper _dbHelper;

        public AccountBookService(IDatabaseHelper dbHelper)
        {
            _dbHelper = dbHelper;
        }

        /// <summary>
        /// 取得帳本列表
        /// </summary>
        /// <param name="arg">使用者編號</param>
        /// <returns></returns>
        public List<AccountBookList> GetAccountBookList(AccountBookList arg)
        {
            string sql = @"SELECT OWNER, ACCOUNT_BOOK_ID, ACCOUNT_BOOK_NAME, DESCRIPTION FROM ACCOUNT_BOOK WHERE ',' || OWNER || ',' LIKE '%,' || @OWNER || ',%'";

            var parameters = new Dictionary<string, object>
            {
                { "@OWNER", arg.UserId }
            };

            DataTable dt = _dbHelper.ExecuteQuery(sql, parameters);
            List<AccountBookList> result = new();

            foreach (DataRow row in dt.Rows)
            {
                result.Add(new AccountBookList
                {
                    UserId = row["OWNER"].ToString(),
                    AccountBookId = Convert.ToInt32(row["ACCOUNT_BOOK_ID"]),
                    AccountBookName = row["ACCOUNT_BOOK_NAME"].ToString(),
                    Description = row["DESCRIPTION"].ToString()
                });
            }

            return result;
        }

        /// <summary>
        /// 取得交易資料列表
        /// </summary>
        /// <param name="arg">帳本編號</param>
        /// <returns></returns>
        public List<TransactionList> GetTransactionList(TransactionList arg)
        {
            string sql = @"SELECT TRANSACTION_ID, ACCOUNT_BOOK_ID, DATE, AMOUNT, DESCRIPTION, TRANSACTION_CURRENCY, CATEGORY 
                           FROM TRANSACTION WHERE ACCOUNT_BOOK_ID = @ACCOUNT_BOOK_ID";

            var parameters = new Dictionary<string, object>
            {
                { "@ACCOUNT_BOOK_ID", arg.AccountBookId }
            };

            DataTable dt = _dbHelper.ExecuteQuery(sql, parameters);
            List<TransactionList> result = new();

            foreach (DataRow row in dt.Rows)
            {
                result.Add(new TransactionList
                {
                    TransactionId = Convert.ToInt32(row["TRANSACTION_ID"]),
                    AccountBookId = Convert.ToInt32(row["ACCOUNT_BOOK_ID"]),
                    Date = Convert.ToDateTime(row["DATE"]),
                    Description = row["DESCRIPTION"].ToString(),
                    Category = row["CATEGORY"].ToString(),
                    Amount = Convert.ToInt32(row["AMOUNT"]),
                    Currency = row["TRANSACTION_CURRENCY"].ToString()
                });
            }

            return result;
        }

        /// <summary>
        /// 取得交易資料
        /// </summary>
        /// <param name="arg">交易編號</param>
        /// <returns></returns>
        public TransactionData GetTransactionData(TransactionData arg)
        {
            string sql = @"SELECT TRANSACTION_ID, ACCOUNT_BOOK_ID, DATE, AMOUNT, DESCRIPTION, TRANSACTION_CURRENCY, CATEGORY 
                           FROM TRANSACTION WHERE TRANSACTION_ID = @TRANSACTION_ID";

            var parameters = new Dictionary<string, object>
            {
                { "@TRANSACTION_ID", arg.TransactionId }
            };

            DataTable dt = _dbHelper.ExecuteQuery(sql, parameters);
            TransactionData result = new();

            foreach (DataRow row in dt.Rows)
            {
                result = new TransactionData
                {
                    TransactionId = Convert.ToInt32(row["TRANSACTION_ID"]),
                    AccountBookId = Convert.ToInt32(row["ACCOUNT_BOOK_ID"]),
                    Date = Convert.ToDateTime(row["DATE"]),
                    Description = row["DESCRIPTION"].ToString(),
                    Category = row["CATEGORY"].ToString(),
                    Amount = Convert.ToInt32(row["AMOUNT"]),
                    Currency = row["TRANSACTION_CURRENCY"].ToString()
                };
            }

            return result;
        }

        /// <summary>
        /// 編輯交易紀錄
        /// </summary>
        /// <param name="data"></param>
        public void UpdateTransactionData(TransactionData arg)
        {
            string sql = @"UPDATE TRANSACTION SET 
                                DATE = @DATE,
                                AMOUNT = @AMOUNT,
                                DESCRIPTION = @DESCRIPTION,
                                TRANSACTION_CURRENCY = @CURRENCY,
                                CATEGORY = @CATEGORY
                           WHERE TRANSACTION_ID = @TRANSACTION_ID AND ACCOUNT_BOOK_ID = @ACCOUNT_BOOK_ID";

            var parameters = new Dictionary<string, object>
            {
                { "@DATE", arg.Date },
                { "@AMOUNT", arg.Amount },
                { "@DESCRIPTION", arg.Description },
                { "@CURRENCY", arg.Currency },
                { "@CATEGORY", arg.Category },
                { "@TRANSACTION_ID", arg.TransactionId },
                { "@ACCOUNT_BOOK_ID", arg.AccountBookId }
            };

            _dbHelper.ExecuteNonQuery(sql, parameters);
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

            var parameters = new Dictionary<string, object>
            {
                { "@ACCOUNT_BOOK_ID", arg.AccountBookId },
                { "@DATE", arg.Date },
                { "@AMOUNT", arg.Amount },
                { "@DESCRIPTION", arg.Description },
                { "@CURRENCY", arg.Currency },
                { "@CATEGORY", arg.Category }
            };

            _dbHelper.ExecuteNonQuery(sql, parameters);
        }

        /// <summary>
        /// 刪除交易紀錄
        /// </summary>
        /// <param name="transactionId"></param>
        public void DeleteTransactionData(int transactionId, int accountBookId)
        {
            string sql = @"DELETE FROM TRANSACTION WHERE TRANSACTION_ID = @TRANSACTION_ID AND ACCOUNT_BOOK_ID = @ACCOUNT_BOOK_ID";

            var parameters = new Dictionary<string, object>
            {
                { "@TRANSACTION_ID", transactionId },
                { "@ACCOUNT_BOOK_ID", accountBookId }
            };

            _dbHelper.ExecuteNonQuery(sql, parameters);
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

            var parameters = new Dictionary<string, object>
            {
                { "@ACCOUNT_BOOK_NAME", arg.AccountBookName },
                { "@DESCRIPTION", arg.Description },
                { "@BASE_CURRENCY", arg.BaseCurrency },
                { "@OWNER", arg.UserId }
            };

            _dbHelper.ExecuteNonQuery(sql, parameters);
        }

        /// <summary>
        /// 刪除帳本(會同時刪除ACCOUNT_BOOK和TRANSACTION的資料)
        /// </summary>
        /// <param name="accountBookId"></param>
        public void DeleteAccountBookData(int accountBookId)
        {
            string deleteTransactionsSql = @"DELETE FROM TRANSACTION WHERE ACCOUNT_BOOK_ID = @ACCOUNT_BOOK_ID";
            string deleteAccountBookSql = @"DELETE FROM ACCOUNT_BOOK WHERE ACCOUNT_BOOK_ID = @ACCOUNT_BOOK_ID";

            var parameters = new Dictionary<string, object>
            {
                { "@ACCOUNT_BOOK_ID", accountBookId }
            };

            _dbHelper.ExecuteTransaction(new List<(string, Dictionary<string, object>)>
            {
                (deleteTransactionsSql, parameters),
                (deleteAccountBookSql, parameters)
            });
        }

        /// <summary>
        /// 查詢帳本
        /// </summary>
        /// <param name="arg"></param>
        public AccountBookData SearchAccountBook(int accountBookId)
        {
            string sql = @"SELECT DESCRIPTION, ACCOUNT_BOOK_NAME 
                   FROM ACCOUNT_BOOK 
                   WHERE ACCOUNT_BOOK_ID = @ACCOUNT_BOOK_ID";

            var parameters = new Dictionary<string, object>
            {
                { "@ACCOUNT_BOOK_ID", accountBookId }
            };

            DataTable dt = _dbHelper.ExecuteQuery(sql, parameters);

            if (dt.Rows.Count == 0)
            {
                return null;
            }

            DataRow row = dt.Rows[0];
            return new AccountBookData
            {
                Description = row["DESCRIPTION"].ToString(),
                AccountBookName = row["ACCOUNT_BOOK_NAME"].ToString()
            };
        }

        /// <summary>
        /// 取得與USER_ID對應的email
        /// </summary>
        /// <param name="userIds"></param>
        /// <returns></returns>
        public List<string> GetEmailsByUserIds(List<int> userIds)
        {
            if (userIds == null || userIds.Count == 0)
                return new List<string>();

            var idParams = new List<string>();
            var parameters = new Dictionary<string, object>();

            for (int i = 0; i < userIds.Count; i++)
            {
                string paramName = $"@Id{i}";
                idParams.Add(paramName);
                parameters[paramName] = userIds[i];
            }

            string sql = $"SELECT EMAIL FROM USERS WHERE USER_ID IN ({string.Join(",", idParams)})";

            DataTable dt = _dbHelper.ExecuteQuery(sql, parameters);

            return dt.AsEnumerable()
                     .Select(row => row["EMAIL"].ToString())
                     .ToList();
        }

        /// <summary>
        /// 取得帳本資料(修改帳本資料)
        /// </summary>
        /// <param name="arg"></param>
        /// <returns></returns>
        public AccountBookList GetAccountBookData(AccountBookList arg)
        {
            string sql = @"SELECT ACCOUNT_BOOK_ID, ACCOUNT_BOOK_NAME, DESCRIPTION, BASE_CURRENCY, OWNER 
                            FROM ACCOUNT_BOOK WHERE ACCOUNT_BOOK_ID = @ACCOUNT_BOOK_ID";

            var parameters = new Dictionary<string, object>
            {
                { "@ACCOUNT_BOOK_ID", arg.AccountBookId }
            };

            DataTable dt = _dbHelper.ExecuteQuery(sql, parameters);
            AccountBookList result = new();

            foreach (DataRow row in dt.Rows)
            {
                var ownerIdsStr = row["OWNER"].ToString();
                var ownerEmails = new List<string>();

                if (!string.IsNullOrEmpty(ownerIdsStr))
                {
                    var ownerIdList = ownerIdsStr.Split(',')
                                                 .Select(id => int.TryParse(id, out var parsed) ? parsed : -1)
                                                 .Where(id => id != -1)
                                                 .ToList();

                    ownerEmails = GetEmailsByUserIds(ownerIdList);
                }

                result = new AccountBookList
                {
                    AccountBookId = Convert.ToInt32(row["ACCOUNT_BOOK_ID"]),
                    Description = row["DESCRIPTION"].ToString(),
                    AccountBookName = row["ACCOUNT_BOOK_NAME"].ToString(),
                    BaseCurrency = row["BASE_CURRENCY"].ToString(),
                    Owners = string.Join(",", ownerEmails),
                    UserId = row["OWNER"].ToString()
                };
            }

            return result;
        }

        /// <summary>
        /// 儲存修改的帳本資料
        /// </summary>
        /// <param name="arg"></param>
        public void UpdateAccountBookData(AccountBookList arg)
        {
            string sql = @"UPDATE ACCOUNT_BOOK SET 
                                ACCOUNT_BOOK_NAME = @ACCOUNT_BOOK_NAME,
                                DESCRIPTION = @DESCRIPTION,
                                BASE_CURRENCY = @BASE_CURRENCY,
                                OWNER = @OWNER
                           WHERE ACCOUNT_BOOK_ID = @ACCOUNT_BOOK_ID";

            var parameters = new Dictionary<string, object>
            {
                { "@ACCOUNT_BOOK_NAME", arg.AccountBookName },
                { "@DESCRIPTION", arg.Description },
                { "@BASE_CURRENCY", arg.BaseCurrency },
                { "@OWNER", arg.Owners },
                { "@ACCOUNT_BOOK_ID", arg.AccountBookId }
            };

            _dbHelper.ExecuteNonQuery(sql, parameters);
        }

        /// <summary>
        /// 檢查email
        /// </summary>
        /// <param name="emails"></param>
        /// <returns></returns>
        public List<string> GetNonExistentUserEmails(List<string> emails)
        {
            if (emails == null || emails.Count == 0)
                return new List<string>();

            var emailParams = new List<string>();
            var parameters = new Dictionary<string, object>();

            for (int i = 0; i < emails.Count; i++)
            {
                string paramName = $"@Email{i}";
                emailParams.Add(paramName);
                parameters[paramName] = emails[i];
            }

            string sql = $"SELECT EMAIL FROM USERS WHERE EMAIL IN ({string.Join(",", emailParams)})";

            DataTable dt = _dbHelper.ExecuteQuery(sql, parameters);

            var existingEmails = dt.AsEnumerable()
                                   .Select(row => row["EMAIL"].ToString())
                                   .ToList();

            var nonExistentEmails = emails.Except(existingEmails, StringComparer.OrdinalIgnoreCase).ToList();

            return nonExistentEmails;
        }

        /// <summary>
        /// 取得與email對應的USER_ID
        /// </summary>
        /// <param name="emails"></param>
        /// <returns></returns>
        public List<int> GetUserIdsByEmails(List<string> emails)
        {
            if (emails == null || emails.Count == 0)
                return new List<int>();

            var emailParams = new List<string>();
            var parameters = new Dictionary<string, object>();

            for (int i = 0; i < emails.Count; i++)
            {
                string paramName = $"@Email{i}";
                emailParams.Add(paramName);
                parameters[paramName] = emails[i];
            }

            string sql = $"SELECT USER_ID FROM USERS WHERE EMAIL IN ({string.Join(",", emailParams)})";

            DataTable dt = _dbHelper.ExecuteQuery(sql, parameters);

            var userIds = dt.AsEnumerable()
                            .Select(row => Convert.ToInt32(row["USER_ID"]))
                            .ToList();

            return userIds;
        }
    }
}
