using W2K.Common.Persistence.Repositories;
using W2K.Config.Persistence.Context;

namespace W2K.Config.Repositories;

public class OfficeConfigFieldValueRepository(ConfigDbContext context) : DbRepository<Entities.OfficeConfigFieldValue>(context), IOfficeConfigFieldValueRepository
{
}
