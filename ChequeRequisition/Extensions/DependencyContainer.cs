using ChequeRequisiontService.Core.Dto.User;
using ChequeRequisiontService.Core.Interfaces.Repositories;
using ChequeRequisiontService.Infrastructure.Repositories.BankRepo;
using ChequeRequisiontService.Infrastructure.Repositories.BranchRepo;
using ChequeRequisiontService.Infrastructure.Repositories.RequisitionRepo;
using ChequeRequisiontService.Infrastructure.Repositories.UserRepo;
using ChequeRequisiontService.Infrastructure.Repositories.VendorRepo;
using Microsoft.AspNetCore.Identity;

namespace ChequeRequisiontService.Extensions;

public static class DependencyContainer
{
    public static void AddDependency(this IServiceCollection services)
    {
        services.AddScoped<IUserRepo, UserRepo>();
        services.AddScoped<IPasswordHasher<UserDto>, PasswordHasher<UserDto>>();
        services.AddScoped<IVendorRepo,VendorRepo>();
        services.AddScoped<IBankRepo, BankRepo>();
        services.AddScoped<IBranchRepo, BranchRepo>();
        services.AddScoped<IRequisitonRepo, RequisitionRepo>();

    }
}
