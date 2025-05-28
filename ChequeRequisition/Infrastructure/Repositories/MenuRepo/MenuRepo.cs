using ChequeRequisiontService.Core.Dto.BranchDto;
using ChequeRequisiontService.Core.Dto.Menu;
using ChequeRequisiontService.Core.Interfaces.Repositories;
using ChequeRequisiontService.DbContexts;
using ChequeRequisiontService.Models.CRDB;
using Mapster;
using Microsoft.EntityFrameworkCore;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace ChequeRequisiontService.Infrastructure.Repositories.MenuRepo
{
    public class MenuRepo(CRDBContext cRDBContext) : IMenuRepo
    {
        private CRDBContext _cRDBContext = cRDBContext;
        public async Task<MenuDto> CreateAsync(MenuDto entity, int UserId, CancellationToken cancellationToken = default)
        {
           try
            {
                var data = entity.Adapt<Menu>();
                 data.CreatedAt = DateTime.UtcNow;
                data.CreatedBy = UserId;
                data.IsDeleted = false;
                await _cRDBContext.Menus.AddAsync(data, cancellationToken);
                var result = await _cRDBContext.SaveChangesAsync(cancellationToken);
                if (result > 0)
                {
                    return data.Adapt<MenuDto>();
                }
                throw new Exception("Failed to create menu");
            }
            catch (DbUpdateException ex)
            {
                throw new Exception("Database update error: " + (ex.InnerException?.Message ?? ex.Message), ex);
            }

        }

        public async Task<bool> DeleteAsync(int id, int UserId, CancellationToken cancellationToken = default)
        {
            var menu = await _cRDBContext.Menus
                .FirstOrDefaultAsync(x => x.Id == id && x.IsDeleted == false, cancellationToken);
            if (menu == null)
                return false;
            menu.IsDeleted = true;
            menu.UpdatedAt = DateTime.UtcNow;
            menu.UpdatedBy = UserId;
            var result = await _cRDBContext.SaveChangesAsync(cancellationToken);
            return result > 0;
        }

        public async Task<IEnumerable<MenuDto>> GetAllAsync(int Skip = 0, int Limit = 10, string? Search = null, CancellationToken cancellationToken = default)
        {
            var data = await _cRDBContext.Menus.AsNoTracking()
                .Where(x => (x.MenuName.Contains(Search) || x.Title.Contains(Search) || x.Path.Contains(Search) || Search == null))
                .Where(x => x.IsDeleted == false)
                .Skip(Skip)
                .Take(Limit)
                .ToListAsync(cancellationToken);
            return data.Adapt<IEnumerable<MenuDto>>();
        }

        public Task<int> GetAllCountAsync(string? Search = null, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public async Task<MenuDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            var data =await _cRDBContext.Menus.AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == id && x.IsDeleted == false, cancellationToken);
            return data?.Adapt<MenuDto>();

        }

        public async Task<MenuDto> UpdateAsync(MenuDto entity, int Id, int UserId, CancellationToken cancellationToken = default)
        {
            try
            {
                var menu = await _cRDBContext.Menus.FirstOrDefaultAsync(x => x.Id == Id && x.IsDeleted == false,cancellationToken);
                if (menu == null)
                    throw new NotFoundException($"Menu with ID {Id} not found.");
               var data = entity.Adapt(menu);
                data.UpdatedAt = DateTime.UtcNow;
                data.UpdatedBy = UserId;
                var result = await _cRDBContext.SaveChangesAsync(cancellationToken);
                if (result > 0)
                {
                    return data.Adapt<MenuDto>();
                }
                throw new Exception("Failed to update menu");
            }
            catch (DbUpdateException ex)
            {
                throw new Exception("Database update error: " + (ex.InnerException?.Message ?? ex.Message), ex);
            }
        }
    }
}
