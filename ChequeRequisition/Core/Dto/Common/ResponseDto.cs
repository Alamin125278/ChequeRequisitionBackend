namespace ChequeRequisiontService.Core.Dto.Common;

public class ResponseDto<T> where T : class
{
    public bool Success {get; set; }
    public int StatusCode { get; set; }
    public string Message { get; set; } = default!;
    public T? Data { get; set; }
}
