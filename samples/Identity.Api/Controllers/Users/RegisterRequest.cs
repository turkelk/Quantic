using System.ComponentModel.DataAnnotations;

namespace Identity.Api.Controllers.Users
{
    public class RegisterRequest
    {
        [Required]
        public string Password { get; set; }
        [Required]
        public string Email { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string LastName { get; set; }        
    }
}