using AutoMapper;
using POS.Domain.Entities;
using POS.Domain.Models.CQRS.Commands.OrderItems;
using POS.Domain.Models.CQRS.Commands.Orders;
using POS.Domain.Models.CQRS.Commands.Products;
using POS.Domain.Models.Results;
using POS.Domain.Models.Results.Bases;

namespace POS.Domain.Configs;

public class PagedListConverter<TSource, TDestination> : ITypeConverter<IPagedList<TSource>, IPagedList<TDestination>>
{
    public IPagedList<TDestination> Convert(IPagedList<TSource> source, IPagedList<TDestination> destination,
        ResolutionContext context)
    {
        var mappedItems = context.Mapper.Map<IEnumerable<TSource>, IEnumerable<TDestination>>(source.Results);
        return new PagedList<TDestination>(mappedItems, source.PageNumber, source.PageSize, source.TotalItemCount);
    }
}

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap(typeof(IPagedList<>), typeof(IPagedList<>)).ConvertUsing(typeof(PagedListConverter<,>));
        ProductMapping();
        OrderMapping();
        OrderItemMapping();
    }

    private void ProductMapping()
    {
        CreateMap<Product, ProductResult>().ReverseMap();
        CreateMap<Product, ProductCreateCommand>().ReverseMap();
        CreateMap<Product, ProductUpdateCommand>().ReverseMap();
    }

    private void OrderMapping()
    {
        CreateMap<Order, OrderResult>().ReverseMap();
        CreateMap<Order, OrderCreateCommand>().ReverseMap();
        CreateMap<Order, OrderUpdateCommand>().ReverseMap();
    }

    private void OrderItemMapping()
    {
        CreateMap<OrderItem, OrderItemResult>().ReverseMap();
        CreateMap<OrderItem, OrderItemCreateCommand>().ReverseMap();
        CreateMap<OrderItem, OrderItemUpdateCommand>().ReverseMap();
    }
}

public static class MapperExtensions
{
    public static PagedList<TDestination> MapPagedList<TSource, TDestination>(
        this IMapper mapper, PagedList<TSource> source)
    {
        var items = mapper.Map<List<TDestination>>(source.Results);
        return new PagedList<TDestination>(items, source.PageNumber, source.PageSize, source.TotalItemCount);
    }
}