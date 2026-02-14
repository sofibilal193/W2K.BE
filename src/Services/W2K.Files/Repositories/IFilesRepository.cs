using W2K.Common.Persistence.Repositories;
using File = W2K.Files.Entities.File;

namespace W2K.Files.Repositories;

public interface IFilesRepository : IDbRepository<File>
{
}
