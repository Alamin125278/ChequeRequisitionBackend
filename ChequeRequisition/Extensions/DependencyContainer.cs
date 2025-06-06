﻿using ChequeRequisiontService.Core.Dto.Auth;
using ChequeRequisiontService.Core.Dto.User;
using ChequeRequisiontService.Core.Interfaces.Repositories;
using ChequeRequisiontService.Core.Interfaces.Repositories.IUserRole;
using ChequeRequisiontService.Core.Interfaces.Services.Auth;
using ChequeRequisiontService.Infrastructure.Repositories.BankRepo;
using ChequeRequisiontService.Infrastructure.Repositories.BranchRepo;
using ChequeRequisiontService.Infrastructure.Repositories.DefaultMenuPermision;
using ChequeRequisiontService.Infrastructure.Repositories.MenuRepo;
using ChequeRequisiontService.Infrastructure.Repositories.RequisitionRepo;
using ChequeRequisiontService.Infrastructure.Repositories.UploadImageRepo;
using ChequeRequisiontService.Infrastructure.Repositories.UserMenuPermissionRepo;
using ChequeRequisiontService.Infrastructure.Repositories.UserRepo;
using ChequeRequisiontService.Infrastructure.Repositories.UserRoleRepo;
using ChequeRequisiontService.Infrastructure.Repositories.VendorRepo;
using ChequeRequisiontService.Infrastructure.Services.Auth;
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

        services.AddScoped<IAuthenticationServices, AuthenticationServices>();

        services.AddScoped<AuthenticatedUserInfo>();
        services.AddScoped<IMenuRepo, MenuRepo>();
        services.AddScoped<IUserRoleRepo, UserRoleRepo>();
        services.AddScoped<IDefaultMenuPermisionRepo, DefaultMenuPermisionRepo>();
        services.AddScoped<IUserMenuPermissionRepo, UserMenuPermissionRepo>();
        services.AddScoped<IUploadImageRepo, UploadImageRepo>();

    }
}
