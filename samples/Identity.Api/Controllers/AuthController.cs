using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Quantic.Core;
using Identity.Api.Commands;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;

namespace Identity.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly ICommandHandler<LoginWithGoogleCommand> _loginHandler;

        public AuthController(ICommandHandler<LoginWithGoogleCommand> loginHandler)
        {
            _loginHandler = loginHandler;
        }

        [HttpPost("google")]
        public async Task<IActionResult> LoginWithGoogle([FromBody] LoginWithGoogleCommand command)
        {
            var context = new RequestContext(
                Guid.NewGuid().ToString(),
                Request.Headers.ToDictionary(h => h.Key, h => h.Value.ToString()));

            var result = await _loginHandler.Handle(command, context);

            if (result.IsSuccess)
            {
                // Create claims from the result
                var userData = result.Data as dynamic;
                var claims = new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, userData.UserId.ToString()),
                    new Claim(ClaimTypes.Email, userData.Email),
                    new Claim(ClaimTypes.Name, userData.Name),
                    new Claim("GoogleId", userData.GoogleId)
                };

                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var authProperties = new AuthenticationProperties
                {
                    IsPersistent = true,
                    ExpiresUtc = DateTimeOffset.UtcNow.AddDays(7)
                };

                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(claimsIdentity),
                    authProperties);

                return Ok(result.Data);
            }

            return BadRequest(result.Errors);
        }
    }
} 