using BuildingBlocks.CQRS;
using ChequeRequisiontService.Core.Dto.Challan;
using ChequeRequisiontService.Core.Interfaces.Repositories;

namespace ChequeRequisiontService.Endpoints.Challan.GetChallan;
public record GetChallanCommand(List<int> ChallanIds):ICommand<GetChallanResponse>;
public record GetChallanResponse(List<ChallanExportDto> Challans);

public class GetChallanHandler(IChallanRepo challanRepo): ICommandHandler<GetChallanCommand, GetChallanResponse>
{
    public async Task<GetChallanResponse> Handle(GetChallanCommand request, CancellationToken cancellationToken)
    {
        var data = await challanRepo.GetChallanExportDataAsync(request.ChallanIds, cancellationToken);
        return new GetChallanResponse(data);
    }
}

