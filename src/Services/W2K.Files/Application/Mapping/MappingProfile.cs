using System.Reflection;
using AutoMapper;
using W2K.Common.Application.Mappings;

namespace W2K.Files.Application.Mapping;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        this.CreateMappings(Assembly.GetExecutingAssembly());
    }
}
