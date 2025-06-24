namespace ChequeRequisiontService.Core.Dto.UserMenuPermission
{
    public class UserMenuPermissionDto
    {
        public int Id { get; set; }
        public required int UserId { get; set; }
        public required int MenuId { get; set; }
        public bool IsActive { get; set; } = true;
    }
}
