using ChequeRequisiontService.Core.Dto.Branch;
using ChequeRequisiontService.Core.Dto.ManageSerial;
using ChequeRequisiontService.Core.Interfaces.Repositories;
using ChequeRequisiontService.DbContexts;
using Mapster;
using Microsoft.EntityFrameworkCore;

namespace ChequeRequisiontService.Infrastructure.Repositories.ManageSerialRepo;

public class ManageSerialRepo(CRDBContext cRDBContext) : IManageSerialRepo
{
    private readonly CRDBContext _cRDBContext = cRDBContext;
    public async Task<ManageSerialDto?> GetByBankIdAndChequeTypeAsync(int BankId, string ChequeType, int Lvs, CancellationToken cancellationToken = default)
    {
        var data =await _cRDBContext.SetSerialNumbers.AsNoTracking()
           .FirstOrDefaultAsync(x => x.BankId == BankId && x.ChequeType == ChequeType && x.Lvs == Lvs && !x.IsDelete,cancellationToken);

        return data?.Adapt<ManageSerialDto>();

    }

    public async Task<ManageSerialDto> UpdateAsync(int bankId, string chequeType, string endingNumber, int lvs, int userId, CancellationToken cancellationToken = default)
    {
        try
        {
            if (!int.TryParse(endingNumber, out var endingNoInt))
                throw new ArgumentException("Ending number must be a valid numeric value.");

            var serial = await _cRDBContext.SetSerialNumbers
                .FirstOrDefaultAsync(x =>
                    x.BankId == bankId &&
                    x.ChequeType == chequeType &&
                    x.Lvs == lvs &&
                    !x.IsDelete,
                    cancellationToken);

            if (serial is null)
                throw new KeyNotFoundException("No serial found for the given bank and cheque type.");

            if (!int.TryParse(serial.EndLimit, out var endLimitInt))
                throw new InvalidOperationException("The stored EndLimit is not a valid numeric value.");

            if (endingNoInt >= endLimitInt)
                throw new InvalidOperationException("This cheque type has reached its serial number limit. Please update the serial limit first.");

            // Update values
            serial.EndingNo = endingNumber;
            serial.UpdatedBy = userId;
            serial.UpdatedAt = DateTime.UtcNow;

            var result = await _cRDBContext.SaveChangesAsync(cancellationToken);

            if (result <= 0)
                throw new Exception("Failed to update the serial number.");

            return serial.Adapt<ManageSerialDto>();
        }
        catch (ArgumentException ex)
        {
            // Input validation error
            throw new Exception($"Invalid input: {ex.Message}", ex);
        }
        catch (KeyNotFoundException ex)
        {
            // Data not found
            throw new Exception($"Data error: {ex.Message}", ex);
        }
        catch (InvalidOperationException ex)
        {
            // Logic or state issue
            throw new Exception($"Operation error: {ex.Message}", ex);
        }
        catch (Exception ex)
        {
            // General fallback for unexpected errors
            throw new Exception("An unexpected error occurred while updating the serial number.", ex);
        }
    }

}
