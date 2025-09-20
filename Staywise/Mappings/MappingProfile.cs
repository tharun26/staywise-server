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
        CreateMap<UpdateListingDto, Listing>();
        CreateMap<AddressDto, Address>();
        CreateMap<LocationDto, Location>();

        CreateMap<CreateBookingDto, Booking>();
        CreateMap<UpdateBookingDto, Booking>();

        // Entity -> Response DTO
        CreateMap<Listing, ListingResponseDto>();
        CreateMap<Booking, BookingResponseDto>();
        CreateMap<Address, AddressDto>();
        CreateMap<Location, LocationDto>();
    } 
}
