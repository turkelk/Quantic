using System;
using Quantic.Core;

namespace Notification.Api
{
    public class SaveUserCommand : ICommand
    {
        public string Email {get;set;}
        public Guid Guid {get;set;} 
    }
}