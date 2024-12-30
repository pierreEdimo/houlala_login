using AutoMapper;
using user_service.Dto;
using user_service.Model;

namespace user_service.Helper;

public class AutoMapperProfiles : Profile
{
    public AutoMapperProfiles()
    {
        CreateMap<UserEntity, UserDto>()
            .ForMember(x => x.Email, options => options.MapFrom(x => x.Email))
            .ForMember(x => x.Id, options => options.MapFrom(x => x.Id))
            .ForMember(x => x.UserName, options => options.MapFrom(x => x.UserName));

        CreateMap<Address, AddressDto>().ReverseMap();

        CreateMap<CreateAddressDto, Address>().ReverseMap();
    }
}