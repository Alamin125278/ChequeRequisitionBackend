namespace ChequeRequisiontService.Core.Dto.UserRole
{
    public class UserRoleDto
    {
        public int Id { get; set; }
        public required string RoleName { get; set; }
        public required bool IsActive { get; set; } =true;

    }
}
