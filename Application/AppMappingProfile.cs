using AutoMapper;
using NewRentalCarManagerAPI.Application.Features.Bookings;
using NewRentalCarManagerAPI.Application.Features.News;
using NewRentalCarManagerAPI.Application.Features.Ops;
using NewRentalCarManagerAPI.Application.Features.Payments;
using NewRentalCarManagerAPI.Application.Features.Users;
using NewRentalCarManagerAPI.Enums;
using NewRentalCarManagerAPI.Models;

namespace NewRentalCarManagerAPI.Application;

public class AppMappingProfile : Profile
{
    public AppMappingProfile()
    {
        // User
        CreateMap<User, UserDto>()
            .ForMember(d => d.RoleName, opt => opt.MapFrom(s => s.Role != null ? s.Role.Name : string.Empty))
            .ForMember(d => d.Status, opt => opt.MapFrom(s => s.Status.ToString()))
            .ForMember(d => d.KycStatus, opt => opt.MapFrom(s => s.KycStatus.ToString()));

        // Booking
        CreateMap<Booking, BookingDto>()
            .ForMember(d => d.RenterName, opt => opt.MapFrom(s => s.Renter != null ? s.Renter.FullName : string.Empty))
            .ForMember(d => d.CarLicensePlate, opt => opt.MapFrom(s => s.Car != null ? s.Car.LicensePlate : string.Empty))
            .ForMember(d => d.Status, opt => opt.MapFrom(s =>
                s.Status == BookingStatus.Pending ? "Pending" :
                s.Status == BookingStatus.Confirmed ? "Confirmed" :
                s.Status == BookingStatus.Active ? "Active" :
                s.Status == BookingStatus.Completed ? "Completed" :
                s.Status == BookingStatus.Cancelled ? "Cancelled" : "Disputed"))
            .ForMember(d => d.PaymentStatus, opt => opt.MapFrom(s =>
                s.Transactions != null && s.Transactions.Any(t =>
                    t.Status == PaymentStatus.Success &&
                    t.Direction == PaymentDirection.Charge) ? "Paid" : "Pending"))
            .ForMember(d => d.EmailStatus, opt => opt.MapFrom(s => "Pending"));

        // Promotion
        CreateMap<Promotion, PromotionDto>();

        // Review
        CreateMap<Review, ReviewDto>()
            .ForMember(d => d.ReviewerName, opt => opt.MapFrom(s => s.Reviewer != null ? s.Reviewer.FullName : string.Empty))
            .ForMember(d => d.RevieweeName, opt => opt.MapFrom(s => s.Reviewee != null ? s.Reviewee.FullName : string.Empty));

        // DamageReport
        CreateMap<DamageReport, DamageReportDto>()
            .ForMember(d => d.ReporterName, opt => opt.MapFrom(s => s.ReportedByNavigation != null ? s.ReportedByNavigation.FullName : string.Empty));

        // Penalty
        CreateMap<Penalty, PenaltyDto>()
            .ForMember(d => d.ChargedToName, opt => opt.MapFrom(s => s.ChargedToNavigation != null ? s.ChargedToNavigation.FullName : string.Empty));

        // Transaction
        CreateMap<Transaction, TransactionDto>()
            .ForMember(d => d.PayerName, opt => opt.MapFrom(s => s.Payer != null ? s.Payer.FullName : string.Empty));

        // OwnerPayout
        CreateMap<OwnerPayout, OwnerPayoutDto>()
            .ForMember(d => d.OwnerName, opt => opt.MapFrom(s => s.Owner != null ? s.Owner.FullName : string.Empty));

        // NewsArticle
        CreateMap<NewsArticle, NewsArticleDto>()
            .ForMember(d => d.AuthorName, opt => opt.MapFrom(s => s.Author != null ? s.Author.FullName : "Ẩn danh"));

        // HandoverRecord
        CreateMap<HandoverRecord, HandoverDto>()
            .ForMember(d => d.Type, opt => opt.MapFrom(s => s.Type.ToString()))
            .ForMember(d => d.RecordedByName, opt => opt.MapFrom(s => s.RecordedByUser != null ? s.RecordedByUser.FullName : string.Empty));
    }
}
