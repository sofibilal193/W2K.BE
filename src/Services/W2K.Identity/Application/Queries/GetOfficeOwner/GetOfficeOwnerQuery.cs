using W2K.Common.Application.DTOs;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace W2K.Identity.Application;
#pragma warning restore IDE0130 // Namespace does not match folder structure

public readonly record struct GetOfficeOwnerQuery(int OfficeId, string FirstName, string LastName, string Email, string MobilePhone) : IRequest<OfficeOwnerDto>;
