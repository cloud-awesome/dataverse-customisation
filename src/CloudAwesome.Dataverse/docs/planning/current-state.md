# Current State Snapshot

Generated: 2026-08-25

## Current Solution

The local solution builds on .NET 8 and contains these production projects:

- `CloudAwesome.Dataverse.Core`
- `CloudAwesome.Dataverse.Customisation`
- `CloudAwesome.Dataverse.Processes`
- `CloudAwesome.Dataverse.Security`
- `CloudAwesome.Dataverse.Cli`
- `CloudAwesome.Dataverse.PowerPages`
- `CloudAwesome.Dataverse.ProjectOps`

There is a `CloudAwesome.Dataverse.Cli.Test` directory in the checkout, but it is not included in `CloudAwesome.Dataverse.sln` and no test `.csproj` was found during the scan.

The current repo has 18 open GitHub issues. The deprecated repo has a richer legacy codebase and a test project covering serialization, model defaults, validation, plugin registration, service endpoint registration, and customisation helpers.

## Build Baseline

Command run:

```powershell
dotnet build CloudAwesome.Dataverse.sln --no-restore
```

Result:

- Build succeeded.
- 97 compiler warnings were emitted.
- Warnings are mostly nullable warnings, but include behavioural risk signals:
  - always-false `Guid` null checks in `EntityExtensions`;
  - possible null dereferences in plugin registration paths;
  - possible null return from JSON deserialization;
  - uninitialized manifest/model properties despite nullable being enabled.

## Current Feature Surface

### Implemented Or Partially Implemented

- Customisation generation:
  - Current code processes entities and attributes.
  - Global option sets, security roles, model-driven apps, forms, and views are still TODO or missing.
- Plugin registration:
  - Core registration classes exist for assemblies, plugin types, steps, and entity images.
  - CLI commands are currently wired to placeholders.
  - Custom APIs and service endpoints are not active in the port.
- Process activation:
  - Current code supports solution-level flags for flows, plugin steps, and SLAs.
  - Manifest properties exist for assemblies, entities, workflows, modern flows, and record creation rules, but those paths are not implemented.
- Security role team assignment:
  - Export and import code exists for team-role mappings.
  - Current issues request optional console audit output.
- Environment variable update:
  - CLI/API code exists for setting environment variable values.
- Power Pages and ProjectOps:
  - Projects exist but are currently empty shells.

### CLI State

The CLI is based on `Spectre.Console.Cli` and is packaged as a .NET tool named `dvcli`.

Current command branches include:

- `plugins register/unregister`
- `customisations generate/set-environment-variable`
- `processes activate/deactivate`
- `security export/import`
- placeholder branches for dependencies, document, and project-ops
- test commands for `who-am-i` and token retrieval

Several branches still use `PlaceholderCommand`. Plugin registration has a `PluginRegistrationCommand`, but it currently only writes `Just testing...` and is not wired in `Program.cs`.

## Known Implementation Risks To Convert Into Regression Tests

These are not complete bug reports, but they are concrete enough to drive Stage 1 tests and Stage 2 fixes.

### CLI Connection Handling

- `SupportsDataverseConnection.UserPassword` sets `ConnectionDetails.UserName` instead of `ConnectionDetails.Password`.
- `DataverseConnectionType.UserNameAndPassword` throws `NotImplementedException`, matching issue #28 but currently exposing a runtime path rather than a clear unsupported-auth message.
- Manifest-supplied connection settings and CLI overrides are inconsistent across commands.

### Plugin Registration

- `Program.cs` maps `plugins register` and `plugins unregister` to `PlaceholderCommand`, not the plugin registration command.
- `PluginRegistrationCommand` does not load a manifest or call `PluginRegistration.Register`.
- `PluginRegistration.Register` has this likely inverted guard:

```csharp
if (pluginStep.EntityImages.Any()) continue;
foreach (var entityImage in pluginStep.EntityImages)
```

This skips entity image creation when images are present.

- `RegisterPluginAssembly.Run` can return null according to the build warning, but callers assume a valid assembly reference.
- Custom API registration is commented out.
- Service endpoints, webhooks, and workflow assemblies from the old manifest are not ported.

### Process Activation

- `ProcessPluginSteps` retrieves each solution component object as `Workflow`, even though the component type is `SdkMessageProcessingStep`. It should target the plugin step entity when processing plugin steps.
- Manifest properties for listed workflows, flows, record creation rules, entities, and plugin assemblies are currently unused.
- BPF, business rule, listed-flow, and plugin-by-assembly requirements are open issues.

### Environment Variables

- When creating a missing environment variable value, `SetEnvironmentVariable.Run` returns immediately, so a manifest containing multiple variables stops after the first create.
- Existing value matching queries `environmentvariablevalue` records by `schemaname`, which is likely a definition field rather than a value field unless joined/aliased. This needs a unit test plus live integration confirmation.

### Security Role Assignment

- Export command validates `--output-filepath` but does not apply the CLI override back onto the deserialized manifest before running export.
- Import removes surplus roles by design. This is useful but destructive enough to require explicit tests and documentation.
- There is no final audit output option yet, matching issue #32.

### Core Utilities

- Several logger `Dispose` methods throw `NotImplementedException`.
- `EntityExtensions` contains unimplemented paths and always-false null checks on `Guid`.
- `SerialisationWrapper.DeserialiseJsonFromFile<T>` can return null but its signature returns non-null `T`.
- `ComponentType` is marked as likely out of date.

## Deprecated Baseline

The deprecated repo provides:

- `CloudAwesome.Xrm.Customisation.Cli`
- `CloudAwesome.Xrm.Customisation`
- `CloudAwesome.Xrm.Customisation.Tests`
- `CloudAwesome.Xrm.Customisation.IntegrationTests`
- `SamplePluginAssembly`
- JSON schemas and sample XML manifests.

The old tests used NUnit, FluentAssertions, FakeXrmEasy, FakeItEasy, FluentValidation, NJsonSchema, and `System.IO.Abstractions.TestingHelpers`.

Useful old test groups to port or re-express:

- customisation extension helpers;
- serialization from manifest files;
- model defaults for plugin assemblies, plugins, steps, entity images, custom APIs, request parameters, and response properties;
- model validation for entities, attributes, plugin assemblies, plugins, steps, and images;
- plugin manifest validation;
- plugin registration happy paths, clobber, update-assembly-only, missing assembly, logging, service endpoints, and custom APIs.

