using BuildingBlocks.CQRS;
using ChequeRequisiontService.Core.Dto.Auth;
using ChequeRequisiontService.Core.Dto.Challan;
using ChequeRequisiontService.Core.Interfaces.Repositories;
using ChequeRequisiontService.Infrastructure.Repositories.RequisitionRepo;
using ChequeRequisiontService.Infrastructure.Repositories.UserRepo;
using ChequeRequisiontService.Models.CRDB;
using DocumentFormat.OpenXml.Spreadsheet;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ChequeRequisiontService.Endpoints.Challan;

public record CreateChallanCommand(Dictionary<string, List<ChallanOrderDto>> ChallanData) : ICommand<CreateChallanRes>;
public record CreateChallanRes(bool IsCreated, List<int> CreatedChallanIds);
public class CreateChallanHandler(IBankRepo bankRepo, IRequisitonRepo requisitonRepo, IChallanRepo challanRepo, AuthenticatedUserInfo authenticatedUserInfo) : ICommandHandler<CreateChallanCommand, CreateChallanRes>
{
    private readonly IChallanRepo _challanRepo = challanRepo;
    private readonly IBankRepo _bankRepo = bankRepo;
    private readonly AuthenticatedUserInfo _authenticatedUserInfo = authenticatedUserInfo;
    private async Task<string> GenerateChallanNumber(string branch, int bankId)
    {
        var bank = await _bankRepo.GetByIdAsync(bankId);
        var bankName = bank != null ? bank.BankName : "UNKNOWN";
        var initials = string.Concat(bankName.Split(' ', StringSplitOptions.RemoveEmptyEntries)
             .Select(word => char.ToUpperInvariant(word[0])));

        // সর্বশেষ চালান নম্বর সংগ্রহ
        int lastChallanNo = await _challanRepo.GetChallanNumber(bankId);

        // নতুন চালান নম্বর তৈরি
        int newChallanNo = lastChallanNo > 0 ? lastChallanNo + 1 : 100001;

        // চালান নম্বর রিটার্ন
        return $"CH-{initials}-{newChallanNo}";
    }
    public async Task<CreateChallanRes> Handle(CreateChallanCommand request, CancellationToken cancellationToken)
    {
        using var transaction = await _challanRepo.BeginTransactionAsync(cancellationToken);
        var createdIds = new List<int>();

        if (request.ChallanData == null || request.ChallanData.Count == 0)
        {
            return new CreateChallanRes(false, new List<int>());
        }

        try
        {
            foreach (var branchEntry in request.ChallanData)
            {
                var branchName = branchEntry.Key;
                var orders = branchEntry.Value;

                if (!orders.Any())
                    continue;

                var firstOrder = orders.First();
                var bankId = firstOrder.BankId;
                var receivingBranchId = firstOrder.ReceivingBranchId;

                var challanNumber = await GenerateChallanNumber(branchName, bankId);
                var challanDate = DateOnly.FromDateTime(DateTime.Today);

                var challan = new ChallanDto
                {
                    ChallanNumber = challanNumber,
                    ChallanDate = challanDate,
                    ReceivingBranch = receivingBranchId
                };

                var createdChallan = await _challanRepo.AddChallanAsync(challan, _authenticatedUserInfo.Id, cancellationToken);
                createdIds.Add(createdChallan);

                var itemIds = new List<int>();

                foreach (var order in orders)
                {
                    var challanRequisition = new ChallanTrackingDto
                    {
                        ChallanId = createdChallan,
                        RequisitionItemId = order.Id
                    };

                    itemIds.Add(order.Id);
                    await _challanRepo.AddChallanRequisitionAsync(challanRequisition, _authenticatedUserInfo.Id, cancellationToken);
                }

                await requisitonRepo.UpdateChequeListAsync(itemIds, 4, _authenticatedUserInfo.Id, cancellationToken);
            }

            await transaction.CommitAsync(cancellationToken);
            return new CreateChallanRes(true, createdIds);
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync(cancellationToken);
            throw new Exception("Database update error", ex);
        }
    }
}

//using BuildingBlocks.CQRS;
//using ChequeRequisiontService.Core.Dto.Auth;
//using ChequeRequisiontService.Core.Dto.Challan;
//using ChequeRequisiontService.Core.Interfaces.Repositories;
//using ChequeRequisiontService.Models.CRDB;
//using System.Collections.Generic;
//using System.Linq;
//using System.Threading;
//using System.Threading.Tasks;

//namespace ChequeRequisiontService.Endpoints.Challan;

//public record CreateChallanCommand(Dictionary<string, List<ChallanOrderDto>> ChallanData) : ICommand<CreateChallanRes>;
//public record CreateChallanRes(bool IsCreated, List<int> CreatedChallanIds);

//public class CreateChallanHandler : ICommandHandler<CreateChallanCommand, CreateChallanRes>
//{
//    private readonly IChallanRepo _challanRepo;
//    private readonly IBankRepo _bankRepo;
//    private readonly IRequisitonRepo _requisitionRepo;
//    private readonly AuthenticatedUserInfo _authenticatedUserInfo;

//    public CreateChallanHandler(
//        IBankRepo bankRepo,
//        IRequisitonRepo requisitionRepo,
//        IChallanRepo challanRepo,
//        AuthenticatedUserInfo authenticatedUserInfo)
//    {
//        _challanRepo = challanRepo;
//        _bankRepo = bankRepo;
//        _requisitionRepo = requisitionRepo;
//        _authenticatedUserInfo = authenticatedUserInfo;
//    }

//    public async Task<CreateChallanRes> Handle(CreateChallanCommand request, CancellationToken cancellationToken)
//    {
//        if (request.ChallanData == null || request.ChallanData.Count == 0)
//            return new CreateChallanRes(false, new List<int>());

//        using var transaction = await _challanRepo.BeginTransactionAsync(cancellationToken);
//        var createdChallanIds = new List<int>();

//        try
//        {
//            foreach (var (branchName, orders) in request.ChallanData)
//            {
//                if (!orders.Any()) continue;

//                var createdChallanId = await ProcessBranchOrders(branchName, orders, cancellationToken);
//                createdChallanIds.Add(createdChallanId);
//            }

//            await transaction.CommitAsync(cancellationToken);
//            return new CreateChallanRes(true, createdChallanIds);
//        }
//        catch (Exception ex)
//        {
//            await transaction.RollbackAsync(cancellationToken);
//            throw new Exception("Database update error", ex);
//        }
//    }

//    private async Task<int> ProcessBranchOrders(string branchName, List<ChallanOrderDto> orders, CancellationToken cancellationToken)
//    {
//        var firstOrder = orders.First();
//        var challanNumber = await GenerateChallanNumber(branchName, firstOrder.BankId);

//        var challan = new ChallanDto
//        {
//            ChallanNumber = challanNumber,
//            ChallanDate = DateOnly.FromDateTime(DateTime.Today),
//            ReceivingBranch = firstOrder.ReceivingBranchId
//        };

//        var createdChallanId = await _challanRepo.AddChallanAsync(challan, _authenticatedUserInfo.Id, cancellationToken);

//        var requisitionTasks = orders.Select(order => _challanRepo.AddChallanRequisitionAsync(
//            new ChallanTrackingDto
//            {
//                ChallanId = createdChallanId,
//                RequisitionItemId = order.Id
//            },
//            _authenticatedUserInfo.Id,
//            cancellationToken
//        ));

//        await Task.WhenAll(requisitionTasks);

//        var itemIds = orders.Select(o => o.Id).ToList();
//        await _requisitionRepo.UpdateChequeListAsync(itemIds, 4, _authenticatedUserInfo.Id, cancellationToken);

//        return createdChallanId;
//    }

//    private async Task<string> GenerateChallanNumber(string branchName, int bankId)
//    {
//        var bank = await _bankRepo.GetByIdAsync(bankId);
//        var bankInitials = GetBankInitials(bank?.BankName);

//        int lastChallanNo = await _challanRepo.GetChallanNumber(bankId);
//        int newChallanNo = lastChallanNo > 0 ? lastChallanNo + 1 : 100001;

//        return $"CH-{bankInitials}-{newChallanNo}";
//    }

//    private static string GetBankInitials(string? bankName)
//    {
//        if (string.IsNullOrWhiteSpace(bankName)) return "UNKNOWN";

//        return string.Concat(bankName.Split(' ', StringSplitOptions.RemoveEmptyEntries)
//            .Select(word => char.ToUpperInvariant(word[0])));
//    }
//}