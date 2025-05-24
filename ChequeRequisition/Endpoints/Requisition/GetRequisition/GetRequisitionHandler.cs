using BuildingBlocks.CQRS;
using ChequeRequisiontService.Core.Dto.Requisition;
using ChequeRequisiontService.Core.Interfaces.Repositories;

namespace ChequeRequisiontService.Endpoints.Requisition.GetRequisition
{
    public record GetRequisitionQuery(int Id) : IQuery<GetRequisitionResult>;
    public record GetRequisitionResult(RequisitionDto? RequisitionDto);
    public class GetRequisitionHandler(IRequisitonRepo requisitionRepo) : IQueryHandler<GetRequisitionQuery, GetRequisitionResult>
    {
        public async Task<GetRequisitionResult> Handle(GetRequisitionQuery request, CancellationToken cancellationToken)
        {
            var requisition = await requisitionRepo.GetByIdAsync(request.Id, cancellationToken);
            if (requisition == null)
            {
                throw new NotFoundException($"Requisition with ID {request.Id} not found.");
            }
            return new GetRequisitionResult(requisition);
        }
    }
}
