using MediatR;

namespace W2K.Common.Application.Auth;

public readonly record struct ValidateDevUserCommand(string Email, string Password)
    : IRequest<ValidateDevUserResponse>;
