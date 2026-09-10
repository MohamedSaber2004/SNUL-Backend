using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SNUL.Shared.Common.DTOs.UserManagement;
using SNUL.Shared.Common.Extensions;
using SNUL.Shared.Domain.Models;
using SNUL.Shared.Localization;
using SNUL.Shared.Results;

namespace UserManagement.Service.API.Features.Users.Queries.GetUsers
{
    public class GetUsersQueryHandler : IRequestHandler<GetUsersQuery, PaginatedResult<UserDto>>
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public GetUsersQueryHandler(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<PaginatedResult<UserDto>> Handle(GetUsersQuery request, CancellationToken cancellationToken)
        {
            var query = _userManager.Users
                .Where(u => !u.IsDeleted)
                .AsNoTracking();

            if (!string.IsNullOrWhiteSpace(request.SearchTerm))
            {
                var term = request.SearchTerm.Trim().ToLower();
                query = query.Where(u =>
                    u.FullName.ToLower().Contains(term) ||
                    (u.Email != null && u.Email.ToLower().Contains(term)) ||
                    (u.PhoneNumber != null && u.PhoneNumber.Contains(term)));
            }

            if (request.UserType.HasValue)
            {
                query = query.Where(u => u.UserType == request.UserType.Value);
            }

            if (request.IsActive.HasValue)
            {
                query = query.Where(u => u.IsActive == request.IsActive.Value);
            }

            return await query
                .OrderByDescending(u => u.CreatedAt)
                .ToPaginatedListAsync(
                    u => new UserDto
                    {
                        Id = u.Id,
                        FullName = u.FullName,
                        Email = u.Email ?? string.Empty,
                        PhoneNumber = u.PhoneNumber,
                        ProfilePictureName = u.ProfilePictureName,
                        UserType = u.UserType,
                        CompanyId = u.CompanyId,
                        Language = u.Language,
                        IsActive = u.IsActive,
                        IsEmailConfirmed = u.EmailConfirmed,
                        CreatedAt = u.CreatedAt,
                        UpdatedAt = u.UpdatedAt,
                        Roles = new List<string> { u.UserType.ToString() }
                    },
                    request.PageNumber,
                    request.PageSize,
                    LocalizationKeys.UserManagement.UsersFetched,
                    cancellationToken);
        }
    }
}
