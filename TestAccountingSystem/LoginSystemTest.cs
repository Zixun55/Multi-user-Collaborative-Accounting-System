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
        public void GetUserIdCorrect()
        {
            var mockDbHelper = new Mock<IDatabaseHelper>();
            var service = new LoginSystemService(mockDbHelper.Object);

            var expectedUserId = 1;

            mockDbHelper
                .Setup(db => db.ExecuteScalar(It.IsAny<string>(), It.IsAny<Dictionary<string, object>>()))
                .Returns(expectedUserId);

            var result = service.GetUserId("test@test.com", "test123");

            Assert.IsNotNull(result);
            Assert.AreEqual(expectedUserId, result.Value);
        }
    }
}
