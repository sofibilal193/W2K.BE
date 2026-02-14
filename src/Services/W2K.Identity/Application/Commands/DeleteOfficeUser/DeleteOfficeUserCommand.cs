#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace W2K.Identity.Application.Commands;
#pragma warning restore IDE0130 // Namespace does not match folder structure

public record DeleteOfficeUserCommand(int? OfficeId, int UserId) : IRequest;
