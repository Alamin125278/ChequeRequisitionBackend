using BuildingBlocks.CQRS;
using ChequeRequisiontService.Core.Dto.SummaryReport;
using ChequeRequisiontService.Core.Interfaces.Repositories;

namespace ChequeRequisiontService.Endpoints.SummaryReport.DailyProductionReport;

public record GetDailyProductionReportQuery(string Date) : IQuery<GetDailyProductionReportRes>;
public record GetDailyProductionReportRes(IEnumerable<DailyProductionReportDto> DailyProductionReports);

public class DailyProductionReportHandler(ISummaryReport summaryReport) : IQueryHandler<GetDailyProductionReportQuery, GetDailyProductionReportRes>
{
    public async Task<GetDailyProductionReportRes> Handle(GetDailyProductionReportQuery request, CancellationToken cancellationToken)
    {
        if (!DateTime.TryParse(request.Date, out var date))
            throw new ArgumentException("Invalid date format. Please use yyyy-MM-dd format.");
        var dailyProductionReports = await summaryReport.GetDailyProductionReportAsync(date, cancellationToken);
        return new GetDailyProductionReportRes(dailyProductionReports);
    }
}
