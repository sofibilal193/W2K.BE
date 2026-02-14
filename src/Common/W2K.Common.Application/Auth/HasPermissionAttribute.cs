#pragma warning disable CA1019 // Define accessors for attribute arguments
using Microsoft.AspNetCore.Authorization;

namespace DFI.Common.Application.Auth;

[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = true)]
public sealed class HasPermissionAttribute(params string[] permissions) : AuthorizeAttribute(string.Join(AuthConstants.PermissionSeparator, permissions))
{
}

#pragma warning restore CA1019 // Define accessors for attribute arguments
