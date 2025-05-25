namespace ChequeRequisiontService.Core.Dto.DefaultMenuPermission;

public class DefaultMenuPermisionDto
{
    public int Id { get; set; }
    public required int MenuId { get; set; }
    public required int RoleId { get; set; }
    public bool IsActive { get; set; }= true;
}
