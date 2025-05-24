namespace ChequeRequisiontService.Core.Dto.User;

public class UserDto
{
    public int Id { get; set; }

    public int? BranchId { get; set; }

    public int? BankId { get; set; }

    public int? VendorId { get; set; }

    public string? Name { get; set; }

    public string? Email { get; set; }

    public string? UserName { get; set; }

    public string? PasswordHash { get; set; }

    public string? ImagePath { get; set; }

    public string? Role { get; set; }

    public bool? IsActive { get; set; }
}
