using BuildingBlocks.CQRS;
using ChequeRequisiontService.Core.Dto.Auth;
using ChequeRequisiontService.Core.Dto.Challan;
using ChequeRequisiontService.Core.Interfaces.Repositories;
using ChequeRequisiontService.DbContexts;

namespace ChequeRequisiontService.Endpoints.Challan.GetChallanExport;
public record GetChallanCommand(List<int> ChallanIds):ICommand<GetChallanResponse>;
public record GetChallanResponse(List<ChallanExportDto> Challans);

public class GetChallanHandler : ICommandHandler<GetChallanCommand, GetChallanResponse>
{
    private readonly IChallanRepo _challanRepo;

    public GetChallanHandler(IChallanRepo challanRepo)
    {
        _challanRepo = challanRepo;
    }

    public async Task<GetChallanResponse> Handle(GetChallanCommand request, CancellationToken ct)
    {
        var data = await _challanRepo.GetChallanExportDataAsync(request.ChallanIds, ct);

        if (data == null || !data.Any())
            return new GetChallanResponse(new());

        return new GetChallanResponse(data);
    }
}
