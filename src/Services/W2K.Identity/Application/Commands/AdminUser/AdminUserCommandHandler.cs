using W2K.Identity.Repositories;
using W2K.Identity.Entities;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace W2K.Identity.Application.Commands;
#pragma warning restore IDE0130 // Namespace does not match folder structure

public class AdminUserCommandHandler(IIdentityUnitOfWork data) : IRequestHandler<AdminUserCommand, int>
{
    private readonly IIdentityUnitOfWork _data = data;

    public async Task<int> Handle(AdminUserCommand command, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(command);
        ArgumentNullException.ThrowIfNull(command.Email);

        var user = await _data.Users.FirstOrDefaultAsync(x => x.Email == command.Email, cancellationToken);

        if (user is null)
        {
            user = new User(null, command.FirstName, command.LastName, command.Email, command.MobilePhone, null);

            _data.Users.Add(user);
            _ = await _data.SaveChangesAsync(cancellationToken);
        }
        return user.Id;
    }
}
