using W2K.Config.Application.DTOs;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace W2K.Config.Application.Queries;
#pragma warning restore IDE0130 // Namespace does not match folder structure

public readonly record struct GetExternalConfigsQuery() : IRequest<IList<ConfigDto>>;
