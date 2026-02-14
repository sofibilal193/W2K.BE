#pragma warning disable CA1040 // Avoid empty interfaces
using System.Diagnostics.CodeAnalysis;
using AutoMapper;

namespace W2K.Common.Application.Mappings;

/// <summary>
/// Indicates that the type defined a mapping profile configuration.
/// </summary>
public interface IMap
{
    void Mapping(Profile profile);
}

/// <summary>
/// Indicates that the type maps one-to-one from <typeparamref name="T"/>.
/// </summary>
/// <typeparam name="T">Type that type is mapped from.</typeparam>
[SuppressMessage("Maintainability", "S2326", Justification = "An empty Interface is required.")]
public interface IMapFrom<T> { }

/// <summary>
/// Indicates that the type maps one-to-one to <typeparamref name="T"/> (Reverse map).
/// </summary>
/// <typeparam name="T">Type that type is mapped to.</typeparam>
[SuppressMessage("Maintainability", "S2326", Justification = "An empty Interface is required.")]
public interface IMapTo<T> { }
#pragma warning restore CA1040 // Avoid empty interfaces
