namespace W2K.Identity.Application.DTOs;

/// <summary>
/// Minimal user data projection for lightweight retrieval scenarios.
/// Naming convention: *UserDto for single-user basic projections.
/// </summary>
public record UserDto
{
    public int Id { get; init; }
    public string? ProviderId { get; init; }
    public string? FirstName { get; init; }
    public string? LastName { get; init; }
    public string Email { get; init; } = string.Empty;
    public string? MobilePhone { get; init; }
    public DateTime? LastLoginDateTimeUtc { get; init; }
    public DateTime? LastLogoutDateTimeUtc { get; init; }
}
