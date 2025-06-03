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
            string sql = @"SELECT TRANSACTION_ID, ACCOUNT_BOOK_ID, DATE, AMOUNT, DESCRIPTION, TRANSACTION_CURRENCY, CATEGORY , INCLUDE_IN_BUDGET
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
                    Currency = row["TRANSACTION_CURRENCY"].ToString(),
                    IncludeInBudget = Convert.ToBoolean(row["INCLUDE_IN_BUDGET"])
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
                    CATEGORY = @CATEGORY,
                    INCLUDE_IN_BUDGET = @INCLUDE_IN_BUDGET
               WHERE TRANSACTION_ID = @TRANSACTION_ID AND ACCOUNT_BOOK_ID = @ACCOUNT_BOOK_ID";


            var parameters = new Dictionary<string, object>
            {
                { "@DATE", arg.Date },
                { "@AMOUNT", arg.Amount },
                { "@DESCRIPTION", arg.Description },
                { "@CURRENCY", arg.Currency },
                { "@CATEGORY", arg.Category },
                { "@INCLUDE_IN_BUDGET", arg.IncludeInBudget },
                { "@TRANSACTION_ID", arg.TransactionId },
                { "@ACCOUNT_BOOK_ID", arg.AccountBookId }
            };

            _dbHelper.ExecuteNonQuery(sql, parameters);
        }

        /// <summary>
        /// 新增交易紀錄
        /// </summary>
        /// <param name="data"></param>
        /// <summary>
        /// 新增交易紀錄
        /// </summary>
        /// <param name="data"></param>
        public void InsertTransactionData(TransactionData arg)
        {
            string sql = @"INSERT INTO TRANSACTION 
                   (ACCOUNT_BOOK_ID, DATE, AMOUNT, DESCRIPTION, TRANSACTION_CURRENCY, CATEGORY, INCLUDE_IN_BUDGET) 
                   VALUES (@ACCOUNT_BOOK_ID, @DATE, @AMOUNT, @DESCRIPTION, @CURRENCY, @CATEGORY, @INCLUDE_IN_BUDGET)";

            var parameters = new Dictionary<string, object>
            {
                { "@ACCOUNT_BOOK_ID", arg.AccountBookId },
                { "@DATE", arg.Date },
                { "@AMOUNT", arg.Amount },
                { "@DESCRIPTION", arg.Description },
                { "@CURRENCY", arg.Currency },
                { "@CATEGORY", arg.Category },
                { "@INCLUDE_IN_BUDGET", arg.IncludeInBudget }
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
        /// 預算查詢
        /// </summary>
        /// <param name="accountBookID"></param>
        public List<BudgetList> GetBudgetsForAccountBook(int accountBookId)
        {
            string sql = @"SELECT BudgetID, accountbookid, BudgetName, Amount, Currency, StartDate, EndDate, Description, CreatedAt
                   FROM Budget
                   WHERE accountbookid = @accountbookid";
            var parameters = new Dictionary<string, object>
    {
        { "@accountbookid", accountBookId }
    };
            DataTable dt = _dbHelper.ExecuteQuery(sql, parameters);
            List<BudgetList> result = new();
            foreach (DataRow row in dt.Rows)
            {
                result.Add(new BudgetList
                {
                    BudgetID = Convert.ToInt32(row["BudgetID"]),
                    AccountBookID = row.Table.Columns.Contains("accountbookid") ? Convert.ToInt32(row["accountbookid"]) : 0,
                    BudgetName = row.Table.Columns.Contains("BudgetName") ? row["BudgetName"].ToString() : null,
                    Amount = Convert.ToInt32(row["Amount"]),
                    Currency = row.Table.Columns.Contains("Currency") ? row["Currency"].ToString() : null,
                    StartDate = row.Table.Columns.Contains("StartDate") && !row.IsNull("StartDate") // 使用 row.IsNull() 更簡潔
            ? Convert.ToDateTime(row["StartDate"])
            : DateTime.MinValue, // 或者你選擇的其他預設日期，例如 DateTime.Today
                    EndDate = row.Table.Columns.Contains("EndDate") && !row.IsNull("EndDate")
            ? Convert.ToDateTime(row["EndDate"])
            : DateTime.MinValue, // 或者你選擇的其他預設日期

                    Description = row.Table.Columns.Contains("Description") ? row["EndDate"].ToString() : null,
                });
            }
            return result;
        }



        /// <summary>
        /// 預算新增
        /// </summary>
        /// <param name="budget"></param>
        public void InsertBudget(Budget budget)
        {
            string sql = @"INSERT INTO Budget  
                           (accountbookid, BudgetName, Amount, Currency, StartDate, EndDate, Description) 
                           VALUES (@accountbookid, @BudgetName, @Amount, @Currency, @StartDate, @EndDate, @Description)";

            var parameters = new Dictionary<string, object>
            {
                { "@accountbookid", budget.AccountBookID },
                { "@BudgetName", budget.BudgetName ?? "預算未命名"},
                { "@Amount", budget.Amount },
                { "@Currency", budget.Currency ?? "TWD"},
                { "@StartDate", budget.StartDate},
                { "@EndDate", budget.EndDate },
                { "@Description", budget.Description ?? (object)DBNull.Value}
            };

            _dbHelper.ExecuteNonQuery(sql, parameters);
        }
        /// <summary>
        /// 取得指定帳本中納入預算的交易，並根據分類與時間區間過濾
        /// </summary>
        public List<TransactionData> GetTransactionsIncludedInBudget(
            int accountBookId,
            string category = null,
            DateTime? startDate = null,
            DateTime? endDate = null)
        {
            string sql = @"
        SELECT TRANSACTION_ID, ACCOUNT_BOOK_ID, DATE, AMOUNT, 
               DESCRIPTION, TRANSACTION_CURRENCY, CATEGORY, INCLUDE_IN_BUDGET
        FROM TRANSACTION
        WHERE ACCOUNT_BOOK_ID = @AccountBookId
          AND INCLUDE_IN_BUDGET = TRUE";

            var parameters = new Dictionary<string, object>
    {
        { "@AccountBookId", accountBookId }
    };

            // 動態加入分類過濾
            if (!string.IsNullOrEmpty(category) && category != "全部")
            {
                sql += " AND CATEGORY = @Category";
                parameters.Add("@Category", category);
            }

            // 動態加入時間區間過濾
            if (startDate.HasValue)
            {
                sql += " AND DATE >= @StartDate";
                parameters.Add("@StartDate", startDate.Value);
            }
            if (endDate.HasValue)
            {
                sql += " AND DATE <= @EndDate";
                parameters.Add("@EndDate", endDate.Value);
            }

            DataTable dt = _dbHelper.ExecuteQuery(sql, parameters);
            List<TransactionData> result = new();

            foreach (DataRow row in dt.Rows)
            {
                result.Add(new TransactionData
                {
                    TransactionId = Convert.ToInt32(row["TRANSACTION_ID"]),
                    AccountBookId = Convert.ToInt32(row["ACCOUNT_BOOK_ID"]),
                    Date = Convert.ToDateTime(row["DATE"]),
                    Amount = Convert.ToInt32(row["AMOUNT"]),
                    Description = row["DESCRIPTION"].ToString(),
                    Currency = row["TRANSACTION_CURRENCY"].ToString(),
                    Category = row["CATEGORY"].ToString(),
                    IncludeInBudget = Convert.ToBoolean(row["INCLUDE_IN_BUDGET"])
                });
            }

            return result;
        }

        /// <summary>
        /// 根據 BudgetID 取得單筆預算資料
        /// </summary>
        /// <param name="budgetId">預算編號</param>
        /// <returns>Budget 物件，如果找不到則為 null</returns>
        public Budget GetBudgetById(int budgetId)
        {
            string sql = @"SELECT BudgetID, accountbookid, BudgetName, Amount, Currency, StartDate, EndDate, Description, CreatedAt 
                   FROM Budget 
                   WHERE BudgetID = @BudgetID"; // 假設你的 Budget 資料表有這些欄位

            var parameters = new Dictionary<string, object>
    {
        { "@BudgetID", budgetId }
    };

            DataTable dt = _dbHelper.ExecuteQuery(sql, parameters);

            if (dt.Rows.Count > 0)
            {
                DataRow row = dt.Rows[0];
                return new Budget
                {
                    BudgetID = Convert.ToInt32(row["BudgetID"]),
                    AccountBookID = Convert.ToInt32(row["accountbookid"]), // 假設 accountbookid 在 Budget 表中對應 AccountBookID
                    BudgetName = row["BudgetName"].ToString(),      // 假設 BudgetName 在 Budget 表中對應 AccountBookName
                    Amount = Convert.ToInt32(row["Amount"]),
                    Currency = row.Table.Columns.Contains("Currency") ? row["Currency"].ToString() : null,
                    Description = row.Table.Columns.Contains("Description") ? row["Description"].ToString() : null,
                    // 確保 StartDate 和 EndDate 的型別與 Budget 模型一致
                    StartDate = row.Table.Columns.Contains("StartDate") && !row.IsNull("StartDate")
                                ? Convert.ToDateTime(row["StartDate"])
                                : default(DateTime), // 或 DateTime?, 根據 Budget 模型
                    EndDate = row.Table.Columns.Contains("EndDate") && !row.IsNull("EndDate")
                                ? Convert.ToDateTime(row["EndDate"])
                                : default(DateTime)  // 或 DateTime?, 根據 Budget 模型
                                                     // 其他 Budget 模型的屬性...
                };
            }
            return null; // 找不到預算
        }
        /// <summary>
        /// 編輯預算紀錄
        /// </summary>
        /// <param name="data"></param>
        public void UpdateBudget(Budget arg)
        {
            string sql = @"UPDATE BUDGET SET 
                                BudgetName = @BudgetName,
                                AMOUNT = @AMOUNT,
                                CURRENCY = @CURRENCY,
                                STARTDATE = @STARTDATE,
                                ENDDATE = @ENDDATE,
                                DESCRIPTION = @DESCRIPTION
                           WHERE BUDGETID = @BUDGETID AND ACCOUNTBOOKID = @ACCOUNTBOOKID";

            var parameters = new Dictionary<string, object>
            {
                { "@BudgetName", arg.BudgetName },
                { "@AMOUNT", arg.Amount },
                { "@CURRENCY", arg.Currency },
                { "@STARTDATE", arg.StartDate },
                { "@ENDDATE", arg.EndDate },
                { "@DESCRIPTION", arg.Description },
            };

            _dbHelper.ExecuteNonQuery(sql, parameters);
        }

        public decimal GetIncludedExpenseSum(int accountBookId)
        {
            string sql = @"SELECT COALESCE(SUM(AMOUNT), 0)
                   FROM TRANSACTION
                   WHERE ACCOUNT_BOOK_ID = @AccountBookId
                     AND INCLUDE_IN_BUDGET = TRUE";

            var parameters = new Dictionary<string, object>
    {
        { "@AccountBookId", accountBookId }
    };

            object result = _dbHelper.ExecuteScalar(sql, parameters);

            // 安全地處理 null 值
            if (result == null || result == DBNull.Value)
            {
                return 0m;
            }

            // 使用 decimal.TryParse 更安全
            if (decimal.TryParse(result.ToString(), out decimal amount))
            {
                return amount;
            }

            return 0m;
        }
        public void DeleteBudget(int budgetId, int accountBookId)
        {
            string sql = @"DELETE FROM Budget WHERE BudgetID = @BudgetID AND accountbookid = @accountBookId";
            var parameters = new Dictionary<string, object>
    {
        { "@BudgetID", budgetId },
        { "@accountBookId", accountBookId }
    };
            _dbHelper.ExecuteNonQuery(sql, parameters);
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
