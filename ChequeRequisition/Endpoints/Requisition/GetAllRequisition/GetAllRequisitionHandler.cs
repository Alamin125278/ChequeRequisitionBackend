using BuildingBlocks.CQRS;
using ChequeRequisiontService.Core.Dto.Requisition;
using ChequeRequisiontService.Core.Interfaces.Repositories;

namespace ChequeRequisiontService.Endpoints.Requisition.GetAllRequisition
{
    public record GetAllRequisitionQuery(int Skip=0, int Limit=10, string? Search=null) : IQuery<GetAllRequisitionResult>;
    public record GetAllRequisitionResult(string Message,IEnumerable<RequisitionDto> Requisitions);
    public class GetAllRequisitionHandler(IRequisitonRepo requisitonRepo) : IQueryHandler<GetAllRequisitionQuery, GetAllRequisitionResult>
    {
        private readonly IRequisitonRepo _requisitonRepo = requisitonRepo;
        public async Task<GetAllRequisitionResult> Handle(GetAllRequisitionQuery request, CancellationToken cancellationToken)
        {
            var requisitions = await _requisitonRepo.GetAllAsync(request.Skip, request.Limit, request.Search, cancellationToken);
            return new GetAllRequisitionResult("Retreiving Requisitions Success.", requisitions);
        }
    }
}
