using BuildingBlocks.CQRS;
using ChequeRequisiontService.Core.Dto.Auth;
using ChequeRequisiontService.Core.Interfaces.Repositories;

namespace ChequeRequisiontService.Endpoints.Requisition.UpdateRequisitionSeverity;

public record UpdateRequisitionSeverityCommand(List<int> RequisitionIds, int Severity) : ICommand<UpdateRequisitionSeverityRes>;
public record UpdateRequisitionSeverityRes(bool IsUpdated);

public class UpdateRequisitionSeverityHandler(IRequisitonRepo requisitonRepo, AuthenticatedUserInfo authenticatedUserInfo)
    : ICommandHandler<UpdateRequisitionSeverityCommand, UpdateRequisitionSeverityRes>
{
    public async Task<UpdateRequisitionSeverityRes> Handle(UpdateRequisitionSeverityCommand request, CancellationToken cancellationToken)
    {
        var updatedCount = await requisitonRepo.UpdateRequisitionSeverityAsync(request.RequisitionIds, request.Severity, authenticatedUserInfo.Id, cancellationToken);
        if (updatedCount > 0)
        {
            return new UpdateRequisitionSeverityRes(true);
        }
        return new UpdateRequisitionSeverityRes(false);
    }
}
