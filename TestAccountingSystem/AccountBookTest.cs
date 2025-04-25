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
        public void GetAccountBookListWhenNoData()
        {
            var mockDbHelper = new Mock<IDatabaseHelper>();

            mockDbHelper
                .Setup(db => db.ExecuteQuery(It.IsAny<string>(), It.IsAny<Dictionary<string, object>>()))
                .Returns(new DataTable());

            var service = new AccountBookService(mockDbHelper.Object);

            var input = new AccountBookList { UserId = 1 };

            var result = service.GetAccountBookList(input);

            Assert.IsNotNull(result);
            Assert.AreEqual(0, result.Count);
        }

        /// <summary>
        /// 取得帳本列表(當有帳本資料時)
        /// </summary>
        [TestMethod]
        public void GetAccountBookListWhenHasData()
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

            var input = new AccountBookList { UserId = 1 };

            var result = service.GetAccountBookList(input);

            Assert.IsNotNull(result);
            Assert.AreEqual(2, result.Count);
            Assert.AreEqual("TEST BOOK", result[0].AccountBookName);
            Assert.AreEqual("This is first account book", result[0].Description);
            Assert.AreEqual("TEST BOOK 2", result[1].AccountBookName);
            Assert.AreEqual("This is second account book", result[1].Description);
        }

        /// <summary>
        /// 新增帳本(當輸入的資料正確時)
        /// </summary>
        [TestMethod]
        public void InsertAccountBookCorrect()
        {
            var mockDbHelper = new Mock<IDatabaseHelper>();
            var service = new AccountBookService(mockDbHelper.Object);

            var testData = new AccountBookData
            {
                AccountBookName = "Test Book",
                Description = "insert a new account book",
                BaseCurrency = "TWD",
                UserId = 123
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
    }
}
