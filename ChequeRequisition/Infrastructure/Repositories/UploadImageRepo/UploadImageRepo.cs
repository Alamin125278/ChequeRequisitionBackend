using ChequeRequisiontService.Core.Interfaces.Repositories;

namespace ChequeRequisiontService.Infrastructure.Repositories.UploadImageRepo
{
    public class UploadImageRepo(IWebHostEnvironment _env) : IUploadImageRepo
    {
        private readonly IWebHostEnvironment _env = _env;
        public async Task<string> UploadImageAsync(IFormFile file, string folderName, CancellationToken cancellationToken = default)
        {
            var uploadsFolder = Path.Combine(_env.WebRootPath, "uploads", folderName);
            if (!Directory.Exists(uploadsFolder))
                Directory.CreateDirectory(uploadsFolder);

            var fileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
            var filePath = Path.Combine(uploadsFolder, fileName);

            await using var stream = new FileStream(filePath, FileMode.Create);
            await file.CopyToAsync(stream, cancellationToken);

            return $"/uploads/{folderName}/{fileName}";
        }
    }
}
