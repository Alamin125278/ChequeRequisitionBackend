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
     DateOnly? RequestDate, bool? IsAgent = null, int Skip = 0, int Limit = 10,
     string? Search = null, CancellationToken cancellationToken = default)
        {
            // Get all matching Requisition IDs by ChallanNumber if search exists
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

            // Now filter ChequeBookRequisitions
            var data = await _cRDBContext.ChequeBookRequisitions.AsNoTracking()
                .Include(x => x.Bank)
                .Include(x => x.Branch)
                .Include(x => x.ReceivingBranch)
                .Include(x => x.StatusNavigation)
                .Include(x => x.RequestedByNavigation)
                .Where(x =>
                    string.IsNullOrEmpty(Search) ||
                    (x.AccountNo != null && x.AccountNo.Contains(Search)) ||x.AccountName.Contains(Search)||
                    requisitionIdsFromChallan.Contains(x.Id)
                )
                .Where(x => x.IsDeleted == false)
                .Where(x => x.Status == Status || Status == null)
                .Where(x => x.BankId == BankId || BankId == null)
                .Where(x => x.BranchId == BranchId || BranchId == null)
                .Where(x => x.Serverity == Severity || Severity == null)
                .Where(x => x.RequestDate == RequestDate || RequestDate == null)
                .Where(x => IsAgent == null || x.IsAgent == IsAgent)
                .Where(x => x.VendorId == VendorId || VendorId == null)
                .OrderByDescending(x => x.Id)
                .Skip(Skip)
                .Take(Limit)
                .ToListAsync(cancellationToken);

            // Map to DTO
            var dtos = data.Adapt<List<RequisitionDto>>();

            // Fetch ChallanNumbers if needed
            var idsForChallan = dtos
                .Where(x => x.Status is not (1 or 2 or 3))
                .Select(x => x.Id)
                .ToList();

            if (idsForChallan.Any())
            {
                var challans = await (
                    from d in _cRDBContext.ChallanDetails
                    join c in _cRDBContext.Challans on d.ChallanId equals c.Id
                    where d.RequisitionItemId.HasValue && idsForChallan.Contains(d.RequisitionItemId.Value)
                    select new { d.RequisitionItemId, c.ChallanNumber }
                ).ToListAsync(cancellationToken);

                var challanMap = challans
    .GroupBy(x => x.RequisitionItemId)
    .ToDictionary(g => g.Key, g => g.First().ChallanNumber);

                foreach (var dto in dtos)
                {
                    if (challanMap.TryGetValue(dto.Id, out var challanNo))
                        dto.ChallanNumber = challanNo;
                }
            }

            return dtos;
        }

        public async Task<IEnumerable<RequisitionDto>> GetAllAsync(
     int? Status, int? BankId, int? BranchId, int? VendorId, int? Severity,
     DateOnly? RequestDate,
     string? Search = null,bool? IsAgent=null, CancellationToken cancellationToken = default)
        {
            var requisitions = await _cRDBContext.ChequeBookRequisitions
                                .AsNoTracking()
                                .Include(x => x.Bank)
                                .Include(x => x.Branch)
                                .Include(x => x.ReceivingBranch)
                                .Include(x => x.StatusNavigation)
                                .Include(x => x.RequestedByNavigation)
                                .Where(x => !x.IsDeleted)
                                .Where(x => string.IsNullOrEmpty(Search) || x.AccountNo.Contains(Search)||x.AccountName.Contains(Search))
                                .Where(x => !Status.HasValue || x.Status == Status)
                                .Where(x => !BankId.HasValue || x.BankId == BankId)
                                .Where(x => !VendorId.HasValue || x.VendorId == VendorId)
                                .Where(x => !BranchId.HasValue || x.BranchId == BranchId)
                                .Where(x => !Severity.HasValue || x.Serverity == Severity)
                                .Where(x => IsAgent == null || x.IsAgent == IsAgent)
                                .Where(x => !RequestDate.HasValue || x.RequestDate == RequestDate)
                                .ToListAsync(cancellationToken);

            // Step 2: Map to DTO
            var dtos = requisitions.Adapt<List<RequisitionDto>>();
            return dtos;
        }

        public Task<int> GetAllCountAsync(string? Search = null, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public async Task<int> GetAllCountAsync(
      int? Status, int? BankId, int? BranchId, int? VendorId, int? Severity,
      DateOnly? RequestDate, string? Search, bool? IsAgent = null, CancellationToken cancellationToken = default)
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
                .Where(x => !VendorId.HasValue || x.VendorId == VendorId);

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
    }
}
