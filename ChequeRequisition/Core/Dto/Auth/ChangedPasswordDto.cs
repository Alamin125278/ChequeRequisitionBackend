namespace ChequeRequisiontService.Core.Dto.Auth;

public class ChangedPasswordDto
{
    public int Id { get; set; }
    public required string NewPassword { get; set; }
}
