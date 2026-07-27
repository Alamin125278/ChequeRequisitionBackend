using ChequeRequisiontService.Core.Dto.Challan;
using ChequeRequisiontService.Core.Dto.Requisition;
using ChequeRequisiontService.Core.Interfaces.Repositories;
using ChequeRequisiontService.DbContexts;
using ChequeRequisiontService.Models.CRDB;
using DocumentFormat.OpenXml.Spreadsheet;
using DocumentFormat.OpenXml.Wordprocessing;
using FluentValidation;
using Mapster;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System.Linq;

namespace ChequeRequisiontService.Infrastructure.Repositories.RequisitionRepo
{
    public class RequisitionRepo(CRDBContext cRDBContext) : IRequisitonRepo
    {
        private readonly CRDBContext _cRDBContext= cRDBContext;
        public Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken cancellationToken = default)
        {
            return _cRDBContext.Database.BeginTransactionAsync(cancellationToken);
        }
        public async Task<RequisitionDto> CreateAsync(RequisitionDto entity, int UserId, CancellationToken cancellationToken = default)
        {
            try
            {
                var data = entity.Adapt<ChequeBookRequisition>();
                data.CreatedAt = DateTime.UtcNow;
                data.CreatedBy = UserId;
                data.RequestedBy = UserId;
                data.Status = 1; // Assuming 1 is the default status for a new requisition
                await _cRDBContext.ChequeBookRequisitions.AddAsync(data, cancellationToken);
                var result = await _cRDBContext.SaveChangesAsync(cancellationToken);
                if(result>0)
                {
                    return data.Adapt<RequisitionDto>();
                }
                throw new Exception("Failed to create cheque requisition");
            }
            catch(DbUpdateException ex)
            {
                throw new Exception("Database update error: " + (ex.InnerException?.Message ?? ex.Message), ex);
            }
        }

        public async Task<bool> DeleteAsync(int id, int UserId, CancellationToken cancellationToken = default)
        {
            var requisition = await _cRDBContext.ChequeBookRequisitions.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
            if (requisition == null)
                return false;
            requisition.IsDeleted = true;
            requisition.UpdatedAt = DateTime.UtcNow;
            requisition.UpdatedBy = UserId;
            var result = await _cRDBContext.SaveChangesAsync(cancellationToken);
            return result > 0;
        }

        public async Task<IEnumerable<RequisitionDto>> GetAllAsync(int Skip = 0, int Limit = 10, string? Search = null, CancellationToken cancellationToken = default)
        {
            var data= await _cRDBContext.ChequeBookRequisitions.AsNoTracking()
                .Where(x => (x.AccountNo != null && x.AccountNo.ToString().Contains(Search)) || x.ChequePrefix.Contains(Search) || Search == null)
                .Where(x => x.IsDeleted == false)
                .Skip(Skip)
                .Take(Limit)
                .ToListAsync(cancellationToken);
            return data.Adapt<IEnumerable<RequisitionDto>>();
        }

        public async Task<IEnumerable<RequisitionDto>> GetAllAsync(
int? Status, int? BankId, int? BranchId, int? VendorId, int? Severity,
DateOnly? RequestDate, bool? IsAgent = null,
int Skip = 0, int Limit = 10,
string? Search = null,string? CourierCode = null,
CancellationToken cancellationToken = default)
        {
            // 1️⃣ মূল query তৈরি
            var query = _cRDBContext.ChequeBookRequisitions
                .AsNoTracking()
                .Where(r => !r.IsDeleted
                            && (Status == null || r.Status == Status)
                            && (BankId == null || r.BankId == BankId)
                            && (BranchId == null || r.BranchId == BranchId)
                            && (VendorId == null || r.VendorId == VendorId)
                            && (Severity == null || r.Serverity == Severity)
                            && (RequestDate == null || r.RequestDate == RequestDate)
                            && (IsAgent == null || r.IsAgent == IsAgent)
                            && (string.IsNullOrEmpty(CourierCode) || r.CourierCode==CourierCode)
                            && (string.IsNullOrEmpty(Search)
                                || (r.AccountNo != null && r.AccountNo.Contains(Search))
                                || r.AccountName.Contains(Search)
                                || _cRDBContext.Challans
                                   .Any(c => _cRDBContext.ChallanDetails
                                                .Any(d => d.RequisitionItemId == r.Id && d.ChallanId == c.Id)
                                             && c.ChallanNumber.Contains(Search))))
                .OrderByDescending(r => r.Id); // ✅ DESC ঠিকভাবে database-level এ

            // 2️⃣ Skip + Take (Pagination)
            var pagedRequisitions = await query
                .Skip(Skip)
                .Take(Limit)
                .ToListAsync(cancellationToken);

            // 3️⃣ Related data fetch করা (joins safer way)
            var requisitionIds = pagedRequisitions.Select(r => r.Id).ToList();

            var joinedData = await (
                from r in _cRDBContext.ChequeBookRequisitions
                join b in _cRDBContext.Banks on r.BankId equals b.Id into rb
                from b in rb.DefaultIfEmpty()
                join br in _cRDBContext.Branches on r.BranchId equals br.Id into rbr
                from br in rbr.DefaultIfEmpty()
                join rbrc in _cRDBContext.Branches on r.ReceivingBranchId equals rbrc.Id into rrbr
                from rbrc in rrbr.DefaultIfEmpty()
                join s in _cRDBContext.Statuses on r.Status equals s.Id into rs
                from s in rs.DefaultIfEmpty()
                join u in _cRDBContext.Users on r.RequestedBy equals u.Id into ru
                from u in ru.DefaultIfEmpty()
                join d in _cRDBContext.ChallanDetails on r.Id equals d.RequisitionItemId into rd
                from d in rd.DefaultIfEmpty()
                join c in _cRDBContext.Challans on d.ChallanId equals c.Id into rc
                from c in rc.DefaultIfEmpty()
                where requisitionIds.Contains(r.Id)
                select new { r, b, br, rbrc, s, u, c }
            ).ToListAsync(cancellationToken);

            // 4️⃣ Map to DTO
            var result = joinedData
                .GroupBy(x => x.r.Id) // duplicate remove
                .Select(g => {
                    var e = g.OrderByDescending(x => x.r.Id).First(); // DESC-safe
                    return new RequisitionDto
                    {
                        Id = e.r.Id,
                        BankId = e.r.BankId,
                        BranchId = e.r.BranchId,
                        AccountNo = e.r.AccountNo,
                        RoutingNo = e.r.RoutingNo,
                        StartNo = e.r.StartNo,
                        EndNo = e.r.EndNo,
                        ChequeType = e.r.ChequeType,
                        ChequePrefix = e.r.ChequePrefix,
                        MicrNo = e.r.MicrNo,
                        Series = e.r.Series,
                        AccountName = e.r.AccountName,
                        CusAddress = e.r.CusAddress,
                        BookQty = e.r.BookQty,
                        TransactionCode = e.r.TransactionCode,
                        Leaves = e.r.Leaves,
                        CourierCode = e.r.CourierCode,
                        ReceivingBranchId = e.r.ReceivingBranchId,
                        RequestDate = e.r.RequestDate.ToString("MM/dd/yyyy"),
                        Serverity = e.r.Serverity,
                        Remarks = e.r.Remarks,
                        AgentNum = e.r.AgentNum,
                        IsAgent = e.r.IsAgent ?? false, // ✅ Null-safe
                        AccFlag = e.r.AccFlag,
                        Status = e.r.Status,
                        IsDeleted = e.r.IsDeleted,

                        ChallanNumber = e.c?.ChallanNumber,
                        BankName = e.b?.BankName,
                        BranchName = e.br?.BranchName,
                        BranchCode = e.br?.BranchCode,
                        StatusName = e.s?.StatusName,
                        RequestName = e.u?.Name,
                        ReceivingBranchName = e.rbrc?.BranchName,
                        ReceivingBranchCode = e.rbrc?.BranchCode,
                        //DistId=e.r.DistId,
                        QrId = e.r.QrId,
                        SecurityCode = e.r.SecurityCode,
                        TokenText = e.r.TokenText,
                        CoverText = e.r.CoverText
                    };
                })
                .ToList();

            return result;
        }

        public async Task<IEnumerable<RequisitionDto>> GetAllAsync(
     int? Status, int? BankId, int? BranchId, int? VendorId, int? Severity,
     DateOnly? RequestDate,
     string? Search = null, bool? IsAgent = null, string? CourierCode = null,
     CancellationToken cancellationToken = default)
        {
            // 1️⃣ মূল query তৈরি
            var query = _cRDBContext.ChequeBookRequisitions
                .AsNoTracking()
                .Where(r => !r.IsDeleted
                            && (Status == null || r.Status == Status)
                            && (BankId == null || r.BankId == BankId)
                            && (BranchId == null || r.BranchId == BranchId)
                            && (VendorId == null || r.VendorId == VendorId)
                            && (Severity == null || r.Serverity == Severity)
                            && (RequestDate == null || r.RequestDate == RequestDate)
                            && (IsAgent == null || r.IsAgent == IsAgent)
                            && (string.IsNullOrEmpty(CourierCode) || r.CourierCode == CourierCode)
                            && (string.IsNullOrEmpty(Search)
                                || (r.AccountNo != null && r.AccountNo.Contains(Search))
                                || r.AccountName.Contains(Search)
                                || _cRDBContext.Challans
                                   .Any(c => _cRDBContext.ChallanDetails
                                                .Any(d => d.RequisitionItemId == r.Id && d.ChallanId == c.Id)
                                             && c.ChallanNumber.Contains(Search))))
                .OrderByDescending(r => r.Id); // ✅ DESC ঠিকভাবে database-level এ

            // 2️⃣ Skip + Take (Pagination)
            var pagedRequisitions = await query
                .ToListAsync(cancellationToken);

            // 3️⃣ Related data fetch করা (joins safer way)
            var requisitionIds = pagedRequisitions.Select(r => r.Id).ToList();

            var joinedData = await (
                from r in _cRDBContext.ChequeBookRequisitions
                join b in _cRDBContext.Banks on r.BankId equals b.Id into rb
                from b in rb.DefaultIfEmpty()
                join br in _cRDBContext.Branches on r.BranchId equals br.Id into rbr
                from br in rbr.DefaultIfEmpty()
                join rbrc in _cRDBContext.Branches on r.ReceivingBranchId equals rbrc.Id into rrbr
                from rbrc in rrbr.DefaultIfEmpty()
                join s in _cRDBContext.Statuses on r.Status equals s.Id into rs
                from s in rs.DefaultIfEmpty()
                join u in _cRDBContext.Users on r.RequestedBy equals u.Id into ru
                from u in ru.DefaultIfEmpty()
                join d in _cRDBContext.ChallanDetails on r.Id equals d.RequisitionItemId into rd
                from d in rd.DefaultIfEmpty()
                join c in _cRDBContext.Challans on d.ChallanId equals c.Id into rc
                from c in rc.DefaultIfEmpty()
                where requisitionIds.Contains(r.Id)
                select new { r, b, br, rbrc, s, u, c }
            ).ToListAsync(cancellationToken);

            // 4️⃣ Map to DTO
            var result = joinedData
                .GroupBy(x => x.r.Id) // duplicate remove
                .Select(g => {
                    var e = g.OrderByDescending(x => x.r.Id).First(); // DESC-safe
                    return new RequisitionDto
                    {
                        Id = e.r.Id,
                        BankId = e.r.BankId,
                        BranchId = e.r.BranchId,
                        AccountNo = e.r.AccountNo,
                        RoutingNo = e.r.RoutingNo,
                        StartNo = e.r.StartNo,
                        EndNo = e.r.EndNo,
                        ChequeType = e.r.ChequeType,
                        ChequePrefix = e.r.ChequePrefix,
                        MicrNo = e.r.MicrNo,
                        Series = e.r.Series,
                        AccountName = e.r.AccountName,
                        CusAddress = e.r.CusAddress,
                        BookQty = e.r.BookQty,
                        TransactionCode = e.r.TransactionCode,
                        Leaves = e.r.Leaves,
                        CourierCode = e.r.CourierCode,
                        ReceivingBranchId = e.r.ReceivingBranchId,
                        RequestDate = e.r.RequestDate.ToString("MM/dd/yyyy"),
                        Serverity = e.r.Serverity,
                        Remarks = e.r.Remarks,
                        AgentNum = e.r.AgentNum,
                        IsAgent = e.r.IsAgent ?? false, // ✅ Null-safe
                        AccFlag = e.r.AccFlag,
                        Status = e.r.Status,
                        IsDeleted = e.r.IsDeleted,

                        ChallanNumber = e.c?.ChallanNumber,
                        BankName = e.b?.BankName,
                        BranchName = e.br?.BranchName,
                        BranchCode = e.br?.BranchCode,
                        StatusName = e.s?.StatusName,
                        RequestName = e.u?.Name,
                        ReceivingBranchName = e.rbrc?.BranchName,
                        ReceivingBranchCode = e.rbrc?.BranchCode,
                        //DistId=e.r.DistId,
                        QrId=e.r.QrId,
                        SecurityCode=e.r.SecurityCode,
                        TokenText=e.r.TokenText,
                        CoverText=e.r.CoverText
                    };
                })
                .ToList();

            return result;
        }


        public Task<int> GetAllCountAsync(string? Search = null, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public async Task<int> GetAllCountAsync(
      int? Status, int? BankId, int? BranchId, int? VendorId, int? Severity,
      DateOnly? RequestDate, string? Search, bool? IsAgent = null, string? CourierCode = null, CancellationToken cancellationToken = default)
        {
            List<int> requisitionIdsFromChallan = new();

            if (!string.IsNullOrEmpty(Search))
            {
                requisitionIdsFromChallan = await (
                    from d in _cRDBContext.ChallanDetails
                    join c in _cRDBContext.Challans on d.ChallanId equals c.Id
                    where d.RequisitionItemId.HasValue &&
                          c.ChallanNumber.Contains(Search)
                    select d.RequisitionItemId.Value
                ).Distinct().ToListAsync(cancellationToken);
            }

            var query = _cRDBContext.ChequeBookRequisitions.AsNoTracking()
                .Where(x =>
                    string.IsNullOrEmpty(Search) ||
                    (x.AccountNo != null && x.AccountNo.Contains(Search)) ||
                    x.AccountName.Contains(Search) ||
                    requisitionIdsFromChallan.Contains(x.Id)
                )
                .Where(x => !x.IsDeleted)
                .Where(x => !Status.HasValue || x.Status == Status)
                .Where(x => !BankId.HasValue || x.BankId == BankId)
                .Where(x => !BranchId.HasValue || x.BranchId == BranchId)
                .Where(x => !Severity.HasValue || x.Serverity == Severity)
                .Where(x => !RequestDate.HasValue || x.RequestDate == RequestDate)
                .Where(x => IsAgent == null || x.IsAgent == IsAgent)
                .Where(x => !VendorId.HasValue || x.VendorId == VendorId)
                .Where(x => string.IsNullOrEmpty(CourierCode) || x.CourierCode == CourierCode);

            return await query.CountAsync(cancellationToken);
        }


        public async Task<RequisitionDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            var requsition = await _cRDBContext.ChequeBookRequisitions.AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == id && x.IsDeleted == false, cancellationToken);
            if (requsition == null)
                return null;
            return requsition.Adapt<RequisitionDto>();
        }

        public async Task<RequisitionDto> UpdateAsync(RequisitionDto entity, int Id, int UserId, CancellationToken cancellationToken = default)
        {
            try
            {
                var requisition = _cRDBContext.ChequeBookRequisitions.FirstOrDefault(x => x.Id == Id && x.IsDeleted == false);
                if (requisition == null)
                    throw new Exception("Cheque requisition not found");
                requisition = entity.Adapt(requisition);
                requisition.UpdatedAt = DateTime.UtcNow;
                requisition.UpdatedBy = UserId;
                requisition.Status = 1;
                var result = await _cRDBContext.SaveChangesAsync(cancellationToken);
                if (result > 0)
                {
                    return requisition.Adapt<RequisitionDto>();
                }
                throw new Exception("Failed to update cheque requisition");
            }
            catch(DbUpdateException ex)
            {
                throw new Exception("Database update error: " + (ex.InnerException?.Message ?? ex.Message), ex);
            }
            

        }

        public async Task<int> UpdateChequeListAsync(List<int> Items, int Status, int UserId, CancellationToken cancellationToken)
        {
            try
            {
                var updatedCount = await _cRDBContext.ChequeBookRequisitions
       .Where(x => Items.Contains(x.Id) && x.IsDeleted == false)
       .ExecuteUpdateAsync(setters => setters
           .SetProperty(x => x.UpdatedAt, x => DateTime.UtcNow)
           .SetProperty(x => x.UpdatedBy, x => UserId)
           .SetProperty(x => x.Status, x => Status),
           cancellationToken);

                return updatedCount;
            }
            catch (DbUpdateException ex)
            {
                throw new Exception("Database update error: " + (ex.InnerException?.Message ?? ex.Message), ex);
            }

        }

        public async Task<int> UpdateRequisitionSeverityAsync(List<int> Items, int Severity, int UserId, CancellationToken cancellationToken)
        {
            try
            {
                var updatedCount = await _cRDBContext.ChequeBookRequisitions
                                   .Where(x => Items.Contains(x.Id) && x.IsDeleted == false)
                                   .ExecuteUpdateAsync(setters => setters
                                   .SetProperty(x => x.UpdatedAt, x => DateTime.UtcNow)
                                   .SetProperty(x => x.UpdatedBy, x => UserId)
                                   .SetProperty(x => x.Serverity, x => Severity),
                                   cancellationToken);

                   return updatedCount;
            }
            catch (DbUpdateException ex)
            {
                throw new Exception("Database update error: " + (ex.InnerException?.Message ?? ex.Message), ex);
            }
        }
    }
}
