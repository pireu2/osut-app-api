using AutoMapper;
using OsutApp.Api.DTOs;
using OsutApp.Api.Models;

namespace OsutApp.Api.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<User, UserDto>()
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()));
        CreateMap<UserDto, User>()
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => Enum.Parse<VolunteerStatus>(src.Status)));

        CreateMap<MemberWhitelist, MemberWhitelistDto>()
            .ForMember(dest => dest.Role, opt => opt.MapFrom(src => src.Role.ToString()));
        CreateMap<MemberWhitelistDto, MemberWhitelist>()
            .ForMember(dest => dest.Role, opt => opt.MapFrom(src => Enum.Parse<UserRole>(src.Role)));

        CreateMap<Department, DepartmentDto>()
            .ForMember(dest => dest.Type, opt => opt.MapFrom(src => src.Type.ToString()))
            .ForMember(dest => dest.EventsCount, opt => opt.MapFrom(src => src.Events.Count));
        CreateMap<DepartmentDto, Department>()
            .ForMember(dest => dest.Type, opt => opt.MapFrom(src => Enum.Parse<DepartmentType>(src.Type)))
            .ForMember(dest => dest.Events, opt => opt.Ignore());

        CreateMap<Event, EventDto>()
            .ForMember(dest => dest.SignupsCount, opt => opt.MapFrom(src => src.Signups.Count));
        CreateMap<EventDto, Event>()
            .ForMember(dest => dest.Signups, opt => opt.Ignore());

        CreateMap<EventSignup, EventSignupDto>();
        CreateMap<EventSignupDto, EventSignup>();

        CreateMap<BoardMember, BoardMemberDto>()
            .ForMember(dest => dest.Position, opt => opt.MapFrom(src => src.Position.ToString()));
        CreateMap<BoardMemberDto, BoardMember>()
            .ForMember(dest => dest.Position, opt => opt.MapFrom(src => Enum.Parse<BoardPosition>(src.Position)));
    }
}