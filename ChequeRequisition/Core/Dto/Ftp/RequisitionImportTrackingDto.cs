namespace ChequeRequisiontService.Core.Dto.Ftp;

public class RequisitionImportTrackingDto
{
    public int Id { get; set; }
    public required int RequisitionId { get; set; }
    public required int ImportLogId { get; set; }
}
