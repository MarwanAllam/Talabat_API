using AutoMapper;
using Talabat.Apis.DTOs;
using Talabat.Core.Entities;
using Talabat.Core.Entities.Order_Aggregate;
namespace Talabat.Apis.Helpers
{
    public class MappingProfiles:Profile
    {
        public MappingProfiles()
        {
            CreateMap<Product, ProductToReturnDto>()
                .ForMember(d => d.Brand, O => O.MapFrom(S => S.Brand.Name))
                .ForMember(d => d.Category, O => O.MapFrom(S => S.Category.Name))
                .ForMember(d => d.PictureUrl, O => O.MapFrom<ProductPictureUrlResolver>());
            CreateMap<AddressDto, Address>();
            CreateMap<Core.Entities.Identity.Address, AddressDto>().ReverseMap();
            CreateMap<CustomerBasketDto, CustomerBasket>();
            CreateMap<BasketItemDto, BasketItem>();
            CreateMap<Order, OrderToReturnDto>().ForMember(d => d.DeliveryMethod, O => O.MapFrom(S => S.DeliveryMethod.ShortName))
                                                .ForMember(d => d.DeliveryMethodCost, O => O.MapFrom(S => S.DeliveryMethod.Cost));
            CreateMap<OrderItem, OrderItemDto>().ForMember(d => d.ProductId, O => O.MapFrom(S => S.Product.ProductId))
                                                .ForMember(d => d.ProductName, O => O.MapFrom(S => S.Product.ProductName))
                                                .ForMember(d => d.PictureUrl, O => O.MapFrom(S => S.Product.PictureUrl))
                                                .ForMember(d => d.PictureUrl, O => O.MapFrom<OrderItemPictureUrlResolver>());

        }
    }
}
