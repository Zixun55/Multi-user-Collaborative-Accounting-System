using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Mvc;
using project.Models;
using project.Models.Services;

namespace project.Controllers
{
    public class AccountingSystemController : Controller
    {
        private readonly AccountBookService _service;

        public AccountingSystemController(AccountBookService service)
        {
            _service = service;
        }

        /// <summary>
        /// 獲得帳本列表
        /// </summary>
        /// <returns></returns>
        public ActionResult AccountBookList()
        {
            int? userId = HttpContext.Session.GetInt32("UserId");

            if (userId == null)
            {
                return RedirectToAction("Login", "LoginSystem");
            }

            ViewBag.UserId = userId;

            var searchArg = new AccountBookList { UserId = userId.Value };
            List<AccountBookList> listResult = _service.GetAccountBookList(searchArg);

            return View(listResult);
        }

        /// <summary>
        /// 獲得帳本資料
        /// </summary>
        /// <returns></returns>
        public ActionResult AccountBookData(int id)
        {
            var searchArg = new TransactionList { AccountBookId = id };
            List<TransactionList> accountBookDataResult = _service.GetAccountBookData(searchArg);
            ViewBag.AccountBookId = id;

            return View(accountBookDataResult);
        }

        /// <summary>
        /// 獲得交易資料(交易編輯頁面)
        /// </summary>
        /// <returns></returns>
        public ActionResult TransactionUpdate(int id)
        {
            var searchArg = new TransactionData { TransactionId = id };
            TransactionData accountBookDataResult = _service.GetTransactionData(searchArg);

            return View(accountBookDataResult);
        }

        /// <summary>
        /// 儲存修改的交易紀錄
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult TransactionUpdateSave(TransactionData data)
        {
            if (ModelState.IsValid)
            {
                _service.UpdateTransactionData(data);
                return RedirectToAction("AccountBookData", "AccountingSystem", new { id = data.AccountBookId });
            }
            return View("TransactionUpdate", data);
        }

        /// <summary>
        /// 新增交易頁面
        /// </summary>
        /// <param name="accountBookId"></param>
        /// <returns></returns>
        public ActionResult TransactionInsert(int accountBookId)
        {
            var model = new TransactionData();
            model.AccountBookId = accountBookId;
            model.Date = DateTime.Now;
            return View(model);
        }

        /// <summary>
        /// 儲存新增的交易紀錄
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult TransactionInsertSave(TransactionData data)
        {
            if (ModelState.IsValid)
            {
                _service.InsertTransactionData(data);
                return RedirectToAction("AccountBookData", "AccountingSystem", new { id = data.AccountBookId });
            }
            return View("TransactionInsert", data);
        }

        /// <summary>
        /// 刪除交易紀錄
        /// </summary>
        /// <param name="transactionId"></param>
        /// <param name="accountBookId"></param>
        /// <returns></returns>
        public IActionResult TransactionDelete(int transactionId, int accountBookId)
        {
            _service.DeleteTransactionData(transactionId, accountBookId);

            TempData["DeleteSuccess"] = "刪除成功";
            return RedirectToAction("AccountBookData", new { id = accountBookId });
        }

        /// <summary>
        /// 顯示新增帳本頁面
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public ActionResult AccountBookInsert(int userId)
        {
            var model = new AccountBookData();
            model.UserId = userId;
            return View(model);
        }

        /// <summary>
        /// 儲存新增的帳本
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AccountBookInsertSave(AccountBookData data)
        {
            if (ModelState.IsValid)
            {
                _service.InsertAccountBook(data);
                return RedirectToAction("AccountBookList", "AccountingSystem", new { id = data.UserId });
            }
            return View("AccountBookInsert", data);
        }

        /// <summary>
        /// 刪除帳本
        /// </summary>
        /// <param name="accountBookId"></param>
        /// <returns></returns>
        public IActionResult AccountBookDelete(int accountBookId)
        {
            try
            {
                _service.DeleteAccountBookData(accountBookId);

                TempData["DeleteAccountBookSuccess"] = "刪除帳本成功";
            }
            catch (Exception ex)
            {
                Console.WriteLine("刪除帳本錯誤：" + ex.Message);

                TempData["DeleteAccountBookError"] = "刪除帳本失敗，請稍後再試";
            }
            return RedirectToAction("AccountBookList");
        }
    }
}
