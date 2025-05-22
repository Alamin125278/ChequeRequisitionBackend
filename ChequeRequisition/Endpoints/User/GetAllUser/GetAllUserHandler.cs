using BuildingBlocks.CQRS;
using ChequeRequisiontService.Core.Interfaces.Repositories;

namespace ChequeRequisiontService.Endpoints.User.GetAllUser
{
    public record GetAllUserQuery(int Skip=0,int Limit=10 ,string? Search = null) : IQuery<GetUserResult>;
    
    public record GetUserResult(string Message, IEnumerable<Core.Dto.User.UserDto> UserDtos);

    public class GetAllUserHandler(IUserRepo userRepo) : IQueryHandler<GetAllUserQuery, GetUserResult>
    {
        private readonly IUserRepo _userRepo = userRepo;
        public async Task<GetUserResult> Handle(GetAllUserQuery request, CancellationToken cancellationToken)
        {
            var users = await _userRepo.GetAllAsync(request.Skip, request.Limit, request.Search, cancellationToken);
            return new GetUserResult("Retreiving Users Success.", users);
        }
    }
}
