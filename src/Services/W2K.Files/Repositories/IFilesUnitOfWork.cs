using W2K.Common.Persistence.Repositories;

namespace W2K.Files.Repositories;

public interface IFilesUnitOfWork : IUnitOfWork
{
    IFilesRepository Files { get; }
}
