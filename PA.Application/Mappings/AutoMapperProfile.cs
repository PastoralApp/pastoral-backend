using AutoMapper;
using PA.Application.DTOs;
using PA.Domain.Entities;

namespace PA.Application.Mappings;

/// <summary>
/// Configuração de mapeamento do AutoMapper
/// </summary>
public class AutoMapperProfile : Profile
{
    public AutoMapperProfile()
    {
        // User mappings
        CreateMap<User, UserDto>()
            .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email.Value))
            .ForMember(dest => dest.RoleName, opt => opt.MapFrom(src => src.Role.Name))
            .ForMember(dest => dest.Tags, opt => opt.MapFrom(src => src.Tags));

        CreateMap<Tag, TagDto>();

        // Post mappings
        CreateMap<Post, PostDto>()
            .ForMember(dest => dest.Type, opt => opt.MapFrom(src => src.Type.ToString()))
            .ForMember(dest => dest.AuthorName, opt => opt.MapFrom(src => src.Author.Name))
            .ForMember(dest => dest.AuthorPhotoUrl, opt => opt.MapFrom(src => src.Author.PhotoUrl));

        // Evento mappings
        CreateMap<Evento, EventoDto>()
            .ForMember(dest => dest.CreatedByName, opt => opt.MapFrom(src => src.CreatedBy.Name));
    }
}
