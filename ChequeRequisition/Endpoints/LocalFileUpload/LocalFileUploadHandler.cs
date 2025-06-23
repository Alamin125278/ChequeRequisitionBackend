using BuildingBlocks.CQRS;
using ChequeRequisiontService.Core.Dto.Auth;
using ChequeRequisiontService.Core.Dto.Requisition;
using ChequeRequisiontService.Core.Interfaces.Repositories;
using FluentValidation;
using Mapster;
using System.Numerics;
using System.Windows.Input;

namespace ChequeRequisiontService.Endpoints.LocalFileUpload;

public record LocalFileUploadCommand(int BankId,string BranchName,string AccountNo,string RoutingNo,string StartNo,string EndNo,string ChequeType,string ChequePrefix,string MicrNo,string Series, string AccountName,string CusAddress,int BookQty,int TransactionCode, int Leaves,int CourierCode,string ReceivingBranchName,int Serverity) :ICommand<LocalFileUploadResult>;
public record LocalFileUploadResult(bool IsSuccess, string Message);

public class LocalFileUploadCommandValidator : AbstractValidator<LocalFileUploadCommand>
{
    public LocalFileUploadCommandValidator()
    {
        RuleFor(x => x.BankId).NotEmpty().WithMessage("Bank Id is required.");
        RuleFor(x => x.BranchName).NotEmpty().WithMessage("Branch Name is required.");
        RuleFor(x => x.AccountNo).NotEmpty().WithMessage("Account Number is required.");
        RuleFor(x => x.RoutingNo).NotEmpty().WithMessage("Routing Number is required.");
        RuleFor(x => x.StartNo).NotEmpty().WithMessage("Start Number is required.");
        RuleFor(x => x.EndNo).NotEmpty().WithMessage("End Number is required.");
        RuleFor(x => x.ChequeType).NotEmpty().WithMessage("Cheque Type is required.");
        RuleFor(x => x.ChequePrefix).NotEmpty().WithMessage("Cheque Prefix is required.");
        RuleFor(x => x.MicrNo).NotEmpty().WithMessage("MICR Number is required.");
        RuleFor(x => x.Series).NotEmpty().WithMessage("Series is required.");
        RuleFor(x => x.AccountName).NotEmpty().WithMessage("Account Name is required.");
        RuleFor(x => x.CusAddress).NotEmpty().WithMessage("Customer Address is required.");
        RuleFor(x => x.BookQty).GreaterThan(0).WithMessage("Book Quantity must be greater than zero.");
        RuleFor(x => x.Leaves).GreaterThan(0).WithMessage("Leaves must be greater than zero.");
        RuleFor(x => x.CourierCode).GreaterThan(0).WithMessage("Courier Code must be greater than zero.");
        RuleFor(x => x.ReceivingBranchName).NotEmpty().WithMessage("Receiving Branch Name is required.");
        RuleFor(x => x.Serverity).GreaterThan(0).WithMessage("Serverity must be between 1 and 5.");
    }
}
public class LocalFileUploadHandler(IBranchRepo branchRepo,ILocalFileUploadRepo localFileUploadRepo, AuthenticatedUserInfo authenticatedUserInfo) : ICommandHandler<LocalFileUploadCommand, LocalFileUploadResult>
{
    private readonly ILocalFileUploadRepo _localFileUploadRepo = localFileUploadRepo;
    public async Task<LocalFileUploadResult> Handle(LocalFileUploadCommand request, CancellationToken cancellationToken)
    {
        int branchId = await branchRepo.GetIdAsync(request.BankId, request.BranchName, cancellationToken);
        if (branchId == 0)
        {
            return new LocalFileUploadResult(false, "Branch not found.");
        }
        int receivingBranchId = await branchRepo.GetIdAsync(request.BankId, request.ReceivingBranchName, cancellationToken);
        if (receivingBranchId == 0)
        {
            return new LocalFileUploadResult(false, "Receiving branch not found.");
        }
        var requisitionDto = request.Adapt<RequisitionDto>();
        requisitionDto.BranchId = branchId;
        requisitionDto.ReceivingBranchId = receivingBranchId;
        bool isUploaded = await _localFileUploadRepo.LocalFileUploadAsync(requisitionDto, authenticatedUserInfo.Id, cancellationToken);
        
        if (isUploaded)
        {
            return new LocalFileUploadResult(true, "File uploaded successfully.");
        }
        return new LocalFileUploadResult(false, "Failed to upload file.");
    }
}
