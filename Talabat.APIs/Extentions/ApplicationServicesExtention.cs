using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Talabat.Apis.Errors;
using Talabat.Apis.Helpers;
using Talabat.Core;
using Talabat.Core.Repositories;
using Talabat.Core.Repositories.Contract;
using Talabat.Core.Services;
using Talabat.Core.Services.Contract;
using Talabat.Repository;
using Talabat.Service;

namespace Talabat.Apis.Extentions
{
    public static class ApplicationServicesExtention
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection Services)
        {


            Services.AddScoped(typeof(IPaymentService), typeof(PaymentService));
            Services.AddScoped(typeof(IProductService), typeof(ProductService));
            Services.AddScoped(typeof(IOrderService), typeof(OrderService));
            Services.AddScoped(typeof(IUnitOfWork), typeof(UnitOfWork));
            Services.AddScoped(typeof(IBasketRepository), typeof(BasketRepository));


            Services.AddAutoMapper(typeof(MappingProfiles));

            Services.Configure<ApiBehaviorOptions>(options =>
            {
                options.InvalidModelStateResponseFactory = (actionContext) =>
                {
                    var errors = actionContext.ModelState.Where(P => P.Value.Errors.Count() > 0)
                    .SelectMany(P => P.Value.Errors)
                    .Select(E => E.ErrorMessage)
                    .ToArray();

                    var validationErrorResponse = new ApiValidationErrorResponse()
                    {
                        Errors = errors
                    };

                    return new BadRequestObjectResult(validationErrorResponse);

                };
            });

            return Services;
    }
    } 
}
