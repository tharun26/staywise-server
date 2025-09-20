using AutoMapper;
using Staywise.Dtos;
using Staywise.Models;

namespace Staywise.Mappings;


public class MappingProfile : Profile
{
    public MappingProfile()
    {

        //Dto to Entity
        CreateMap<CreateListingDto, Listing>();
        CreateMap<AddressDto, Address>();
        CreateMap<LocationDto, Location>();

         // Entity -> Response DTO
        CreateMap<Listing, ListingResponseDto>();
        CreateMap<Address, AddressDto>();
        CreateMap<Location, LocationDto>();
    } 
}
