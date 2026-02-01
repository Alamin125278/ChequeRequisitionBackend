namespace ChequeRequisiontService.Core.Dto.UserMenuPermission
{
    public class UserMenusPermissionsDto
    {
        public required int MenuId { get; set; }
        public required string MenuName { get; set; }
        public required string MenuPath { get; set; }
        public required string Icon { get; set; }
        public bool? CanAccess { get; set; }
    }
}
