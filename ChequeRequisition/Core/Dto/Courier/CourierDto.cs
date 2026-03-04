namespace ChequeRequisiontService.Core.Dto.Courier
{
    public class CourierDto
    {
        public int Id { get; set; }
        public required string CourierName { get; set; }
        public required string CourierCode { get; set; }
        public required string Phone { get; set; }
        public required string Email { get; set; }
        public bool IsActive { get; set; } = true;
    }
}
