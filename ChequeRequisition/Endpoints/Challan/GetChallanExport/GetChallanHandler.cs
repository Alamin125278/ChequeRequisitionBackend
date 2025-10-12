using BuildingBlocks.CQRS;
using ChequeRequisiontService.Core.Dto.Auth;
using ChequeRequisiontService.Core.Dto.Challan;
using ChequeRequisiontService.Core.Interfaces.Repositories;
using ChequeRequisiontService.DbContexts;

namespace ChequeRequisiontService.Endpoints.Challan.GetChallanExport;
public record GetChallanCommand(List<int> ChallanIds):ICommand<GetChallanResponse>;
public record GetChallanResponse(List<ChallanExportDto> Challans);

public class GetChallanHandler(IChallanRepo challanRepo,IRequisitonRepo requisitonRepo, AuthenticatedUserInfo authenticatedUserInfo): ICommandHandler<GetChallanCommand, GetChallanResponse>
{
    public async Task<GetChallanResponse> Handle(GetChallanCommand request, CancellationToken cancellationToken)
    {
        await using var transaction = await requisitonRepo.BeginTransactionAsync(cancellationToken);
        try
        {
            // Step 1: Get challan export data
            var data = await challanRepo.GetChallanExportDataAsync(request.ChallanIds, cancellationToken);

            // Step 2: If no data, return empty list
            if (data == null || data.Count == 0)
            {
                return new GetChallanResponse(new List<ChallanExportDto>());
            }

            // Step 3: Commit transaction and return data
            await transaction.CommitAsync(cancellationToken);
            return new GetChallanResponse(data);
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync(cancellationToken);
            throw new Exception("Failed to get challan data", ex);
        }
    }

}

