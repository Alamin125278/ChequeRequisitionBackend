using BuildingBlocks.CQRS;
using ChequeRequisiontService.Core.Dto.Auth;
using ChequeRequisiontService.Core.Dto.Courier;
using ChequeRequisiontService.Core.Interfaces.Repositories;

namespace ChequeRequisiontService.Endpoints.Courier.GetAllCourierForDropDown;
public record GetAllCourierForDropDownQuery():IQuery<GetAllCourierForDropDownResult>;
public record GetAllCourierForDropDownResult(string Message, IEnumerable<CourierDto> CourierDtos);

public  class GetAllCourierForDropDownHandler(ICourierRepo courierRepo, AuthenticatedUserInfo authenticatedUserInfo) : IQueryHandler<GetAllCourierForDropDownQuery, GetAllCourierForDropDownResult>
{
    public async Task<GetAllCourierForDropDownResult> Handle(GetAllCourierForDropDownQuery request, CancellationToken cancellationToken)
    {
        var couriers = await courierRepo.GetAllAsync(cancellationToken);
        return new GetAllCourierForDropDownResult("Retreiving Couriers for dropdown Success.", couriers);
    }
}
