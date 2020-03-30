using System;
using System.Collections.Generic;

namespace Quantic.Core
{
    public class CommandResult : Result
    {
        public Guid Guid { get; }

        public static readonly CommandResult Success = new CommandResult(SuccessMessage);
        public static CommandResult WithError(string errorCode, bool retry = false)
        {
            return new CommandResult(new Failure(errorCode), retry);
        }
        public static CommandResult WithError(string errorCode, string errorMessage, bool retry = false)
        {
            return new CommandResult(new Failure(errorCode, errorMessage), retry);
        }

        public static CommandResult WithError(Failure error, bool retry = false)
        {
            return new CommandResult(error, retry);
        }
        public static CommandResult WithError(IList<Failure> errors, bool retry = false)
        {
            return new CommandResult(errors, retry);
        }        
        public static CommandResult WithGuid(Guid guid)
        {
            return new CommandResult(guid);
        }
        public static CommandResult WithCode(string code)
        {
            return new CommandResult(code);
        }
        public static CommandResult WithMessage(string code, string message)
        {
            return new CommandResult(code, message);
        }

        public CommandResult(IList<Failure> errors, bool retry = false)
            : base(errors, retry)
        {
        }

        public CommandResult(Failure error, bool retry = false)
            : base(error, retry)
        {
        }

        public CommandResult(string code, string message = default)
            : base(code, message)
        {
        }

        public CommandResult(Guid guid)
            : base(SuccessMessage)
        {
            Guid = (guid == Guid.Empty) ? throw new ArgumentNullException(nameof(guid)) : guid;
        }

        public CommandResult(Guid guid, string code, string message = default)
            : base(code, message)
        {
            Guid = (guid == Guid.Empty) ? throw new ArgumentNullException(nameof(guid)) : guid;
        }
    }
}
