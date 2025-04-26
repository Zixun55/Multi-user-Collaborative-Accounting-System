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
    public sealed class LoginSystemTest
    {
        /// <summary>
        /// 取得使用者編號(當帳號密碼正確時)
        /// </summary>
        [TestMethod]
        public void GetUserId_Correct()
        {
            var mockDbHelper = new Mock<IDatabaseHelper>();
            var service = new LoginSystemService(mockDbHelper.Object);

            var userId = 1;

            mockDbHelper
                .Setup(db => db.ExecuteScalar(It.IsAny<string>(), It.IsAny<Dictionary<string, object>>()))
                .Returns(userId);

            var result = service.GetUserId("test@test.com", "test123");

            Assert.IsNotNull(result);
            Assert.AreEqual(userId, result.Value);
        }

        /// <summary>
        /// 取得使用者編號(當帳號密碼錯誤時)
        /// </summary>
        [TestMethod]
        public void GetUserId_Incorrect()
        {
            var mockDbHelper = new Mock<IDatabaseHelper>();
            var service = new LoginSystemService(mockDbHelper.Object);

            mockDbHelper
                .Setup(db => db.ExecuteScalar(It.IsAny<string>(), It.IsAny<Dictionary<string, object>>()))
                .Returns((object)null);

            var result = service.GetUserId("wrong@test.com", "wrong123");

            Assert.IsNull(result);
        }

        /// <summary>
        /// 檢查Email(有找到重複的Email)
        /// </summary>
        [TestMethod]
        public void CheckEmail_When_Email_Exists()
        {
            var mockDbHelper = new Mock<IDatabaseHelper>();
            var service = new LoginSystemService(mockDbHelper.Object);

            mockDbHelper
                .Setup(db => db.ExecuteScalar(It.IsAny<string>(), It.IsAny<Dictionary<string, object>>()))
                .Returns(1);

            var result = service.CheckEmail("test@test.com");

            Assert.IsTrue(result);
        }

        /// <summary>
        /// 檢查Email(沒有找到重複的Email)
        /// </summary>
        [TestMethod]
        public void CheckEmail_When_Email_Not_Exists()
        {
            var mockDbHelper = new Mock<IDatabaseHelper>();
            var service = new LoginSystemService(mockDbHelper.Object);

            mockDbHelper
                .Setup(db => db.ExecuteScalar(It.IsAny<string>(), It.IsAny<Dictionary<string, object>>()))
                .Returns(0);

            var result = service.CheckEmail("test@test.com");

            Assert.IsFalse(result);
        }

        /// <summary>
        /// 建立新使用者(註冊)
        /// </summary>
        [TestMethod]
        public void CreateNewUser_Correct()
        {
            var mockDbHelper = new Mock<IDatabaseHelper>();
            var service = new LoginSystemService(mockDbHelper.Object);

            var userData = new UserData
            {
                UserName = "TestUser",
                Email = "testuser@test.com",
                Password = "password123"
            };

            service.CreateNewUser(userData);

            mockDbHelper.Verify(m => m.ExecuteNonQuery(
                It.Is<string>(sql => sql.Contains("INSERT INTO USERS")),
                It.Is<Dictionary<string, object>>(p =>
                    p["@USER_NAME"].Equals(userData.UserName) &&
                    p["@EMAIL"].Equals(userData.Email) &&
                    p["@PASSWORDS"].Equals(userData.Password)
                )
            ), Times.Once);
        }
    }
}
