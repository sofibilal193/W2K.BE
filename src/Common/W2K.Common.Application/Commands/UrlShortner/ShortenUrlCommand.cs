using MediatR;

namespace DFI.Common.Application.Commands;

public record ShortenUrlCommand(string LongUrl) : IRequest<string>;
