namespace ChequeRequisiontService.Core.Dto.DefaultMenuPermission
{
    public class DefaultMenuPermissionByRoleDto

    {
            public int? Id { get; set; }
            public int MenuId { get; set; }
            public required string MenuName { get; set; }
            public required string MenuPath { get; set; }
            public required bool HasPermission { get; set; }
        

    }
}
