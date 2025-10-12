using BuildingBlocks.CQRS;
using ChequeRequisiontService.Core.Dto.SummaryReport;
using ChequeRequisiontService.Core.Interfaces.Repositories;

namespace ChequeRequisiontService.Endpoints.SummaryReport.CourierSummary;
public record GetCourierSummaryReportQuery(int BankId,string StartDate,string EndDate,int Severity,bool AgentType) :IQuery<GetCourierSummaryReportRes>;
public record GetCourierSummaryReportRes(IEnumerable<SummaryReportDto> SummaryReports);
public class CourierSummaryReportHandler(ISummaryReport summaryReport): IQueryHandler<GetCourierSummaryReportQuery, GetCourierSummaryReportRes>
{
    public async Task<GetCourierSummaryReportRes> Handle(GetCourierSummaryReportQuery request, CancellationToken cancellationToken)
    {
        var fromDate = DateOnly.Parse(request.StartDate);
        var toDate = DateOnly.Parse(request.EndDate);
        var summaryReports = await summaryReport.GetCourierSummaryReportAsync(request.BankId, fromDate, toDate,request.Severity,request.AgentType, cancellationToken);
        return new GetCourierSummaryReportRes(summaryReports);
    }
}
