using W2K.Common.Persistence.Repositories;
using W2K.Files.Persistence.Context;
using File = W2K.Files.Entities.File;

namespace W2K.Files.Repositories;

public class FilesRepository(FilesDbContext context) : DbRepository<File>(context), IFilesRepository
{
}
