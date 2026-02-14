#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace W2K.Identity.Application.Commands;
#pragma warning restore IDE0130 // Namespace does not match folder structure

/// <summary>
/// Command to log out the current user by clearing the current session (single-device logout).
/// </summary>
public record LogoutSessionCommand : IRequest;
