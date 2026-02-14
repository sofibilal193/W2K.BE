#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace W2K.Identity.Application.Queries;
#pragma warning restore IDE0130 // Namespace does not match folder structure

public readonly record struct GetUserPermissionsQuery(int? UserId, int? OfficeId) : IRequest<IList<string>>;
