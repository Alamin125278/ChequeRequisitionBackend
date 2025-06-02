namespace ChequeRequisiontService.Core.Interfaces.Repositories
{
    public interface IUploadImageRepo
    {
        Task<string> UploadImageAsync(IFormFile file, string folderName, CancellationToken cancellationToken = default); 
    }
}