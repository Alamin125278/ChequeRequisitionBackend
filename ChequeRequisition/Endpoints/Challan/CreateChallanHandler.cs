using BuildingBlocks.CQRS;
using ChequeRequisiontService.Core.Dto.Auth;
using ChequeRequisiontService.Core.Dto.Challan;
using ChequeRequisiontService.Core.Interfaces.Repositories;
using ChequeRequisiontService.Infrastructure.Repositories.UserRepo;
using System.Windows.Input;

namespace ChequeRequisiontService.Endpoints.Challan;

public record CreateChallanCommand(Dictionary<string, List<ChallanOrderDto>> ChallanData) :ICommand<CreateChallanRes>;
public record CreateChallanRes(bool IsCreated, List<int> CreatedChallanIds);
public class CreateChallanHandler(IChallanRepo challanRepo,AuthenticatedUserInfo authenticatedUserInfo) : ICommandHandler<CreateChallanCommand, CreateChallanRes>
{
    private readonly IChallanRepo _challanRepo = challanRepo;
    private readonly AuthenticatedUserInfo _authenticatedUserInfo = authenticatedUserInfo;
    private static string GenerateChallanNumber(string branch)
    {
        var initials = new string(branch
            .Split(' ')
            .Select(word => word[0])
            .ToArray())
            .ToUpper();

        var randomPart = new Random().Next(100000, 999999);
        return $"CH-{initials}-{randomPart}";
    }
    public async Task<CreateChallanRes> Handle(CreateChallanCommand request, CancellationToken cancellationToken)
    {
        using var transaction = await _challanRepo.BeginTransactionAsync(cancellationToken);
        var createdIds = new List<int>();

        if (request.ChallanData == null || request.ChallanData.Count == 0)
        {
            return new CreateChallanRes(false, []);
        }
        try
        {
            foreach (var branchEntry in request.ChallanData)
            {
                var branchName = branchEntry.Key;
                var orders = branchEntry.Value;

                if (!orders.Any()) continue;

                var challanNumber = GenerateChallanNumber(branchName);
                var date = DateOnly.FromDateTime(DateTime.Today); 
                var receivingBranchId = orders.First().ReceivingBranchId;

                var challan = new ChallanDto
                {
                    ChallanNumber = challanNumber,
                    ChallanDate = date,
                    ReceivingBranch = receivingBranchId
                };

                var createdChallan = await _challanRepo.AddChallanAsync(challan,_authenticatedUserInfo.Id, cancellationToken);
                createdIds.Add(createdChallan);
                foreach (var order in orders)
                {
                    var challanRequisition = new ChallanTrackingDto
                    {
                        ChallanId = createdChallan,
                        RequisitionItemId = order.Id
                    };

                    await _challanRepo.AddChallanRequisitionAsync(challanRequisition,_authenticatedUserInfo.Id, cancellationToken);
                }
            }

            await transaction.CommitAsync(cancellationToken); 
            return new CreateChallanRes(true, createdIds);
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync(cancellationToken);
            throw new Exception("Database update error: " +  ex);
        }
    }
}

