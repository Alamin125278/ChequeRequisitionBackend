using ChequeRequisiontService.Core.Dto.SummaryReport;

namespace ChequeRequisiontService.Core.Interfaces.Repositories
{
    public interface ISummaryReport
    {
        Task<IEnumerable<SummaryReportDto>> GetSummaryReportAsync(
            int BankId,
            DateOnly fromDate, 
            DateOnly toDate,int Severity,CancellationToken cancellationToken=default);
    }
}
