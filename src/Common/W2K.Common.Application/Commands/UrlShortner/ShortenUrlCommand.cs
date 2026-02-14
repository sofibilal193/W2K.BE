using MediatR;

namespace W2K.Common.Application.Commands;

public record ShortenUrlCommand(string LongUrl) : IRequest<string>;
