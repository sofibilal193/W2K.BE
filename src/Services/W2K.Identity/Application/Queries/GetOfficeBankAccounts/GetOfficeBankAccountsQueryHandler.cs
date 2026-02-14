using AutoMapper;
using W2K.Common.Application.DTOs;
using W2K.Common.Exceptions;
using W2K.Identity.Entities;
using W2K.Identity.Repositories;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace W2K.Identity.Application;
#pragma warning restore IDE0130 // Namespace does not match folder structure

public class GetOfficeBankAccountsQueryHandler(IIdentityUnitOfWork data, IMapper mapper)
    : IRequestHandler<GetOfficeBankAccountsQuery, OfficeBankAccountDto>
{
    private readonly IIdentityUnitOfWork _data = data;
    private readonly IMapper _mapper = mapper;

    public async Task<OfficeBankAccountDto> Handle(GetOfficeBankAccountsQuery request, CancellationToken cancellationToken)
    {
        var office = await _data.Offices.Include(x => x.BankAccounts).GetAsync(request.OfficeId, cancellationToken)
            ?? throw new NotFoundException(nameof(Office), request.OfficeId);

        return _mapper.Map<OfficeBankAccountDto>(office.BankAccounts.FirstOrDefault(x => x.IsPrimary));
    }
}
