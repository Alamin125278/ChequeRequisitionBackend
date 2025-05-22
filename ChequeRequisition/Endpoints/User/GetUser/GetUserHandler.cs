using BuildingBlocks.CQRS;
using ChequeRequisiontService.Core.Dto.User;
using ChequeRequisiontService.Core.Interfaces.Repositories;

namespace ChequeRequisiontService.Endpoints.User.GetUser
{
    public record GetUserByIdQuery(int Id) : IQuery<GetUserByIdResult>;

    public record GetUserByIdResult(UserDto? User);
    public class GetUserHandler(IUserRepo userRepo) : IQueryHandler<GetUserByIdQuery, GetUserByIdResult>
    {
        public async Task<GetUserByIdResult> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
        {
            var user = await userRepo.GetByIdAsync(request.Id, cancellationToken);
            return user == null ? throw new NotFoundException($"User with ID {request.Id} not found.") : new GetUserByIdResult(user);
        }
    }
}
