using ChequeRequisiontService.Core.Dto.SummaryReport;
using ChequeRequisiontService.Core.Interfaces.Repositories;
using ChequeRequisiontService.Endpoints.SummaryReport.AgentReceipt;
using MediatR;

namespace ChequeRequisiontService.Endpoints.SummaryReport.AgentSummaryReport;

public record GetAgentSummaryReportQuery(int BankId, string RequestDate ,bool? AgentType ,string? Courier) : IRequest<GetAgentSummaryReportRes>;
public record GetAgentSummaryReportRes(IEnumerable<AgentSummaryReportDto> AgentSummaryReports);


public class AgentSummaryReportHandler(ISummaryReport summaryReport) : IRequestHandler<GetAgentSummaryReportQuery, GetAgentSummaryReportRes>
{
    public async Task<GetAgentSummaryReportRes> Handle(GetAgentSummaryReportQuery request, CancellationToken cancellationToken)
    {
        var requestDate = DateOnly.Parse(request.RequestDate);
        var agentSummaryReports = await summaryReport.GetAgentSummaryReportAsync(request.BankId, requestDate, request.AgentType,request.Courier, cancellationToken);
        return new GetAgentSummaryReportRes(agentSummaryReports);
    }
}
