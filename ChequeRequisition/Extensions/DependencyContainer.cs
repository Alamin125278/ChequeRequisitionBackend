using ChequeRequisiontService.Core.Dto.Auth;
using ChequeRequisiontService.Core.Dto.User;
using ChequeRequisiontService.Core.Interfaces.Repositories;
using ChequeRequisiontService.Core.Interfaces.Repositories.IUserRole;
using ChequeRequisiontService.Core.Interfaces.Services.Auth;
using ChequeRequisiontService.Core.Interfaces.Services.FtpServices;
using ChequeRequisiontService.Infrastructure.Repositories;
using ChequeRequisiontService.Infrastructure.Repositories.BankRepo;
using ChequeRequisiontService.Infrastructure.Repositories.BranchRepo;
using ChequeRequisiontService.Infrastructure.Repositories.DefaultMenuPermision;
using ChequeRequisiontService.Infrastructure.Repositories.FtpImportExcel;
using ChequeRequisiontService.Infrastructure.Repositories.LocalFileUploadRepo;
using ChequeRequisiontService.Infrastructure.Repositories.MenuRepo;
using ChequeRequisiontService.Infrastructure.Repositories.RequisitionRepo;
using ChequeRequisiontService.Infrastructure.Repositories.UploadImageRepo;
using ChequeRequisiontService.Infrastructure.Repositories.UserMenuPermissionRepo;
using ChequeRequisiontService.Infrastructure.Repositories.UserRepo;
using ChequeRequisiontService.Infrastructure.Repositories.UserRoleRepo;
using ChequeRequisiontService.Infrastructure.Repositories.VendorRepo;
using ChequeRequisiontService.Infrastructure.Services.Auth;
using ChequeRequisiontService.Infrastructure.Services.FtpServices;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

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
        services.AddScoped<IFtpImportRepo, FtpImportRepo>();
        services.AddScoped<IExcelParserService,ExcelParserService>();
        services.AddScoped<IFtpService,FtpService>();
        //services.AddHostedService<FtpImportService>();
        services.AddHostedService<FtpWorkerStarter>();
        services.AddScoped<IFtpLogDbService, FtpLogDbService>();
        services.AddScoped<IRequisitionTrackingRepo, RequisitionTrackingRepo>();
        services.AddScoped<ILocalFileUploadRepo, LocalFileUploadRepo>();
        services.AddScoped<IChallanRepo, ChallanRepo>();
    }
}
