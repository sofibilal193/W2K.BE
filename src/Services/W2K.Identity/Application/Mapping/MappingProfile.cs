using System.Reflection;
using AutoMapper;
using W2K.Common.Application;
using W2K.Common.Application.DTOs;
using W2K.Common.Application.Mappings;
using W2K.Common.ValueObjects;
using W2K.Identity.Application.DTOs;
using W2K.Identity.Application.Mappings;
using W2K.Identity.Entities;

namespace W2K.Identity.Application.Mapping;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        this.CreateMappings(Assembly.GetExecutingAssembly());

        _ = this.CreateMap<Office, OfficeDto>()
            .ForMember(x => x.Address, x => x.MapFrom(src => MapPrimaryAddress(src)))
            .ForMember(x => x.Owners, x => x.MapFrom(src => MapOwners(src)))
            .ForMember(x => x.BankAccounts, x => x.MapFrom(src => MapBankAccounts(src)));
        ConfigureUserForOfficeDtoMapping();

        _ = this.CreateMap<OfficeOwner, OfficeOwnerDto>()
            .ForMember(x => x.DOB, x => x.MapFrom(src => src.DOBDate));

        _ = this.CreateMap<Address, AddressDto>();

        _ = this.CreateMap<OfficeBankAccount, OfficeBankAccountDto>();

        _ = this.CreateMap<Office, LenderDto>();
    }

    private static Address? MapPrimaryAddress(Office src)
    {
        return src.Addresses.FirstOrDefault(x => x.Type == IdentityConstants.OfficePrimaryAddressType);
    }

    private static List<OfficeOwnerDto> MapOwners(Office src)
    {
        return [.. src.Owners.Select(x => new OfficeOwnerDto
        {
            FirstName = x.FirstName,
            MiddleName = x.MiddleName,
            LastName = x.LastName,
            Email = x.Email,
            MobilePhone = x.MobilePhone ?? string.Empty,
            SSN = x.SSN ?? string.Empty,
            DOB = !string.IsNullOrWhiteSpace(x.DOB) && DateOnly.TryParse(x.DOB, System.Globalization.DateTimeFormatInfo.InvariantInfo, out var dobValue) ? dobValue : null,
            Ownership = x.Ownership,
            Address = x.Address is null
                ? new AddressDto()
                : new AddressDto
                {
                    Address1 = x.Address.Address1,
                    Address2 = x.Address.Address2,
                    City = x.Address.City,
                    State = x.Address.State,
                    ZipCode = x.Address.ZipCode,
                    Country = x.Address.Country,
                    Type = x.Address.Type,
                    County = x.Address.County,
                },
        })];
    }

    private static List<OfficeBankAccountDto> MapBankAccounts(Office src)
    {
        return [.. src.BankAccounts.Select(x => new OfficeBankAccountDto
        {
            Type = x.Type,
            BankName = x.BankName,
            RoutingNumber = x.RoutingNumber,
            AccountName = x.AccountName,
            AccountNumber = x.AccountNumber,
            IsPrimary = x.IsPrimary,
        })];
    }

    private void ConfigureUserForOfficeDtoMapping()
    {
        _ = CreateMap<OfficeUser, OfficeUserDto>()
            .ForMember(x => x.OfficeName, x => x.MapFrom(src => MapOfficeName(src)))
            .ForMember(x => x.FirstName, x => x.MapFrom(src => MapUserFirstName(src)))
            .ForMember(x => x.LastName, x => x.MapFrom(src => MapUserLastName(src)))
            .ForMember(x => x.Email, x => x.MapFrom(src => MapUserEmail(src)))
            .ForMember(x => x.MobilePhone, x => x.MapFrom(src => MapUserMobilePhone(src)))
            .ForMember(x => x.Title, x => x.MapFrom(src => src.Title))
            .ForMember(x => x.Role, x => x.MapFrom(src => MapRoleName(src)))
            .ForMember(x => x.LastLoginDateTimeUtc, x => x.MapFrom(src => MapLastLoginDateTimeUtc(src)))
            .ForMember(x => x.IsActive, x => x.MapFrom(src => !src.IsDisabled))
            .ForMember(x => x.IsDefault, x => x.MapFrom(src => src.IsDefault))
            .ForMember(x => x.InviteAcceptedDateTimeUtc, x => x.MapFrom(src => src.InviteAcceptedDateTimeUtc))
            .ForMember(x => x.Status, x => x.MapFrom(src => UserMappings.MapUserStatus(src.User)));
    }

    private static string? MapOfficeName(OfficeUser src)
    {
        return src.Office?.Name;
    }

    private static string? MapUserFirstName(OfficeUser src)
    {
        return src.User?.FirstName;
    }

    private static string? MapUserLastName(OfficeUser src)
    {
        return src.User?.LastName;
    }

    private static string MapUserEmail(OfficeUser src)
    {
        return src.User?.Email ?? string.Empty;
    }

    private static string? MapUserMobilePhone(OfficeUser src)
    {
        return src.User?.MobilePhone;
    }

    private static string MapRoleName(OfficeUser src)
    {
        return src.Role?.Name ?? string.Empty;
    }

    private static DateTime MapLastLoginDateTimeUtc(OfficeUser src)
    {
        return src.User?.LastLoginDateTimeUtc ?? DateTime.MinValue;
    }

}
