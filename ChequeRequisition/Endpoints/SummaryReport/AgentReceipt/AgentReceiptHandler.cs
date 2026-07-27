using ChequeRequisiontService.Core.Dto.SummaryReport;
using ChequeRequisiontService.Core.Interfaces.Repositories;
using MediatR;

namespace ChequeRequisiontService.Endpoints.SummaryReport.AgentReceipt;

public record GetAgentReceiptReportQuery(int BankId, string RequestDate) : IRequest<GetAgentReceiptReportRes>;
public record GetAgentReceiptReportRes(IEnumerable<AgentReceiptDto> AgentReceipts);

public class AgentReceiptHandler(ISummaryReport summaryReport) : IRequestHandler<GetAgentReceiptReportQuery, GetAgentReceiptReportRes>
{
    public async Task<GetAgentReceiptReportRes> Handle(GetAgentReceiptReportQuery request, CancellationToken cancellationToken)
    {
        var requestDate = DateOnly.Parse(request.RequestDate);
        var agentReceipts = await summaryReport.GetAgentReceiptReportAsync(request.BankId, requestDate, cancellationToken);
        return new GetAgentReceiptReportRes(agentReceipts);
    }
}
