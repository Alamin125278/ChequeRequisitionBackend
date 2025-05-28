using ChequeRequisiontService.Core.Dto.Requisition;
using ChequeRequisiontService.Core.Interfaces.Repositories;
using ChequeRequisiontService.DbContexts;
using ChequeRequisiontService.Models.CRDB;
using Mapster;
using Microsoft.EntityFrameworkCore;

namespace ChequeRequisiontService.Infrastructure.Repositories.RequisitionRepo
{
    public class RequisitionRepo(CRDBContext cRDBContext) : IRequisitonRepo
    {
        private readonly CRDBContext _cRDBContext= cRDBContext;
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

        public Task<int> GetAllCountAsync(string? Search = null, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
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
    }
}
