using ChequeRequisiontService.Core.Dto.Ftp;
using ChequeRequisiontService.Core.Interfaces.Repositories;
using ChequeRequisiontService.Core.Interfaces.Services.FtpServices;
using ChequeRequisiontService.Models.CRDB;
using ClosedXML.Excel;
using DocumentFormat.OpenXml.Office2016.Drawing.ChartDrawing;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.Identity.Client;
using OfficeOpenXml;
using System.Globalization;
using System.Numerics;

namespace ChequeRequisiontService.Infrastructure.Services.FtpServices;

public class ExcelParserService(IBranchRepo branchRepo,IBankRepo bankRepo) : IExcelParserService
{
    private readonly IBranchRepo _branchRepo = branchRepo;
    private readonly IBankRepo _bankRepo = bankRepo;
    public async Task<IEnumerable<ChequeBookRequisition>> ParseAsync(Stream stream, FtpSetting ftp)
    {
        return await Task.Run(async () =>
        {
            var requisitions = new List<ChequeBookRequisition>();

            using var workbook = new XLWorkbook(stream);
            var ws = workbook.Worksheet(1); // প্রথম worksheet
            var banks = await _bankRepo.GetByIdAsync(ftp.BankId);

            for (int i = 2; i <= ws.LastRowUsed().RowNumber(); i++)
            {
                if (ftp.BankId == 4)
                {
                    var row = ws.Row(i);
                    var RoutingNo = row.Cell(2).GetString();
                    var receivingBranchName = row.Cell(19).GetString();
                    int branchId = await _branchRepo.GetIdAsync(ftp.BankId, RoutingNo);
                    int receivingBranchId = await _branchRepo.GetIdAsync(ftp.BankId, RoutingNo);

                    string prefix = row.Cell(5).GetString().Trim().ToUpper();
                    string chequeType = "";
                    int Leaves = int.Parse(row.Cell(9).GetString());
                    string series = row.Cell(6).GetString();
                    string chequePrefix = "";
                    if (prefix == "A")
                    {
                        chequeType = "Current";
                        chequePrefix = "CD" + Leaves + series;
                    }
                    else if (prefix == "B")
                    {
                        chequeType = "Savings";
                        chequePrefix = "SB" + Leaves + series;
                    }
                    else
                    {
                        chequeType = "Payment Order";
                        chequePrefix = "PO" + Leaves + series;
                    }
                    var accountNoRaw = row.Cell(3)?.GetString()?.Trim() ?? "";
                    var micrNo = accountNoRaw.Length >= 13 ? accountNoRaw.Substring(accountNoRaw.Length - 13, 13) : accountNoRaw;
                    var requisition = new ChequeBookRequisition
                    {
                        BankId = ftp.BankId,
                        BranchId = branchId,
                        RequestedBy = 1,
                        AccountNo = row.Cell(3).GetString(),
                        RoutingNo = row.Cell(2).GetString(),
                        StartNo = row.Cell(7).GetString(),
                        EndNo = row.Cell(8).GetString(),
                        ChequeType = chequeType,
                        ChequePrefix = chequePrefix,
                        MicrNo = micrNo,
                        Series = series,
                        AccountName = row.Cell(4).GetString(),
                        CusAddress = row.Cell(13).GetString(),
                        BookQty = int.Parse(row.Cell(10).GetString()),
                        TransactionCode = int.Parse(row.Cell(11).GetString()),
                        Leaves = Leaves,
                        VendorId=banks?.VendorId ?? 0,
                        CourierCode = 1,
                        ReceivingBranchId = receivingBranchId,
                        RequestDate = DateOnly.ParseExact(row.Cell(14).GetString(), "dd-MM-yyyy", CultureInfo.InvariantCulture),
                        Serverity = 1,
                        Status = 3,
                        IsDeleted = false,
                        CreatedBy = 1,
                        CreatedAt = DateTime.UtcNow,
                    };

                    requisitions.Add(requisition);
                }
            }
              

            return requisitions;
        });
    }
}
