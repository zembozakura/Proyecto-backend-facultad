using AutoMapper;
using MiApp.Application.DTOs;
using MiApp.Domain.Entities;

namespace MiApp.Application.Mappings;

/// <summary>
/// Configuración de AutoMapper para convertir Entities ↔ DTOs
/// </summary>
public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // Category
        CreateMap<Category, CategoryDto>().ReverseMap();
        CreateMap<CreateCategoryDto, Category>();

        // Product
        CreateMap<Product, ProductDto>()
            .ForMember(dest => dest.Category, opt => 
                opt.MapFrom(src => src.Category))
            .ReverseMap();
        CreateMap<CreateProductDto, Product>();
        CreateMap<UpdateProductDto, Product>()
            .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

        // OrderItem
        CreateMap<OrderItem, OrderItemDto>()
            .ForMember(dest => dest.ProductName, opt => 
                opt.MapFrom(src => src.Product!.Name))
            .ReverseMap();
        CreateMap<CreateOrderItemDto, OrderItem>();

        // Order
        CreateMap<Order, OrderDto>()
            .ForMember(dest => dest.Status, opt => 
                opt.MapFrom(src => src.Status.ToString()))
            .ForMember(dest => dest.Items, opt => 
                opt.MapFrom(src => src.Items))
            .ReverseMap();
        CreateMap<CreateOrderDto, Order>();

        // Payment
        CreateMap<Payment, PaymentDto>().ReverseMap();
        CreateMap<CreatePaymentDto, Payment>();
    }
}
