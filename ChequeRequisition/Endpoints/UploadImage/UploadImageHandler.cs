using BuildingBlocks.CQRS;
using ChequeRequisiontService.Core.Interfaces.Repositories;

namespace ChequeRequisiontService.Endpoints.UploadImage;

public record UplaodImageCommand(IFormFile File, string FolderName) : ICommand<UplaodImageResult>;
public record UplaodImageResult(string ImageUrl);

public class UploadImageHandler(IUploadImageRepo uploadImageRepo) : ICommandHandler<UplaodImageCommand, UplaodImageResult>
{
    public async Task<UplaodImageResult> Handle(UplaodImageCommand request, CancellationToken cancellationToken)
    {
        if (request.File == null || request.File.Length == 0)
            throw new ArgumentException("No image file provided.");
        var imageUrl = await uploadImageRepo.UploadImageAsync(request.File, "user", cancellationToken);
        return new UplaodImageResult(imageUrl);
    }
}
