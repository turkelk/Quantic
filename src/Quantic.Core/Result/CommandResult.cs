using System;
using System.Collections.Generic;

namespace Quantic.Core
{
    public class CommandResult : Result
    {
        public Guid Guid { get; }

        public static readonly CommandResult Success = new CommandResult(SuccessMessage);

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
