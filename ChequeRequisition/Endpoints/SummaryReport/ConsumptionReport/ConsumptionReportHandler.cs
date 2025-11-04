using BuildingBlocks.CQRS;
using ChequeRequisiontService.Core.Dto.SummaryReport;
using ChequeRequisiontService.Core.Interfaces.Repositories;

namespace ChequeRequisiontService.Endpoints.SummaryReport.ConsumptionReport;
public record GetConsumptionReportQuery(int BankId,string StartDate,string EndDate) :IQuery<GetConsumptionReportRes>;
public record GetConsumptionReportRes(IEnumerable<ConsumptionReportDto> ConsumptionReports);

public class ConsumptionReportHandler(ISummaryReport summaryReport): IQueryHandler<GetConsumptionReportQuery, GetConsumptionReportRes>
{
    public async Task<GetConsumptionReportRes> Handle(GetConsumptionReportQuery request, CancellationToken cancellationToken)
    {
        var fromDate = DateOnly.Parse(request.StartDate);
        var toDate = DateOnly.Parse(request.EndDate);
        var consumptionReports = await summaryReport.GetConsumptionReportAsync(request.BankId,fromDate, toDate, cancellationToken);
        return new GetConsumptionReportRes(consumptionReports);
    }
}
