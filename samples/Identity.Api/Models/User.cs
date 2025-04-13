using System;

namespace Identity.Api.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public string Name { get; set; }
        public string GoogleId { get; set; }
        public DateTime CreatedAt { get; set; }
    }
} 