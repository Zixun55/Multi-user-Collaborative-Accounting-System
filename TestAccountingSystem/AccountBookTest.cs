using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Microsoft.Extensions.Configuration;
using project.Models.Services;
using project.Models;
using System.Collections.Generic;
using project.Models.Interfaces;
using System.Data;

namespace TestAccountingSystem
{
    [TestClass]
    public sealed class AccountBookTest
    {
        /// <summary>
        /// 取得帳本列表(當沒有帳本資料時)
        /// </summary>
        [TestMethod]
        public void GetAccountBookList_When_No_Data()
        {
            var mockDbHelper = new Mock<IDatabaseHelper>();

            mockDbHelper
                .Setup(db => db.ExecuteQuery(It.IsAny<string>(), It.IsAny<Dictionary<string, object>>()))
                .Returns(new DataTable());

            var service = new AccountBookService(mockDbHelper.Object);
            var input = new AccountBookList { UserId = "1" };
            var result = service.GetAccountBookList(input);

            Assert.IsNotNull(result);
            Assert.AreEqual(0, result.Count);
        }

        /// <summary>
        /// 取得帳本列表(當有帳本資料時)
        /// </summary>
        [TestMethod]
        public void GetAccountBookList_When_Has_Data()
        {
            var mockDbHelper = new Mock<IDatabaseHelper>();

            var dt = new DataTable();
            dt.Columns.Add("OWNER", typeof(int));
            dt.Columns.Add("ACCOUNT_BOOK_ID", typeof(int));
            dt.Columns.Add("ACCOUNT_BOOK_NAME", typeof(string));
            dt.Columns.Add("DESCRIPTION", typeof(string));

            dt.Rows.Add(1, 1, "TEST BOOK", "This is first account book");
            dt.Rows.Add(1, 2, "TEST BOOK 2", "This is second account book");

            mockDbHelper
                .Setup(db => db.ExecuteQuery(It.IsAny<string>(), It.IsAny<Dictionary<string, object>>()))
                .Returns(dt);

            var service = new AccountBookService(mockDbHelper.Object);
            var input = new AccountBookList { UserId = "1" };
            var result = service.GetAccountBookList(input);

            Assert.IsNotNull(result);
            Assert.AreEqual(2, result.Count);
            Assert.AreEqual("TEST BOOK", result[0].AccountBookName);
            Assert.AreEqual("This is first account book", result[0].Description);
            Assert.AreEqual("TEST BOOK 2", result[1].AccountBookName);
            Assert.AreEqual("This is second account book", result[1].Description);
        }

        /// <summary>
        /// 取得帳本資料(當帳本內沒有交易資料時)
        /// </summary>
        [TestMethod]
        public void GetAccountBookData_When_No_Data()
        {
            var mockDbHelper = new Mock<IDatabaseHelper>();

            mockDbHelper
                .Setup(db => db.ExecuteQuery(It.IsAny<string>(), It.IsAny<Dictionary<string, object>>()))
                .Returns(new DataTable());

            var service = new AccountBookService(mockDbHelper.Object);
            var input = new TransactionList { AccountBookId = 1 };
            var result = service.GetAccountBookData(input);

            Assert.IsNotNull(result);
            Assert.AreEqual(0, result.Count);
        }

        /// <summary>
        /// 取得帳本資料(當帳本內有交易資料時)
        /// </summary>
        [TestMethod]
        public void GetAccountBookData_When_Has_Data()
        {
            var mockDbHelper = new Mock<IDatabaseHelper>();

            var dt = new DataTable();
            dt.Columns.Add("TRANSACTION_ID", typeof(int));
            dt.Columns.Add("ACCOUNT_BOOK_ID", typeof(int));
            dt.Columns.Add("DATE", typeof(DateTime));
            dt.Columns.Add("DESCRIPTION", typeof(string));
            dt.Columns.Add("CATEGORY", typeof(string));
            dt.Columns.Add("AMOUNT", typeof(int));
            dt.Columns.Add("TRANSACTION_CURRENCY", typeof(string));

            dt.Rows.Add(1, 1, new DateTime(2024, 2, 13), "午餐", "餐飲", 100, "TWD");
            dt.Rows.Add(2, 1, new DateTime(2025, 3, 27), "晚餐", "餐飲", 200, "TWD");

            mockDbHelper
                .Setup(db => db.ExecuteQuery(It.IsAny<string>(), It.IsAny<Dictionary<string, object>>()))
                .Returns(dt);

            var service = new AccountBookService(mockDbHelper.Object);
            var input = new TransactionList { AccountBookId = 1 };
            var result = service.GetAccountBookData(input);

            Assert.IsNotNull(result);
            Assert.AreEqual(2, result.Count);
            Assert.AreEqual(1, result[0].TransactionId);
            Assert.AreEqual(1, result[0].AccountBookId);
            Assert.AreEqual(new DateTime(2024, 2, 13), result[0].Date);
            Assert.AreEqual("午餐", result[0].Description);
            Assert.AreEqual("餐飲", result[0].Category);
            Assert.AreEqual(100, result[0].Amount);
            Assert.AreEqual("TWD", result[0].Currency);
            Assert.AreEqual(2, result[1].TransactionId);
            Assert.AreEqual(1, result[1].AccountBookId);
            Assert.AreEqual(new DateTime(2025, 3, 27), result[1].Date);
            Assert.AreEqual("晚餐", result[1].Description);
            Assert.AreEqual("餐飲", result[1].Category);
            Assert.AreEqual(200, result[1].Amount);
            Assert.AreEqual("TWD", result[1].Currency);
        }

        /// <summary>
        /// 取得交易紀錄詳細資料
        /// </summary>
        [TestMethod]
        public void GetTransactionData_When_Has_Data()
        {
            var mockDbHelper = new Mock<IDatabaseHelper>();

            var dt = new DataTable();
            dt.Columns.Add("TRANSACTION_ID", typeof(int));
            dt.Columns.Add("ACCOUNT_BOOK_ID", typeof(int));
            dt.Columns.Add("DATE", typeof(DateTime));
            dt.Columns.Add("DESCRIPTION", typeof(string));
            dt.Columns.Add("CATEGORY", typeof(string));
            dt.Columns.Add("AMOUNT", typeof(int));
            dt.Columns.Add("TRANSACTION_CURRENCY", typeof(string));

            dt.Rows.Add(1, 1, new DateTime(2024, 2, 13), "午餐", "餐飲", 100, "TWD");

            mockDbHelper
                .Setup(db => db.ExecuteQuery(It.IsAny<string>(), It.IsAny<Dictionary<string, object>>()))
                .Returns(dt);

            var service = new AccountBookService(mockDbHelper.Object);
            var input = new TransactionData { TransactionId = 1 };
            var result = service.GetTransactionData(input);

            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.TransactionId);
            Assert.AreEqual(1, result.AccountBookId);
            Assert.AreEqual(new DateTime(2024, 2, 13), result.Date);
            Assert.AreEqual("午餐", result.Description);
            Assert.AreEqual("餐飲", result.Category);
            Assert.AreEqual(100, result.Amount);
            Assert.AreEqual("TWD", result.Currency);
        }

        /// <summary>
        /// 修改交易資料
        /// </summary>
        [TestMethod]
        public void UpdateTransactionData_Correct()
        {
            var mockDbHelper = new Mock<IDatabaseHelper>();
            var service = new AccountBookService(mockDbHelper.Object);

            var transaction = new TransactionData
            {
                TransactionId = 1,
                AccountBookId = 1,
                Date = new DateTime(2025, 4, 26),
                Amount = 150,
                Description = "午餐",
                Currency = "TWD",
                Category = "餐飲"
            };

            service.UpdateTransactionData(transaction);

            mockDbHelper.Verify(db => db.ExecuteNonQuery(
                It.Is<string>(sql => sql.Contains("UPDATE TRANSACTION SET")),
                It.Is<Dictionary<string, object>>(param =>
                    (DateTime)param["@DATE"] == transaction.Date &&
                    (int)param["@AMOUNT"] == transaction.Amount &&
                    (string)param["@DESCRIPTION"] == transaction.Description &&
                    (string)param["@CURRENCY"] == transaction.Currency &&
                    (string)param["@CATEGORY"] == transaction.Category &&
                    (int)param["@TRANSACTION_ID"] == transaction.TransactionId &&
                    (int)param["@ACCOUNT_BOOK_ID"] == transaction.AccountBookId
                )
            ), Times.Once);
        }

        /// <summary>
        /// 新增交易資料
        /// </summary>
        [TestMethod]
        public void InsertTransactionData_Correct()
        {
            var mockDbHelper = new Mock<IDatabaseHelper>();
            var service = new AccountBookService(mockDbHelper.Object);

            var transaction = new TransactionData
            {
                AccountBookId = 1,
                Date = new DateTime(2025, 4, 26),
                Amount = 20,
                Description = "搭捷運",
                Currency = "TWD",
                Category = "交通"
            };

            service.InsertTransactionData(transaction);

            mockDbHelper.Verify(m => m.ExecuteNonQuery(
                It.Is<string>(sql => sql.Contains("INSERT INTO TRANSACTION")),
                It.Is<Dictionary<string, object>>(p =>
                    (int)p["@ACCOUNT_BOOK_ID"] == transaction.AccountBookId &&
                    (DateTime)p["@DATE"] == transaction.Date &&
                    (int)p["@AMOUNT"] == transaction.Amount &&
                    (string)p["@DESCRIPTION"] == transaction.Description &&
                    (string)p["@CURRENCY"] == transaction.Currency &&
                    (string)p["@CATEGORY"] == transaction.Category
                )
            ), Times.Once);
        }

        /// <summary>
        /// 刪除交易資料
        /// </summary>
        [TestMethod]
        public void DeleteTransactionData_Correct()
        {
            var mockDbHelper = new Mock<IDatabaseHelper>();
            var service = new AccountBookService(mockDbHelper.Object);

            int transactionId = 1;
            int accountBookId = 1;

            service.DeleteTransactionData(transactionId, accountBookId);

            mockDbHelper.Verify(m => m.ExecuteNonQuery(
                It.Is<string>(sql => sql.Contains("DELETE FROM TRANSACTION")),
                It.Is<Dictionary<string, object>>(p =>
                    (int)p["@TRANSACTION_ID"] == transactionId
                )
            ), Times.Once);
        }

        /// <summary>
        /// 新增帳本(當輸入的資料正確時)
        /// </summary>
        [TestMethod]
        public void InsertAccountBook_Correct()
        {
            var mockDbHelper = new Mock<IDatabaseHelper>();
            var service = new AccountBookService(mockDbHelper.Object);

            var testData = new AccountBookData
            {
                AccountBookName = "Test Book",
                Description = "insert a new account book",
                BaseCurrency = "TWD",
                UserId = "123"
            };

            service.InsertAccountBook(testData);

            mockDbHelper.Verify(m => m.ExecuteNonQuery(
                It.Is<string>(sql => sql.Contains("INSERT INTO ACCOUNT_BOOK")),
                It.Is<Dictionary<string, object>>(p =>
                    p["@ACCOUNT_BOOK_NAME"].Equals(testData.AccountBookName) &&
                    p["@DESCRIPTION"].Equals(testData.Description) &&
                    p["@BASE_CURRENCY"].Equals(testData.BaseCurrency) &&
                    p["@OWNER"].Equals(testData.UserId)
                )
            ), Times.Once);
        }

        /// <summary>
        /// 刪除帳本(確認刪除帳本的transaction有兩個SQL)
        /// </summary>
        [TestMethod]
        public void DeleteAccountBook_Correct()
        {
            var mockDbHelper = new Mock<IDatabaseHelper>();
            var service = new AccountBookService(mockDbHelper.Object);

            int accountBookId = 1;

            service.DeleteAccountBookData(accountBookId);

            mockDbHelper.Verify(m => m.ExecuteTransaction(
                It.Is<List<(string, Dictionary<string, object>)>>(list =>
                    list.Count == 2 &&
                    list[0].Item1.Contains("DELETE FROM TRANSACTION") &&
                    list[1].Item1.Contains("DELETE FROM ACCOUNT_BOOK") &&
                    list[0].Item2 == list[1].Item2 &&
                    list[0].Item2["@ACCOUNT_BOOK_ID"].Equals(accountBookId)
                )
            ), Times.Once);
        }

        /// <summary>
        /// 搜尋帳本(找到資料)
        /// </summary>
        [TestMethod]
        public void SearchAccountBook_When_Has_Data()
        {
            var mockDbHelper = new Mock<IDatabaseHelper>();

            var dt = new DataTable();
            dt.Columns.Add("ACCOUNT_BOOK_ID", typeof(int));
            dt.Columns.Add("DESCRIPTION", typeof(string));
            dt.Columns.Add("ACCOUNT_BOOK_NAME", typeof(string));

            var row = dt.NewRow();
            row["ACCOUNT_BOOK_ID"] = 1;
            row["DESCRIPTION"] = "旅遊花費";
            row["ACCOUNT_BOOK_NAME"] = "旅遊";
            dt.Rows.Add(row);

            mockDbHelper
                .Setup(db => db.ExecuteQuery(It.IsAny<string>(), It.IsAny<Dictionary<string, object>>()))
                .Returns(dt);

            var service = new AccountBookService(mockDbHelper.Object);
            var result = service.SearchAccountBook(1);

            Assert.IsNotNull(result);
            Assert.AreEqual("旅遊花費", result.Description);
            Assert.AreEqual("旅遊", result.AccountBookName);
        }

        /// <summary>
        /// 搜尋帳本(查無資料)
        /// </summary>
        [TestMethod]
        public void SearchAccountBook_When_No_Data()
        {
            var mockDbHelper = new Mock<IDatabaseHelper>();

            var dt = new DataTable();
            dt.Columns.Add("ACCOUNT_BOOK_ID", typeof(int));
            dt.Columns.Add("DESCRIPTION", typeof(string));
            dt.Columns.Add("ACCOUNT_BOOK_NAME", typeof(string));

            mockDbHelper
                .Setup(db => db.ExecuteQuery(It.IsAny<string>(), It.IsAny<Dictionary<string, object>>()))
                .Returns(dt);

            var service = new AccountBookService(mockDbHelper.Object);
            var result = service.SearchAccountBook(1);

            Assert.IsNull(result);
        }
    }
}
