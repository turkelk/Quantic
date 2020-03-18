using System.Linq;
using System.Net.Mime;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Quantic.Core;
using Quantic.Web;

namespace Identity.Api.Controllers.Users
{
    [ApiVersion("1.0")]
    [Route("game/api/v{v:apiVersion}/Users")]
    [ApiController]    
    public class UsersController : BaseController
    {
        private readonly ICommandHandler<RegisterCommand> registerCommandHandler;

        public UsersController(ICommandHandler<RegisterCommand> registerCommandHandler)
        {
            this.registerCommandHandler = registerCommandHandler;
        }

        [HttpPost]
        [Consumes(MediaTypeNames.Application.Json)]
        [Produces(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(Error[]),StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(Error[]),StatusCodes.Status409Conflict)]           
        [ProducesResponseType(typeof(Error[]),StatusCodes.Status500InternalServerError)]     
        public async Task<IActionResult> Register([FromBody]RegisterRequest registerRequest)
        {            
            var result = await registerCommandHandler.Handle(new RegisterCommand 
            { 
                Email = registerRequest.Email, 
                Password = registerRequest.Password,
                Name = registerRequest.Name,
                LastName = registerRequest.LastName                
            }, Context);

            return result.ToResponse(x=> {
                if(x.HasError 
                && x.Errors.Any(err=> err.Code == Msg.UserAlreadyExistByEmail))
                {
                    var errors = x.Errors.Select( err => new Error { 
                        Code = err.Code, 
                        Message = err.Message 
                    });

                    return new ConflictObjectResult(errors);                    
                }
                return result.Created("users");
            });            
        }        
    }
}