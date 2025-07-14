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

            // Step 2: Extract all item IDs from the challan
            var itemIds = data.SelectMany(d => d.Items)
                              .Select(item => item.ItemId)
                              .ToList();

            // Step 3: If there are no item IDs, return empty response
            if (itemIds.Count == 0)
            {
                return new GetChallanResponse([]);
            }

            // Step 4: Update all requisitions at once
            var updatedCount = await requisitonRepo.UpdateChequeListAsync(itemIds,4, authenticatedUserInfo.Id, cancellationToken);

            // Step 5: If update count matches item count, commit and return data
            if (updatedCount == itemIds.Count)
            {
                await transaction.CommitAsync(cancellationToken);
                return new GetChallanResponse(data);
            }

            // Step 6: Mismatch in count? Rollback and return empty
            await transaction.RollbackAsync(cancellationToken);
            return new GetChallanResponse([]);
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync(cancellationToken);
            // Optional: log the error
            throw new Exception("Failed to get and update challan data", ex);
        }
    }
}

