namespace ChequeRequisiontService.Core.Dto.Menu
{
    public class MenuDto
    {
        public int Id { get; set; }
        public string? MenuName { get; set; }
        public string? Title { get; set; }
        public string? Path { get; set; }
        public string? Icon { get; set; }
        public int? ParentId { get; set; }
        public bool IsActive { get; set; } = true;
    }
}
