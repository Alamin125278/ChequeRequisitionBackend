using BuildingBlocks.CQRS;
using ChequeRequisiontService.Core.Dto.SummaryReport;
using ChequeRequisiontService.Core.Interfaces.Repositories;

namespace ChequeRequisiontService.Endpoints.SummaryReport;
public record GetSummaryReportQuery(int BankId,string StartDate,string EndDate,int Severity) :IQuery<GetSummaryReportRes>;
public record GetSummaryReportRes(IEnumerable<SummaryReportDto> SummaryReports);
public class SummaryReportHandler(ISummaryReport summaryReport): IQueryHandler<GetSummaryReportQuery, GetSummaryReportRes>
{
    public async Task<GetSummaryReportRes> Handle(GetSummaryReportQuery request, CancellationToken cancellationToken)
    {
        var fromDate = DateOnly.Parse(request.StartDate);
        var toDate = DateOnly.Parse(request.EndDate);
        var summaryReports = await summaryReport.GetSummaryReportAsync(request.BankId, fromDate, toDate,request.Severity, cancellationToken);
        return new GetSummaryReportRes(summaryReports);
    }
}
