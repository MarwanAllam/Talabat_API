using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Talabat.Apis.DTOs;
using Talabat.Apis.Errors;
using Talabat.APIs.Controllers;
using Talabat.Core.Entities;
using Talabat.Core.Repositories.Contract;

namespace Talabat.Apis.Controllers
{

    public class BasketController : BaseApiController
    {
        private readonly IBasketRepository _basketRepository;
        private readonly IMapper _mapper;

        public BasketController(IBasketRepository basketRepository, IMapper mapper)
        {
            _basketRepository = basketRepository;
            _mapper = mapper;
        }






        //Get Or Recreate Basket
        [HttpGet]
        public async Task<ActionResult<CustomerBasket>> GetBasket(string id)
        {
            var Basket = await _basketRepository.GetBasketAsync(id);
            return Ok(Basket ?? new CustomerBasket(id));
        }



        //Update Or Create New Basket
        [HttpPost]
        public async Task<ActionResult<CustomerBasket>> UpdateBasket(CustomerBasketDto basket)
        {
            var MappedBasket = _mapper.Map<CustomerBasketDto, CustomerBasket>(basket);
            var CreatedOrUpdatedBasket = await _basketRepository.UpdateBasketAsync(MappedBasket);
            if (CreatedOrUpdatedBasket is null)
                return BadRequest(new ApiResponse(400));
            return Ok(CreatedOrUpdatedBasket);

        }
        
        
        
        //Delete Basket
        [HttpDelete]
        public async Task<ActionResult<bool>> DeleteBasket(string id)
        {
            return await _basketRepository.DeleteBasketAsync(id);
        }

    }
}
