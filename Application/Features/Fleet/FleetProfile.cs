using AutoMapper;
using NewRentalCarManagerAPI.Models;

namespace NewRentalCarManagerAPI.Application.Features.Fleet;

public class FleetProfile : Profile
{
    public FleetProfile()
    {
        CreateMap<CarBrand, CarBrandDto>();
        CreateMap<CreateCarBrandDto, CarBrand>();
        CreateMap<UpdateCarBrandDto, CarBrand>();

        CreateMap<CarModel, CarModelDto>()
            .ForMember(d => d.BrandName, opt => opt.MapFrom(s => s.Brand != null ? s.Brand.Name : string.Empty));
        CreateMap<CreateCarModelDto, CarModel>();
        CreateMap<UpdateCarModelDto, CarModel>();

        CreateMap<Location, LocationDto>();
        CreateMap<CreateLocationDto, Location>();
        CreateMap<UpdateLocationDto, Location>();

        CreateMap<CarPricing, CarPricingDto>();
        CreateMap<CreateCarPricingDto, CarPricing>();
        CreateMap<UpdateCarPricingDto, CarPricing>();

        CreateMap<CarAvailabilityBlock, CarAvailabilityBlockDto>();
        CreateMap<CreateCarAvailabilityBlockDto, CarAvailabilityBlock>();

        CreateMap<Car, CarDto>()
            .ForMember(d => d.OwnerName, opt => opt.MapFrom(s => s.Owner != null ? s.Owner.FullName : string.Empty))
            .ForMember(d => d.ModelName, opt => opt.MapFrom(s => s.Model != null ? s.Model.Name : string.Empty))
            .ForMember(d => d.BrandName, opt => opt.MapFrom(s => s.Model != null && s.Model.Brand != null ? s.Model.Brand.Name : string.Empty))
            .ForMember(d => d.PricePerDay, opt => opt.MapFrom(s => s.CarPricings != null ? s.CarPricings.Where(p => p.IsActive).Select(p => p.PriceVnd).FirstOrDefault() : 0))
            .ForMember(d => d.Seats, opt => opt.MapFrom(s => s.Model != null ? s.Model.SeatCount : (short)4))
            .ForMember(d => d.Transmission, opt => opt.MapFrom(s => s.TransmissionType.ToString()))
            .ForMember(d => d.Status, opt => opt.MapFrom(s => s.Status.ToString()))
            .ForMember(d => d.Location, opt => opt.MapFrom(s => s.Location != null ? s.Location.City : string.Empty));
    }
}