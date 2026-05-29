using AutoMapper;
using MiApp.Application.DTOs;
using MiApp.Domain.Entities;

namespace MiApp.Application.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<Category, CategoryDto>();

        CreateMap<Product, ProductDto>()
            .ForMember(d => d.Category, o => o.MapFrom(s => s.Category));

        CreateMap<Customer, CustomerDto>();

        CreateMap<OrderItem, OrderItemDto>()
            .ForMember(d => d.ProductName, o => o.MapFrom(s => s.Product != null ? s.Product.Name : null));

        CreateMap<Order, OrderDto>()
            .ForMember(d => d.Status, o => o.MapFrom(s => s.Status.ToString()))
            .ForMember(d => d.Total, o => o.MapFrom(s => s.Total));

        CreateMap<Payment, PaymentDto>();
    }
}
