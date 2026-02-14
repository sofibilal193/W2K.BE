using System.Text.Json.Serialization;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace W2K.Identity.Application;
#pragma warning restore IDE0130 // Namespace does not match folder structure

public record UpdateOfficeStatusCommand : IRequest
{
    [JsonIgnore]
    public int OfficeId { get; private set; }

    [JsonRequired]
    public bool IsReviewed { get; init; }

    [JsonRequired]
    public bool IsEnrollmentCompleted { get; init; }

    public void SetOfficeId(int officeId)
    {
        OfficeId = officeId;
    }
}

