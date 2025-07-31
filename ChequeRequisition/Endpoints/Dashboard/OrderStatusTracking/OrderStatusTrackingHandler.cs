using BuildingBlocks.CQRS;
using ChequeRequisiontService.Core.Dto.Auth;
using ChequeRequisiontService.Core.Dto.Dashboard;
using ChequeRequisiontService.Core.Interfaces.Repositories.Dashboard;

namespace ChequeRequisiontService.Endpoints.Dashboard.OrderStatusTracking;

public record GetOrderStatusTrackingQuery(string TimeRange):IQuery<OrderStatusTrackingRes>;
public record OrderStatusTrackingRes(IEnumerable<OrderTrackingDto> OrderTrackings);

public class OrderStatusTrackingHandler(IDashboardRepo dashboardRepo, AuthenticatedUserInfo authenticatedUserInfo) : IQueryHandler<GetOrderStatusTrackingQuery, OrderStatusTrackingRes>
{
    public async Task<OrderStatusTrackingRes> Handle(GetOrderStatusTrackingQuery request, CancellationToken cancellationToken)
    {
        DateOnly startDate;
        DateOnly endDate = DateOnly.FromDateTime(DateTime.Today); // আজকের দিন পর্যন্ত

        var today = DateTime.Today;

        if (request.TimeRange == "today")
        {
            startDate = DateOnly.FromDateTime(DateTime.Today);
        }
        else if (request.TimeRange == "week")
        {
            int diff = (int)today.DayOfWeek;
            startDate = DateOnly.FromDateTime(today.AddDays(-diff));
        }
        else if (request.TimeRange == "month")
        {
            startDate = new DateOnly(today.Year, today.Month, 1);
        }
        else if (request.TimeRange == "quarter")
        {
            int currentMonth = today.Month;
            int quarterStartMonth = ((currentMonth - 1) / 3) * 3 + 1;
            startDate = new DateOnly(today.Year, quarterStartMonth, 1);
        }
        else
        {
            throw new ArgumentException("Invalid time range specified.");
        }

        var orderTrackings = await dashboardRepo.GetOrderTrackingAsync(
            startDate,
            endDate,
            authenticatedUserInfo.BankId,
            authenticatedUserInfo.BranchId,
            authenticatedUserInfo.VendorId,
            cancellationToken
        );

        return new OrderStatusTrackingRes(orderTrackings);
    }

}
