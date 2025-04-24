using Microsoft.AspNetCore.Mvc;
using project.Models.Services;
using System.Text;
using System.IO;
using project.Models;

namespace project.Controllers
{
    public class CSVInoutputController : Controller
    {
        private readonly IConfiguration _configuration;
        public CSVInoutputController(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        // 匯出帳本為 CSV
        [HttpGet]
        public IActionResult ExportAccountBook(int accountBookId)
        {
            AccountBookService service = new AccountBookService(_configuration);
            var searchArg = new TransactionList { AccountBookId = accountBookId };
            List<TransactionList> accountBookDataResult = service.GetAccountBookData(searchArg);

            // 將帳本資料轉成 CSV 字串
            var csvBuilder = new StringBuilder();
            csvBuilder.AppendLine("日期,項目,金額,備註");
            foreach (TransactionList id in accountBookDataResult)
            {
                var searchArg2 = new TransactionData { TransactionId = id.TransactionId };
                TransactionData accountBookDataResult2 = service.GetTransactionData(searchArg2);
                csvBuilder.AppendLine($"{accountBookDataResult2.TransactionId},{accountBookDataResult2.Date:yyyy-MM-dd},{accountBookDataResult2.Category},{accountBookDataResult2.Description},{accountBookDataResult2.Amount},{accountBookDataResult2.Currency},{accountBookDataResult2.AccountBookId}");
            }
            var bytes = Encoding.UTF8.GetBytes(csvBuilder.ToString());
            var fileName = $"AccountBook_{accountBookId}.csv";

            // 回傳檔案下載
            return File(bytes, "text/csv", fileName);
        }

        // POST: 處理匯入的 CSV 檔案
        //[HttpPost]
        //public IActionResult ImportAccountBook(IFormFile csvFile)
        //{
        //    if (csvFile == null || csvFile.Length == 0)
        //    {
        //        ModelState.AddModelError("", "請選擇要上傳的檔案");
        //        return View();
        //    }

        //    // 讀取 CSV 檔案內容
        //    List<TransactionData> transactionDataList = new List<TransactionData>();
        //    using (var reader = new StreamReader(csvFile.OpenReadStream(), Encoding.UTF8))
        //    {
        //        string headerLine = reader.ReadLine(); // 讀取標題行
        //        string line;
        //        while ((line = reader.ReadLine()) != null)
        //        {
        //            string[] values = line.Split(',');
        //            // 假設 CSV 格式為：TransactionId,Date,Category,Description,Amount,Currency,AccountBookId
        //            if (values.Length >= 7)
        //            {
        //                TransactionData data = new TransactionData
        //                {
        //                    TransactionId = int.Parse(values[0]),
        //                    Date = DateTime.Parse(values[1]),
        //                    Category = values[2],
        //                    Description = values[3],
        //                    Amount = int.Parse(values[4]),
        //                    Currency = values[5],
        //                    AccountBookId = int.Parse(values[6])
        //                };
        //                transactionDataList.Add(data);
        //            }
        //        }
        //    }

        // 使用 service 匯入資料
        //AccountBookService service = new AccountBookService(_configuration);
        //bool result = service.ImportTransactionData(transactionDataList);

        //if (result)
        //{
        //    TempData["SuccessMessage"] = "帳本資料已成功匯入";
        //    return RedirectToAction("AccountBookList", "AccountingSystem");
        //}
        //else
        //{
        //    ModelState.AddModelError("", "匯入失敗，請檢查檔案格式是否正確");
        //    return View();
        //}
        //}

    }

}
