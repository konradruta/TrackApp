using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TrackAPI.Models;
using VueApp1.Server.Services;

namespace TrackAPI.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly IValidator<RegisterUserDto> _validator;
        public AuthController(IAuthService authService, IValidator<RegisterUserDto> validator)
        {
            _authService = authService;
            _validator = validator;
        }

        [HttpPost("login")]
        public ActionResult LoginUser([FromBody] LoginUserDto dto)
        {
            var token = _authService.LoginUser(dto);

            return Ok(token);
        }

        [HttpPost("register")]
        public async Task<ActionResult> RegisterAccount([FromBody] RegisterUserDto dto)
        {
            var validator = await _validator.ValidateAsync(dto);

            if (!validator.IsValid)
            {
                return BadRequest(validator.Errors.Select(e => new { e.PropertyName, e.ErrorMessage }));
            }

            _authService.Register(dto);

            return Ok(new { message = "Konto zostało utworzone" });
        }
    }
}
