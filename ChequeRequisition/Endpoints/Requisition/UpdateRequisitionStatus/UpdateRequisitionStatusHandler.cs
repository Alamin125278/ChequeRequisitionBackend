using BuildingBlocks.CQRS;
using ChequeRequisiontService.Core.Dto.Auth;
using ChequeRequisiontService.Core.Interfaces.Repositories;
using DocumentFormat.OpenXml.Spreadsheet;
using System.Data.Common;

namespace ChequeRequisiontService.Endpoints.Requisition.UpdateRequisitionStatus;
public record UpdateRequisitionStatusCommand(List<int> RequisitionIds, int Status) : ICommand<UpdateRequisitionStatusRes>;
public record UpdateRequisitionStatusRes(bool IsUpdated);

public class UpdateRequisitionStatusHandler(IRequisitonRepo requisitonRepo, AuthenticatedUserInfo authenticatedUserInfo)
    : ICommandHandler<UpdateRequisitionStatusCommand, UpdateRequisitionStatusRes>
{
    public async Task<UpdateRequisitionStatusRes> Handle(UpdateRequisitionStatusCommand request, CancellationToken cancellationToken)
    {
       
            var updatedCount = await requisitonRepo.UpdateChequeListAsync(request.RequisitionIds, request.Status, authenticatedUserInfo.Id, cancellationToken);
            if (updatedCount > 0)
            {
                return new UpdateRequisitionStatusRes(true);
            }
        return new UpdateRequisitionStatusRes(false);
    }
}
