using BuildingBlocks.CQRS;
using ChequeRequisiontService.Core.Dto.Auth;
using ChequeRequisiontService.Core.Dto.User;
using ChequeRequisiontService.Core.Dto.UserMenuPermission;
using ChequeRequisiontService.Core.Interfaces.Repositories;
using ChequeRequisiontService.Infrastructure.Repositories.DefaultMenuPermision;
using FluentValidation;
using Mapster;
using System;

namespace ChequeRequisiontService.Endpoints.User.UpdateUser
{
    public record UpdateUserCommand(int Id, int? BankId, int? BranchId, int? VendorId, string Name, string Email, string UserName, int Role, string? ImagePath = null, string? IsActive = null) : ICommand<UpdateUserResult>;
   public record UpdateUserResult(UserDto User);
    public class UpdateUserValidator : AbstractValidator<UpdateUserCommand>
    {
        public UpdateUserValidator()
        {
            RuleFor(x => x.Id).NotEmpty().WithMessage("Id is required.");
            RuleFor(x => x.Name).NotEmpty().WithMessage("Name is required.");
            RuleFor(x => x.Email).NotEmpty().EmailAddress().WithMessage("Valid email is required.");
            RuleFor(x => x.UserName).NotEmpty().WithMessage("Username is required.");
            RuleFor(x => x.Role).NotEmpty().WithMessage("Role is required.");
            RuleFor(x => x.IsActive).NotNull().WithMessage("IsActive is required.");
        }
    }
    public class UpdateUserHandler(IDefaultMenuPermisionRepo defaultMenuPermisionRepo, IUserMenuPermissionRepo userMenuPermissionRepo,IUserRepo userRepo,AuthenticatedUserInfo authenticatedUserInfo) : ICommandHandler<UpdateUserCommand, UpdateUserResult>
    {
        private readonly IUserRepo _userRepo = userRepo;
        private readonly IUserMenuPermissionRepo _userMenuPermissionRepo = userMenuPermissionRepo;
        private readonly IDefaultMenuPermisionRepo _defaultMenuPermisionRepo = defaultMenuPermisionRepo;
        private readonly int _currentUserId = authenticatedUserInfo.Id;
        public async Task<UpdateUserResult> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
        {
            // 1. Normalize and prepare data
            bool isActive = request.IsActive?.Equals("InActive", StringComparison.OrdinalIgnoreCase) == false;

            // 2. Get existing user data
            var existingUser = await _userRepo.GetByIdAsync(request.Id, cancellationToken) ?? throw new InvalidOperationException($"User with ID {request.Id} not found.");

            // Get Default Menu Permission by Role
            var defaultMenus = await _defaultMenuPermisionRepo.GetAllAsync(request.Role, cancellationToken);
            // 3. If role changed, delete old permissions
            if (request.Role != existingUser.Role)
            {
                await _userMenuPermissionRepo.DeleteMenusByUserIdAsync(request.Id, _currentUserId, cancellationToken);

                foreach (var menu in defaultMenus)
                {
                    var menuId = menu.MenuId;

                    var perMenu = new UserMenuPermissionDto
                    {
                        MenuId = menuId,
                        UserId = request.Id,
                        Id = 0
                    };

                    await _userMenuPermissionRepo.CreateAsync(perMenu,_currentUserId, cancellationToken);
                }
            }


            // 4. Map to DTO
            var updatedUserDto = new UserDto
            {
                Id = request.Id,
                BankId = request.BankId,
                BranchId = request.BranchId,
                VendorId = request.VendorId,
                Name = request.Name,
                UserName = request.UserName,
                Email = request.Email,
                Role = request.Role,
                ImagePath = request.ImagePath,
                IsActive = isActive
            };

            // 5. Persist changes
            var updatedUser = await _userRepo.UpdateAsync(updatedUserDto, request.Id, _currentUserId, cancellationToken);

            // 6. Return result
            return new UpdateUserResult(updatedUser);
        }
    }
}
