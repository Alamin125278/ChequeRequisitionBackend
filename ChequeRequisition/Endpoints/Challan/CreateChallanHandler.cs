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

public record CreateChallanCommand(Dictionary<string, List<ChallanOrderDto>> ChallanData) :ICommand<CreateChallanRes>;
public record CreateChallanRes(bool IsCreated, List<int> CreatedChallanIds);
public class CreateChallanHandler(IBankRepo bankRepo,IRequisitonRepo requisitonRepo,IChallanRepo challanRepo,AuthenticatedUserInfo authenticatedUserInfo) : ICommandHandler<CreateChallanCommand, CreateChallanRes>
{
    private readonly IChallanRepo _challanRepo = challanRepo;
    private readonly IBankRepo _bankRepo = bankRepo;
    private readonly AuthenticatedUserInfo _authenticatedUserInfo = authenticatedUserInfo;
    private async Task<string> GenerateChallanNumber(string branch, int bankId)
    {
        var bank =await _bankRepo.GetByIdAsync(bankId);
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

