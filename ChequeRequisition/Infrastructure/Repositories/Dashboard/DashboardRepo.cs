using ChequeRequisiontService.Core.Dto.Dashboard;
using ChequeRequisiontService.Core.Interfaces.Repositories.Dashboard;
using ChequeRequisiontService.DbContexts;
using DocumentFormat.OpenXml.Wordprocessing;
using Mapster;
using Microsoft.EntityFrameworkCore;

namespace ChequeRequisiontService.Infrastructure.Repositories.Dashboard
{
    public class DashboardRepo(CRDBContext cRDBContext) : IDashboardRepo
    {
        private readonly CRDBContext _cRDBContext = cRDBContext;
        public async Task<int> GetAllCountAsync(int? Status, int? BankId, int? BranchId, int? VendorId, DateOnly? StartDate, DateOnly? EndDate, CancellationToken cancellationToken = default)
        {
            var count = await _cRDBContext.ChequeBookRequisitions.AsNoTracking()
                .Where(x =>
                            (BankId == null || x.BankId == BankId) &&
                            (BranchId == null || x.BranchId == BranchId) &&
                            (Status == null || x.Status == Status) &&
                            (VendorId == null || x.VendorId == VendorId) &&
                            x.RequestDate >= StartDate &&
                            x.RequestDate <= EndDate)
                            .SumAsync(x => x.BookQty, cancellationToken);


            return count;
        }

        public async Task<IEnumerable<BankWiserRequisitonDto>> GetBankWiseRequisitionAsync(int? vendorId, CancellationToken cancellationToken = default)
        {
            var result = await (
                from r in _cRDBContext.ChequeBookRequisitions
                join b in _cRDBContext.Banks on r.BankId equals b.Id
                where r.VendorId == vendorId && r.IsDeleted == false
                group r by b.BankName into g
                orderby g.Sum(x => x.BookQty) descending
                select new BankWiserRequisitonDto
                {
                    BankName = g.Key,
                    TotalRequisitions = g.Sum(x => x.BookQty)
                }
            ).ToListAsync(cancellationToken);

            return result;
        }

        public async Task<IEnumerable<OrderTrackingDto>> GetOrderTrackingAsync(
    DateOnly StartDate, DateOnly EndDate,
    int? BankId,
    int? BranchId,
    int? VendorId,
    CancellationToken cancellationToken = default)
        {
            var query = from s in _cRDBContext.Statuses
                        where  s.IsActive == true && s.IsDeleted != true
                        join r in _cRDBContext.ChequeBookRequisitions
                             .Where(r =>
                                r.IsDeleted != true &&
                                r.RequestDate >= StartDate &&
                                r.RequestDate <= EndDate &&
                                (BankId == null || r.BankId == BankId) &&
                                (BranchId == null || r.BranchId == BranchId) &&
                                (VendorId == null || r.VendorId == VendorId)
                             )
                            on s.Id equals r.Status into joined
                        from r in joined.DefaultIfEmpty()
                        group r by new { s.Id, s.StatusName } into g
                        select new OrderTrackingDto
                        {
                            Label = g.Key.StatusName,
                            Count = g.Sum(x => x.BookQty)

                        };

            return await query.ToListAsync(cancellationToken);
        }

    }
}
