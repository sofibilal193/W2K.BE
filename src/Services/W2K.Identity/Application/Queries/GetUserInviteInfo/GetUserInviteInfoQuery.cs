using W2K.Identity.Application.DTOs;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace W2K.Identity.Application.Queries;
#pragma warning restore IDE0130 // Namespace does not match folder structure

public record GetUserInviteInfoQuery(int UserId, int? OfficeId) : IRequest<UserInviteInfoDto>;
