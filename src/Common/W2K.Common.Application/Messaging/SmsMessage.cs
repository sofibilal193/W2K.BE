namespace DFI.Common.Application.Messaging;

public record struct SmsMessage
{
    public string From { get; init; }

    public string To { get; init; }

    public string Body { get; init; }

    public string Sid { get; private set; }

    public void SetSid(string sid)
    {
        Sid = sid;
    }
}
