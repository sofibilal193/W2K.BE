using W2K.Identity.Application.DTOs;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace W2K.Identity.Application.Queries;
#pragma warning restore IDE0130 // Namespace does not match folder structure

public readonly record struct GetLoginUserInfoQuery(
    string ProviderId,
    string? FirstName,
    string? LastName,
    string? FullName,
    string Email,
    string? MobilePhone,
    string Step) : IRequest<LoginUserInfoDto>
{
    public bool IsSignUp => Step == "SignUp";

    public bool IsSignIn => Step == "SignIn";

    public bool IsMobileChange => Step == "MobileChange";

    public bool IsPwdChange => Step == "PwdChange";

    public bool IsTokenValidated => Step == "TokenValidated";
}
