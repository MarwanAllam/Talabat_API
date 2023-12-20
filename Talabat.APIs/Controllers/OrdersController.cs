using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Security.Claims;
using Talabat.Apis.DTOs;
using Talabat.Apis.Errors;
using Talabat.APIs.Controllers;
using Talabat.Core;
using Talabat.Core.Entities.Order_Aggregate;
using Talabat.Core.Services;


namespace Talabat.Apis.Controllers
{
    [Authorize]
    public class OrdersController : BaseApiController
    {
        private readonly IOrderService _orderService;
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;

        public OrdersController(IOrderService orderService,IMapper mapper,IUnitOfWork unitOfWork)
        {
            _orderService = orderService;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }




        //End Point Create Order
        [ProducesResponseType(typeof(OrderToReturnDto),StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse),StatusCodes.Status400BadRequest)]
        [HttpPost]
        
        public async Task<ActionResult<OrderToReturnDto>>CreateOrder(OrderDto orderDto)
        { 
            var buyerEmail=User.FindFirstValue(ClaimTypes.Email);
            var address = _mapper.Map<AddressDto, Address>(orderDto.shipToAddress);
            var order = await _orderService.CreateOrderAsync(buyerEmail, orderDto.BasketId, orderDto.DeliveryMethodId, address);
            if (order is null) return BadRequest(new ApiResponse(400,"There is A Problem With Your Order"));
            return Ok(_mapper.Map<Order, OrderToReturnDto>(order));   
        }

          
        [HttpGet]
        public async Task<ActionResult<IReadOnlyList<OrderToReturnDto>>> GetOrdersForUser()
        {
            var buyerEmail = User.FindFirstValue(ClaimTypes.Email);
            var orders = await _orderService.GetOrdersForUserAsync(buyerEmail);
            if (orders is null) return NotFound(new ApiResponse(404, "There is No Orders For This User"));
            return Ok(_mapper.Map<IReadOnlyList<Order>, IReadOnlyList <OrderToReturnDto> >(orders));
        }



        [ProducesResponseType(typeof(OrderToReturnDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
        [HttpGet("{id}")]
         public async Task<ActionResult<OrderToReturnDto>> GetOrderForUser(int id)
        {
            var buyerEmail = User.FindFirstValue(ClaimTypes.Email);
            var order = await _orderService.GetOrderByIdForUserAsync(buyerEmail, id);
            if (order is null)
                return NotFound(new ApiResponse(404, $"There is No Order With Id = {id} For This User"));
            var MappedOrder = _mapper.Map<Order, OrderToReturnDto>(order);
            return Ok(MappedOrder);

        }

        ////End Point Get All GeliveryMethod
        [HttpGet("deliveryMethods")]
        public async Task<ActionResult<IReadOnlyList<DeliveryMethod>>> GetDeliveryMethods()
        { 
            var deliveryMethods = await _orderService.GetDeliveryMethodsAsync();
            return Ok(deliveryMethods);

        }





    }
}
