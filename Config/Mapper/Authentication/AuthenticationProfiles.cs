using AutoMapper;
using Models.Dtos.Example;
using Models.Dtos.Role;
using Models.Dtos.User;
using Models.Entities.Example;
using Models.Entities.Identity;

namespace Config.Mapper.Authentication;

public class AuthenticationProfiles : Profile
{
    public AuthenticationProfiles()
    {
        //Example Mapping
        CreateMap<TestDto, Test>()
            .ForMember(dest => dest.CreatedDate, src => src.Ignore());
        CreateMap<Test, TestDto>();

        // User mappings
        CreateMap<UserCreateDto, User>()
            .ForMember(dest => dest.Id, src => src.Ignore())
            .ForMember(dest => dest.IsActive, src => src.MapFrom(s => true))
            .ForMember(dest => dest.LastLoginDate, src => src.Ignore())
            .ForMember(dest => dest.RefreshToken, src => src.Ignore())
            .ForMember(dest => dest.RefreshTokenExpirationTime, src => src.Ignore());
        CreateMap<UserUpdateDto, User>()
            .ForMember(dest => dest.Id, src => src.Ignore())
            .ForMember(dest => dest.LastLoginDate, src => src.Ignore())
            .ForMember(dest => dest.RefreshToken, src => src.Ignore())
            .ForMember(dest => dest.RefreshTokenExpirationTime, src => src.Ignore());
        CreateMap<User, UserDto>();
        CreateMap<User, UserListDto>();

        // Role mappings
        CreateMap<RoleCreateUpdateDto, Role>()
            .ForMember(dest => dest.Id, src => src.Ignore());
        CreateMap<Role, RoleDto>();
        CreateMap<Role, RoleListDto>();


    }
}