using W2K.Common.Exceptions;
using W2K.Common.Identity;
using W2K.Identity.Repositories;
using Microsoft.Extensions.Logging;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace W2K.Identity.Application;
#pragma warning restore IDE0130 // Namespace does not match folder structure

public class UpdateOfficeStatusCommandHandler(
    IIdentityUnitOfWork data,
    ICurrentUser currentUser,
    ILogger<UpdateOfficeStatusCommandHandler> logger)
    : IRequestHandler<UpdateOfficeStatusCommand>
{
    private readonly IIdentityUnitOfWork _data = data;
    private readonly ICurrentUser _currentUser = currentUser;
    private readonly ILogger<UpdateOfficeStatusCommandHandler> _logger = logger;

    public async Task Handle(UpdateOfficeStatusCommand request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        var office = await _data.Offices.GetAsync(request.OfficeId, cancellationToken)
            ?? throw new NotFoundException($"Office with ID {request.OfficeId} not found.");

        // Only update if values have changed (optimization)
        if (office.IsReviewed != request.IsReviewed)
        {
            office.SetReviewed(request.IsReviewed);
        }

        if (office.IsEnrollmentCompleted != request.IsEnrollmentCompleted)
        {
            office.SetEnrollment(request.IsEnrollmentCompleted);
        }

        _ = await _data.SaveEntitiesAsync(cancellationToken);

        _logger.LogInformation(
            "Office status updated by user {UserId}. OfficeId: {OfficeId}, IsReviewed: {IsReviewed}, IsEnrollmentCompleted: {IsEnrollmentCompleted}",
            _currentUser.UserId,
            request.OfficeId,
            request.IsReviewed,
            request.IsEnrollmentCompleted);
    }
}
