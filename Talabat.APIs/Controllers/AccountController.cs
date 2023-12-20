using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Talabat.Apis.DTOs;
using Talabat.Apis.Errors;
using Talabat.Apis.Extentions;
using Talabat.APIs.Controllers;
using Talabat.Core.Entities.Identity;
using Talabat.Core.Services.Contract;

namespace Talabat.Apis.Controllers
{

    public class AccountController : BaseApiController
    {
        private readonly UserManager<AppUser> _userManger;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly IAuthService _authService;
        private readonly IMapper _mapper;

        public AccountController
            (UserManager<AppUser> userManger
            , SignInManager<AppUser> signInManager
            , IAuthService authService, IMapper mapper
            )
        {
            _userManger = userManger;
            _signInManager = signInManager;
            _authService = authService;
            _mapper = mapper;
        }
        //Register
        [HttpPost("register")]
        public async Task<ActionResult<UserDto>> Register(RegisterDto model)
        {
            if (CheckEmailExists(model.Email).Result.Value)
                return BadRequest(new ApiResponse(400, "This Email is Already is Use"));
            var user = new AppUser()
            {
                DisplayName = model.DisplayName,
                Email = model.Email,
                UserName = model.Email.Split('@')[0],
                PhoneNumber = model.PhoneNumber,

            };
            var Result = await _userManger.CreateAsync(user, model.Password);
            if (!Result.Succeeded) 
                return BadRequest(new ApiResponse(400));
            var ReturnedUser = new UserDto()
            {
                DisplayName = user.DisplayName,
                Email = user.Email,
                Token = await _authService.CreateTokenAsync(user, _userManger)
            };
            return Ok(ReturnedUser);

        }

        //Login
        [HttpPost("login")]
        public async Task<ActionResult<UserDto>> Login(LoginDto model)
        {
            var user = await _userManger.FindByEmailAsync(model.Email);
            if (user is null)
                return Unauthorized(new ApiResponse(401));
            var result = await _signInManager.CheckPasswordSignInAsync(user, model.Password, false);
            if (!result.Succeeded) 
                return Unauthorized(new ApiResponse(401));
            return Ok(new UserDto()
            {
                DisplayName = user.DisplayName,
                Email = user.Email,
                Token = await _authService.CreateTokenAsync(user, _userManger)
            });

        }
        
        
        [Authorize]
        [HttpGet]
        public async Task<ActionResult<UserDto>> GetCurrentUser()
        {
            var email = User.FindFirstValue(ClaimTypes.Email);
            var user = await _userManger.FindByEmailAsync(email);
            var ReturnedUser = new UserDto()
            {
                DisplayName = user.DisplayName,
                Email = user.Email,
                Token = await _authService.CreateTokenAsync(user, _userManger)
            };
            return Ok(ReturnedUser);
        }


        [Authorize]
        [HttpGet("address")]
        public async Task<ActionResult<AddressDto>> GetUserAddress()
        {
            var user = await _userManger.FindUserWithAddressAsync(User);
            var MappedAddress = _mapper.Map<Address, AddressDto>(user.Address);
            return Ok(MappedAddress);
        }



        [Authorize]
        [HttpPut("address")]
        public async Task<ActionResult<AddressDto>> UpdateAddress(AddressDto UpdateAddress)
        {
            var user = await _userManger.FindUserWithAddressAsync(User);
            if (user is null) return Unauthorized(new ApiResponse(401));
            var Address = _mapper.Map<AddressDto, Address>(UpdateAddress);
            Address.Id = user.Address.Id;
            user.Address = Address;
            var Result = await _userManger.UpdateAsync(user);
            if (!Result.Succeeded) return BadRequest(new ApiResponse(400));
            return Ok(UpdateAddress);
        }


        [HttpGet("emailExists")]
        public async Task<ActionResult<bool>> CheckEmailExists(string email)
        {
            return await _userManger.FindByEmailAsync(email) is not null;
        }

    }
}
