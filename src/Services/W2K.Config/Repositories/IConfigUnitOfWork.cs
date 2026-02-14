using W2K.Common.Persistence.Repositories;

namespace W2K.Config.Repositories;

public interface IConfigUnitOfWork : IUnitOfWork
{
    IConfigRepository Configs { get; }

    IOfficeConfigFieldRepository OfficeConfigFields { get; }

    IOfficeConfigFieldValueRepository OfficeConfigFieldValues { get; }
}
