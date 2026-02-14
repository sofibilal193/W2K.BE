using W2K.Common.Persistence.Repositories;

namespace W2K.Config.Repositories;

public interface IConfigRepository : IDbRepository<Entities.Config>
{
}
