using MediatR;

namespace DFI.Common.Application.Auth;

public readonly record struct ValidateDevUserCommand(string Email, string Password)
    : IRequest<ValidateDevUserResponse>;
