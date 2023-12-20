using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Talabat.Apis.DTOs;
using Talabat.Apis.Errors;
using Talabat.APIs.Controllers;
using Talabat.Core.Entities;
using Talabat.Core.Services.Contract;

namespace Talabat.Apis.Controllers
{
    [Authorize]
    public class PaymentsController : BaseApiController
    {
        private readonly IPaymentService _paymentService;
        private readonly IMapper _mapper;

        public PaymentsController(IPaymentService paymentService,IMapper mapper)
        {
            _paymentService = paymentService;
            _mapper = mapper;
        }
        //Create Or Update End Point 
        [ProducesResponseType(typeof(CustomerBasket),StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse),StatusCodes.Status400BadRequest)]
        [HttpPost("{basketId}")]
        public async Task<ActionResult<CustomerBasket>> CreateOrUpdatePaymentIntent(string basketId)
        {
         var basket= await  _paymentService.CreateOrUpdatePaymentIntent(basketId);
            if (basket is null)
                return BadRequest(new ApiResponse(400, "There Is the Problem With Tour Basket"));
            //var MappedBasket = _mapper.Map<CustomerBasket, CustomerBasketDto>(basket);
            return Ok(basket);
        }
    }
}
