using W2K.Identity.Application.DTOs;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace W2K.Identity.Application.Commands;
#pragma warning restore IDE0130 // Namespace does not match folder structure

public record UpsertSessionCommand(string Base64FingerPrint, string? SessionId, string? ClientEncryptionKeyBase64Encoded) : IRequest<UserSessionDto?>;
