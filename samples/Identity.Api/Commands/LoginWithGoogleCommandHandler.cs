using System;
using System.Threading.Tasks;
using System.Security.Claims;
using Quantic.Core;
using Identity.Api.Models;
using Identity.Api.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using Google.Apis.Auth;

namespace Identity.Api.Commands
{
    public class LoginWithGoogleCommandHandler : ICommandHandler<LoginWithGoogleCommand>
    {
        private readonly IdentityDbContext _dbContext;

        public LoginWithGoogleCommandHandler(IdentityDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<CommandResult> Handle(LoginWithGoogleCommand command, RequestContext context)
        {
            try
            {
                // Validate Google token and get user info
                var payload = await ValidateGoogleToken(command.IdToken);
                
                // Check if user exists
                var user = await _dbContext.Users
                    .FirstOrDefaultAsync(u => u.Email == payload.Email);

                if (user == null)
                {
                    // Create new user if doesn't exist
                    user = new User
                    {
                        Email = payload.Email,
                        Name = payload.Name,
                        GoogleId = payload.Subject,
                        CreatedAt = DateTime.UtcNow
                    };

                    await _dbContext.Users.AddAsync(user);
                    await _dbContext.SaveChangesAsync();
                }

                // Return user information and tokens
                return new CommandResult(new { 
                    UserId = user.Id,
                    Email = user.Email,
                    Name = user.Name,
                    AccessToken = command.AccessToken,
                    IdToken = command.IdToken
                });
            }
            catch (Exception ex)
            {
                return new CommandResult(new Failure("LOGIN_FAILED", ex.Message));
            }
        }

        private async Task<GoogleJsonWebSignature.Payload> ValidateGoogleToken(string idToken)
        {
            var settings = new GoogleJsonWebSignature.ValidationSettings()
            {
                Audience = new[] { "YOUR_GOOGLE_CLIENT_ID" } // Replace with your Google Client ID
            };

            return await GoogleJsonWebSignature.ValidateAsync(idToken, settings);
        }
    }
} 