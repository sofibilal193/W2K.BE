using W2K.Identity.Application.DTOs;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace W2K.Identity.Application.Queries;
#pragma warning restore IDE0130 // Namespace does not match folder structure

/// <summary>
/// Query to retrieve a single user by Id returning basic identity fields.
/// </summary>
public readonly record struct GetUserByIdQuery(int Id) : IRequest<UserDto?>;
