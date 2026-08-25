# Public Interface Decisions

Generated: 2026-08-25

These decisions are intentionally narrow. They exist to keep the foundation work consistent while the project is brought back under test.

## Package Shape

Initial package target:

- one .NET tool package for `dvcli`;
- one or more API packages only after feature boundaries are validated by tests.

Do not publish API packages until the public service classes have stable constructor/dependency patterns and manifest validation behaviour.

## CLI Contract

CLI commands should follow the same pattern across features:

- accept `--manifest` as a file path, not as a complex object bound directly by `Spectre.Console.Cli`;
- allow a small set of no-manifest options for common workflows only where the issue backlog calls for it;
  - The definition of whether a command *requires*, *supports*, or has no need for a manifest is defined by the use of  the CommandInterfaces used on each command's `CommandSettings`.
- return `0` on success and non-zero on validation, connection, or execution failure;
- fail before connecting to Dataverse when required local inputs are missing or invalid;
- never silently route a user-facing command to `PlaceholderCommand`;
- keep test/debug commands under a clearly named non-production branch until they are removed or promoted.

## Manifest Contract

Manifests should be JSON-first in the modern repo.

Rules:

- every feature manifest should have a generated sample manifest task before or alongside the mutating command;
- every feature manifest should have a JSON schema before release;
- manifest validation should happen before any Dataverse mutation;
- unsupported legacy XML nodes should be rejected with clear validation errors rather than ignored;
- CLI connection arguments may override manifest connection settings, but the override precedence must be explicit and tested.

## API Contract

Feature APIs should be testable without a live Dataverse environment.

Rules:

- business logic should accept `IOrganizationService` or a small abstraction that can be faked in unit tests;
- command classes should do only CLI concerns: option validation, manifest loading, connection creation, tracing setup, and process invocation;
- Dataverse mutation orchestration should live outside CLI projects;
- feature classes should return enough structured result data for CLI output and future pipeline usage, not only write to logs.

## Logging And Errors

- Logging should be observable in tests without redirecting global console state where avoidable.
- Validation errors should be collected and shown together when practical.
- Runtime `NotImplementedException` paths should be replaced by explicit unsupported-feature validation errors before release.

## Integration Contract

Integration tests are opt-in.

Required environment variables should use the `CA_DATAVERSE_` prefix and be documented beside the integration test project. Tests that mutate Dataverse must create or use clearly named disposable artifacts and must describe cleanup.

