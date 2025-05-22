using ChequeRequisiontService.Core.Dto.User;

namespace ChequeRequisiontService.Core.Interfaces.Repositories;

public interface IUserRepo : IGenericRepository<UserDto> 
{
    Task<IEnumerable<UserDto>> GetAllAsync(int RoleId, int Skip = 0, int Limit = 10, string? Search = null);
}

