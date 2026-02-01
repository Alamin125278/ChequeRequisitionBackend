namespace ChequeRequisiontService.Core.Dto.UserMenuPermission
{
    public record UserMenuAccessDto
    {
        public int MenuId { get; init; }
        public bool CanAccess { get; init; }
    }

}
