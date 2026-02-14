#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace W2K.Identity.Application;
#pragma warning restore IDE0130 // Namespace does not match folder structure

public readonly record struct OfficesOwnersListItemDto(int OfficeId, string OfficeName, string FirstName, string LastName, string Email, string? MobilePhone);
