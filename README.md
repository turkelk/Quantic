## Quantic

Quantic is a lightweight **CQRS** framework for .NET that helps you build Web APIs with a consistent handler pipeline and optional cross‑cutting “decorators” (logging, caching, validation, tracing, etc.).

This repo contains the core library plus optional packages and a set of unit tests.

## Packages in this repo

- **`Quantic.Core`**: CQRS abstractions (`ICommand`, `IQuery<T>`, handlers, `RequestContext`, `CommandResult`/`QueryResult<T>`) and core plumbing.
- **`Quantic.Log`**: request/response logging decorators for command/query handlers, with configurable redaction.
- **`Quantic.Cache.InMemory` / `Quantic.Cache.Redis`**: caching decorators.
- **`Quantic.Validation`**: validation helpers/decorators.
- **`Quantic.Ef`**: EF helpers.
- **`Quantic.Web`**: web helpers / composition.
- **`Quantic.FeatureManagement`**: feature toggles.
- **`Quantic.Trace.Elastic.Apm`**: tracing integration.
- **`Quantic.MassTransit.RabbitMq`**: messaging integration.

## Requirements

- **.NET SDK**: the solution targets modern .NET (tests currently run on `net9.0`).

## Build

```bash
dotnet build Quantic.sln -c Release
```

## Run tests

All tests:

```bash
dotnet test Quantic.sln -c Release
```

Single project (example):

```bash
dotnet test test/Quantic.Log.UnitTest/Quantic.Log.UnitTest.csproj -c Release
```

## Minimal CQRS example

Define a command + handler:

```csharp
using System.Threading.Tasks;
using Quantic.Core;

public sealed class CreateUser : ICommand
{
    public string Email { get; set; } = "";
}

public sealed class CreateUserHandler : ICommandHandler<CreateUser>
{
    public Task<CommandResult> Handle(CreateUser command, RequestContext context)
    {
        // do work...
        return Task.FromResult(CommandResult.Success);
    }
}
```

Define a query + handler:

```csharp
using System.Threading.Tasks;
using Quantic.Core;

public sealed class GetUserByEmail : IQuery<UserDto?>
{
    public string Email { get; set; } = "";
}

public sealed class GetUserByEmailHandler : IQueryHandler<GetUserByEmail, UserDto?>
{
    public Task<QueryResult<UserDto?>> Handle(GetUserByEmail query, RequestContext context)
    {
        // read...
        return Task.FromResult(new QueryResult<UserDto?>(null));
    }
}
```

## Logging (Quantic.Log)

`Quantic.Log` provides handler decorators that log command/query request + response objects via `IRequestLogger`.

### Field redaction (avoid logging secrets / payload bytes)

You can configure redaction globally and per handler type name:

```csharp
using Quantic.Log;

var logSettings = new LogSettings
{
    // applied to ALL requests/responses
    GlobalRedactProperties = new[] { "ApiKey", "AccessToken", "Password", "Base64", "Bytes" },
    RedactionMask = "***",

    // per-command/query overrides
    Settings = new[]
    {
        new LogSetting
        {
            Name = "MySensitiveCommand",
            RedactRequestProperties = new[] { "Secret", "Token" },
            RedactResponseProperties = new[] { "RawResponse" }
        }
    }
};
```

Redaction is **recursive** and matches property names **case-insensitively**.

## Repository layout

- `src/`: Quantic libraries
- `test/`: unit test projects
- `samples/`: example applications

## Contributing

Open `Quantic.sln`, make changes in `src/`, and add/update tests under `test/`. Keep changes small and include unit tests for new behavior.

