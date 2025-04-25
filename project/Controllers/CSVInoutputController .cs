using Microsoft.AspNetCore.Mvc;
using project.Models.Services;
using System.Text;
using System.IO;
using project.Models;
using System.Collections.Generic;

namespace project.Controllers
{
    public class CSVInoutputController : Controller
    {

        private readonly AccountBookService _service;

        public CSVInoutputController(AccountBookService service)
        {
            _service = service;
        }

        // 匯出帳本為 CSV
        [HttpGet]
        public IActionResult ExportAccountBook(int accountBookId)
        {
            var searchArg1 = new AccountBookList {AccountBookId = accountBookId };
            AccountBookData accountBookDataResult1 = _service.SearchAccountBook(accountBookId);
            var searchArg2 = new TransactionList { AccountBookId = accountBookId };
            List<TransactionList> accountBookDataResult = _service.GetAccountBookData(searchArg2);

            // 使用MemoryStream和StreamWriter自動添加BOM
            using (var memoryStream = new MemoryStream())
            using (var streamWriter = new StreamWriter(memoryStream, new UTF8Encoding(true))) // 啟用BOM
            {
                streamWriter.WriteLine("帳本名稱,帳本描述");
                streamWriter.WriteLine($"{accountBookDataResult1.AccountBookName}," +$"{accountBookDataResult1.Description},");
                // 寫入CSV標題
                streamWriter.WriteLine("日期,項目,備註,金額,貨幣");

                foreach (TransactionList id in accountBookDataResult)
                {
                    var searchArg3 = new TransactionData { TransactionId = id.TransactionId };
                    TransactionData accountBookDataResult2 = _service.GetTransactionData(searchArg3);
                    streamWriter.WriteLine(
                        $"{accountBookDataResult2.Date:yyyy/M/d tt hh:mm:ss}," +
                        $"{accountBookDataResult2.Category}," +
                        $"{accountBookDataResult2.Description}," +
                        $"{accountBookDataResult2.Amount}," +
                        $"{accountBookDataResult2.Currency},"
                    );
                }

                streamWriter.Flush();
                var fileName = $"AccountBook_{accountBookId}.csv";
                return File(memoryStream.ToArray(), "text/csv; charset=utf-8", fileName);
            }
        }


        [HttpPost]
        public IActionResult ImportAccountBook(int userId, IFormFile csvFile)
        {
            if (csvFile == null || csvFile.Length == 0)
            {
                ModelState.AddModelError("", "請選擇要上傳的檔案");
                return View();
            }
            AccountBookData accountBookData = new AccountBookData();
            List<TransactionData> transactionDataList = new List<TransactionData>();

            using (var reader = new StreamReader(csvFile.OpenReadStream(), Encoding.UTF8))
            {
                string headerLine = reader.ReadLine(); // 讀取標題行
                string secondLine = reader.ReadLine(); // 讀取標題行
                string[] accountbookdata = secondLine.Split(',');
                accountBookData.AccountBookName = accountbookdata[0];
                accountBookData.Description = accountbookdata[1];
                accountBookData.BaseCurrency = "TWD";
                accountBookData.UserId = userId;
                string thirdLine = reader.ReadLine(); // 讀取標題行
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    string[] values = line.Split(',');

                    // 檢查欄位數
                    if (values.Length <= 7)
                    {
                        TransactionData data = new TransactionData
                        {
                            // TransactionId 不需指定，資料庫自動產生
                            Date = DateTime.Parse(values[0]),
                            Category = values[1],
                            Description = values[2],
                            Amount = int.Parse(values[3]),
                            Currency = values[4]
                        };
                        transactionDataList.Add(data);
                    }
                }
            }

            // 1. 新增帳本（名稱與描述可讓使用者輸入，這裡假設預設值）
            _service.InsertAccountBook(accountBookData);
            AccountBookList accountBookList = new AccountBookList();
            accountBookList.UserId = userId;
            List<AccountBookList> transactionDatas = _service.GetAccountBookList(accountBookList);
            int abId = transactionDatas[transactionDatas.Count() - 1].AccountBookId;
            // 2. 新增所有交易紀錄
            foreach (var transaction in transactionDataList)
            {
                transaction.AccountBookId = abId;
                _service.InsertTransactionData(transaction);
            }

            TempData["SuccessMessage"] = "帳本與交易紀錄已成功匯入";
            return RedirectToAction("AccountBookList", "AccountingSystem");
        }


    }

}
