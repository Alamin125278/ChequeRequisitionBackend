using ChequeRequisiontService.Core.Dto.Ftp;
using ChequeRequisiontService.Core.Interfaces.Services.FtpServices;
using ChequeRequisiontService.Models.CRDB;
using ClosedXML.Excel;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.Identity.Client;
using OfficeOpenXml;
using System.Globalization;
using System.Numerics;

namespace ChequeRequisiontService.Infrastructure.Services.FtpServices;

public class ExcelParserService : IExcelParserService
{
    public async Task<IEnumerable<ChequeBookRequisition>> ParseAsync(Stream stream, FtpSetting ftp)
    {
        return await Task.Run(() =>
        {
            var requisitions = new List<ChequeBookRequisition>();

            using var workbook = new XLWorkbook(stream);
            var ws = workbook.Worksheet(1); // প্রথম worksheet

            for (int i = 2; i <= ws.LastRowUsed().RowNumber(); i++)
            {
                var row = ws.Row(i);

                string chequePrefix = row.Cell(5).GetString().Trim().ToUpper(); 
                string chequeType = "";
                if (chequePrefix == "A")
                {
                    chequeType = "Current";
                }
                else if (chequePrefix == "B")
                {
                    chequeType = "Savings";
                }
                else
                {
                    chequeType = "Payment Order";
                }
                var accountNoRaw = row.Cell(3)?.GetString()?.Trim() ?? "";
                var micrNo = accountNoRaw.Length >= 13? accountNoRaw.Substring(accountNoRaw.Length - 13, 13): accountNoRaw;
                var requisition = new ChequeBookRequisition
                {
                    BankName = ftp.BankName,
                    BranchName = row.Cell(12).GetString(),
                    RequestedBy = 1,
                    AccountNo = (long)BigInteger.Parse(row.Cell(3).GetString()),
                    RoutingNo = int.Parse(row.Cell(2).GetString()),
                    StartNo = int.Parse(row.Cell(7).GetString()),
                    EndNo = int.Parse(row.Cell(8).GetString()),
                    ChequeType = chequeType,
                    ChequePrefix = row.Cell(5).GetString(),
                    MicrNo = micrNo,
                    Series = row.Cell(6).GetString(),
                    AccountName = row.Cell(4).GetString(),
                    CusAddress = row.Cell(13).GetString(),
                    BookQty = int.Parse(row.Cell(10).GetString()),
                    TransactionCode = int.Parse(row.Cell(11).GetString()),
                    Leaves = int.Parse(row.Cell(9).GetString()),
                    CourierCode = 1,
                    ReceivingBranchName = row.Cell(19).GetString(),
                    RequestDate = DateOnly.ParseExact(row.Cell(14).GetString(), "dd-MM-yyyy", CultureInfo.InvariantCulture),
                    Serverity = 1,
                    Status = 3,
                    IsDeleted = false,
                    CreatedBy = 1,
                    CreatedAt = DateTime.UtcNow,
                };

                requisitions.Add(requisition);
            }

            return requisitions;
        });
    }
}
