using W2K.Common.Exceptions;
using W2K.Identity.Application.DTOs;
using W2K.Identity.Entities;
using W2K.Identity.Repositories;
using W2K.Common.Utils;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace W2K.Identity.Application.Queries;
#pragma warning restore IDE0130 // Namespace does not match folder structure

public class GetUserInviteInfoQueryHandler(IIdentityUnitOfWork data) : IRequestHandler<GetUserInviteInfoQuery, UserInviteInfoDto>
{
    private readonly IIdentityUnitOfWork _data = data;

    public async Task<UserInviteInfoDto> Handle(GetUserInviteInfoQuery query, CancellationToken cancellationToken)
    {
        // Get user with office information
        var user = await _data.Users
                    .Include("Offices.Office.Addresses")
                    .FirstOrDefaultAsync(x => x.Id == query.UserId, cancellationToken)
            ?? throw new NotFoundException(nameof(User), query.UserId);

        // Only get office information when OfficeId is provided
        OfficeUser? officeUser = null;
        if (query.OfficeId.HasValue)
        {
            officeUser = user.Offices.FirstOrDefault(x => x.OfficeId == query.OfficeId.Value && !x.IsDisabled)
                ?? throw new NotFoundException(nameof(OfficeUser), query.OfficeId.Value);
        }

        var office = officeUser?.Office;
        var primaryAddress = office?.Addresses.FirstOrDefault(x => x.Type == IdentityConstants.OfficePrimaryAddressType);

        // Mask phone number using extension method
        var maskedPhone = user.MobilePhone.MaskPhoneNumber();

        return new UserInviteInfoDto
        {
            OfficeName = office?.Name ?? string.Empty,
            Address = primaryAddress?.StreetAddress ?? string.Empty,
            City = primaryAddress?.City ?? string.Empty,
            State = primaryAddress?.State ?? string.Empty,
            ZipCode = primaryAddress?.ZipCode ?? string.Empty,
            MaskedPhoneNumber = maskedPhone
        };
    }
}
