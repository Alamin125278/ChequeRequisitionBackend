using BuildingBlocks.CQRS;
using ChequeRequisiontService.Core.Dto.Auth;
using ChequeRequisiontService.Core.Dto.User;
using ChequeRequisiontService.Core.Interfaces.Repositories;

namespace ChequeRequisiontService.Endpoints.User.GetLoggedinUser
{
    public record GetLoggedinUserQuery():IQuery<GetLoggedinUserResponse>;
    public record GetLoggedinUserResponse(UserDto User);
    public class GetLoggedinUserHandler(IUserRepo userRepo, AuthenticatedUserInfo authenticatedUserInfo) : IQueryHandler<GetLoggedinUserQuery, GetLoggedinUserResponse>
    {
        public async Task<GetLoggedinUserResponse> Handle(GetLoggedinUserQuery request, CancellationToken cancellationToken)
        {
           var userId= authenticatedUserInfo.Id;
            if (userId <= 0)
            {
                throw new UnauthorizedAccessException("User is not authenticated");
            }
            var user = await userRepo.GetByIdAsync(userId, cancellationToken);
            if (user == null)
            {
                throw new NotFoundException("User not found");
            }
            return new GetLoggedinUserResponse(user);
        }
    }
}
