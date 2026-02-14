namespace DFI.Common.Application.Validations;

[AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
public sealed class MessageAttribute(string message) : Attribute
{
    public string Message { get; } = message;
}
