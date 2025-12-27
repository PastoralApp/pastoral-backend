using AutoMapper;
using PA.Application.DTOs;
using PA.Domain.Entities;

namespace PA.Application.Mappings;

public class AutoMapperProfile : Profile
{
    public AutoMapperProfile()
    {
        CreateMap<User, UserDto>()
            .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email.Value))
            .ForMember(dest => dest.RoleName, opt => opt.MapFrom(src => src.Role.Name))
            .ForMember(dest => dest.IgrejaNome, opt => opt.MapFrom(src => src.Igreja != null ? src.Igreja.Nome : null))
            .ForMember(dest => dest.Tags, opt => opt.MapFrom(src => src.Tags))
            .ForMember(dest => dest.Grupos, opt => opt.MapFrom(src => src.UserGrupos.Where(ug => ug.IsAtivo)));

        CreateMap<UserGrupo, UserGrupoDto>()
            .ForMember(dest => dest.GrupoName, opt => opt.MapFrom(src => src.Grupo.Name))
            .ForMember(dest => dest.GrupoSigla, opt => opt.MapFrom(src => src.Grupo.Sigla))
            .ForMember(dest => dest.GrupoLogoUrl, opt => opt.MapFrom(src => src.Grupo.LogoUrl))
            .ForMember(dest => dest.PrimaryColor, opt => opt.MapFrom(src => src.Grupo.Theme.PrimaryColor))
            .ForMember(dest => dest.PastoralName, opt => opt.MapFrom(src => src.Grupo.Pastoral != null ? src.Grupo.Pastoral.Name : null));

        CreateMap<Tag, TagDto>();
        CreateMap<Role, RoleDto>()
            .ForMember(dest => dest.Type, opt => opt.MapFrom(src => (int)src.Type));

        CreateMap<Post, PostDto>()
            .ForMember(dest => dest.Type, opt => opt.MapFrom(src => src.Type.ToString()))
            .ForMember(dest => dest.TipoPastoral, opt => opt.MapFrom(src => src.TipoPastoral.ToString()))
            .ForMember(dest => dest.AuthorName, opt => opt.MapFrom(src => src.Author.Name))
            .ForMember(dest => dest.AuthorPhotoUrl, opt => opt.MapFrom(src => src.Author.PhotoUrl))
            .ForMember(dest => dest.CommentsCount, opt => opt.MapFrom(src => src.CommentsCount));

        CreateMap<Post, PostDetailDto>()
            .ForMember(dest => dest.Type, opt => opt.MapFrom(src => src.Type.ToString()))
            .ForMember(dest => dest.TipoPastoral, opt => opt.MapFrom(src => src.TipoPastoral.ToString()))
            .ForMember(dest => dest.AuthorName, opt => opt.MapFrom(src => src.Author.Name))
            .ForMember(dest => dest.AuthorPhotoUrl, opt => opt.MapFrom(src => src.Author.PhotoUrl))
            .ForMember(dest => dest.CommentsCount, opt => opt.MapFrom(src => src.CommentsCount))
            .ForMember(dest => dest.Comments, opt => opt.Ignore());

        CreateMap<PostComment, CommentDto>()
            .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.User.Name))
            .ForMember(dest => dest.UserPhotoUrl, opt => opt.MapFrom(src => src.User.PhotoUrl));

        CreateMap<Evento, EventoDto>()
            .ForMember(dest => dest.CreatedByName, opt => opt.MapFrom(src => src.CreatedBy.Name))
            .ForMember(dest => dest.TypeName, opt => opt.MapFrom(src => src.Type.ToString()))
            .ForMember(dest => dest.TotalParticipantes, opt => opt.MapFrom(src => src.TotalParticipantes))
            .ForMember(dest => dest.VagasDisponiveis, opt => opt.MapFrom(src => src.VagasDisponiveis))
            .ForMember(dest => dest.GrupoNome, opt => opt.MapFrom(src => src.Grupo != null ? src.Grupo.Name : null))
            .ForMember(dest => dest.ResponsavelUserName, opt => opt.MapFrom(src => src.ResponsavelUser != null ? src.ResponsavelUser.Name : null));

        CreateMap<EventoParticipante, EventoParticipanteDto>()
            .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.User.Name))
            .ForMember(dest => dest.UserPhotoUrl, opt => opt.MapFrom(src => src.User.PhotoUrl));

        CreateMap<Notificacao, NotificacaoDto>()
            .ForMember(dest => dest.RemetenteNome, opt => opt.MapFrom(src => src.Remetente.Name))
            .ForMember(dest => dest.GrupoNome, opt => opt.MapFrom(src => src.Grupo != null ? src.Grupo.Name : null))
            .ForMember(dest => dest.GrupoSigla, opt => opt.MapFrom(src => src.Grupo != null ? src.Grupo.Sigla : null));

        CreateMap<Pastoral, PastoralDto>()
            .ForMember(dest => dest.Type, opt => opt.MapFrom(src => src.Type.ToString()))
            .ForMember(dest => dest.TipoPastoral, opt => opt.MapFrom(src => src.TipoPastoral.ToString()));

        CreateMap<Grupo, GrupoDto>()
            .ForMember(dest => dest.PastoralSigla, opt => opt.MapFrom(src => src.Pastoral.Sigla))
            .ForMember(dest => dest.IgrejaNome, opt => opt.MapFrom(src => src.Igreja != null ? src.Igreja.Nome : null));

        CreateMap<Igreja, IgrejaDto>();
        CreateMap<HorarioMissa, HorarioMissaDto>();
    }
}
