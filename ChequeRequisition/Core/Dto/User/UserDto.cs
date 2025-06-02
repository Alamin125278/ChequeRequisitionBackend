using ChequeRequisiontService.Core.Dto.Bank;
using ChequeRequisiontService.Core.Dto.Branch;
using ChequeRequisiontService.Core.Dto.UserRole;
using ChequeRequisiontService.Core.Dto.Vendor;

namespace ChequeRequisiontService.Core.Dto.User;

public class UserDto
{
    public int Id { get; set; }

    public int? BranchId { get; set; } = null;

    public int? BankId { get; set; } = null;

    public int? VendorId { get; set; } = null;

    public string? Name { get; set; }

    public string? Email { get; set; }

    public string? UserName { get; set; }

    public string? PasswordHash { get; set; }

    public string? ImagePath { get; set; }

    public int? Role { get; set; }

    public bool? IsActive { get; set; } = true;

    public VendorDto? Vendor { get; set; } = null;
    public BankDto? Bank { get; set; } = null;
    public UserRoleDto? RoleNavigation { get; set; } = null;
    public BranchDto? Branch { get; set; } = null;
    public string BankName => Bank?.BankName ?? string.Empty;
    public string RoleName => RoleNavigation?.RoleName ?? string.Empty;
    public string BranchName=> Branch?.BranchName ?? string.Empty;
    public string VendorName => Vendor?.VendorName ?? string.Empty;

}
