using System.Text.Json;
using W2K.Common.Exceptions;

namespace W2K.Common.Extensions;

public static class JsonSerializerOptionsExtensions
{
    private static IServiceProvider? _provider;

    public static IServiceProvider GetServiceProvider(this JsonSerializerOptions _)
    {
        return _provider
            ?? throw new DomainException("ServiceProvider has not been set.");
    }

    public static void SetServiceProvider(this JsonSerializerOptions _, IServiceProvider provider)
    {
        _provider = provider;
    }
}
