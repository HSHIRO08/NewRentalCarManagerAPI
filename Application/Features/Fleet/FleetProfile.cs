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

        // UpdateCarDto → Car: only map simple scalar fields.
        // Enums (Status/FuelType/TransmissionType), ImageUrls, LocationId,
        // PricePerDay, TenantId and ManufactureYear are handled manually in CarService.
        CreateMap<UpdateCarDto, Car>()
            .ForMember(d => d.Color, opt => opt.MapFrom(s => s.Color))
            .ForMember(d => d.MileageKm, opt => opt.MapFrom(s => s.MileageKm))
            .ForMember(d => d.Description, opt => opt.MapFrom(s => s.Description))
            .ForMember(d => d.HasIotDevice, opt => opt.MapFrom(s => s.HasIotDevice))
            .ForMember(d => d.IotDeviceId, opt => opt.MapFrom(s => s.IotDeviceId))
            .ForMember(d => d.LicensePlate, opt => opt.Condition(s => !string.IsNullOrWhiteSpace(s.LicensePlate)))
            .ForMember(d => d.ManufactureYear, opt => opt.Condition(s => s.ManufactureYear.HasValue))
            .ForMember(d => d.ManufactureYear, opt => opt.MapFrom(s => s.ManufactureYear!.Value))
            .ForMember(d => d.Status, opt => opt.Ignore())
            .ForMember(d => d.FuelType, opt => opt.Ignore())
            .ForMember(d => d.TransmissionType, opt => opt.Ignore())
            .ForMember(d => d.ImageUrls, opt => opt.Ignore())
            .ForMember(d => d.LocationId, opt => opt.Ignore())
            .ForMember(d => d.Location, opt => opt.Ignore())
            .ForMember(d => d.TenantId, opt => opt.Ignore())
            .ForMember(d => d.Features, opt => opt.MapFrom(s => s.Features))
            .ForAllMembers(opt => opt.Condition((src, dest, srcMember) => srcMember != null));

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