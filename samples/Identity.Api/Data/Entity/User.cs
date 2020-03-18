using System;
using System.ComponentModel.DataAnnotations;

namespace Identity.Api
{
    public class User
    {
        [Key]
        public Guid Guid { get; set; }
        public string Name { get; set; }
        public string LastName { get; set; }
        public string Email {get;set;}    
    }
}