# Code Review – Per Project Details

Date: 2025-10-28
Reviewer: Junie (JetBrains autonomous programmer)

## CloudAwesome.Dataverse.Core
Purpose
- Shared types: TracingHelper, LoggingConfiguration, serialization helpers, EarlyBound models, platform models.

Observations
- Pros: Centralizes concerns; EarlyBound models reduce mistakes.
- Cons: Core contains both pure infrastructure and Dataverse-bound models. Risk of Core being a dumping ground.
- TracingHelper: API is simple; but lacks structured properties/correlation IDs.
- SerialisationWrapper: Ensure consistent encoding and path handling; add overloads for streams.

Recommendations
- Separate infrastructure vs platform models with clear namespaces (Core.Infrastructure vs Core.EarlyBoundModels).
- Enable nullable reference types and analyzers.
- Add OperationContext and enrich logging with scopes.

## CloudAwesome.Dataverse.Customisation
Purpose
- Customization flows like environment variables, mailbox operations, WhoAmI helper, and SecurityRoleAssignment.

Observations
- SecurityRoleAssignment overlaps with Security project responsibilities; move security-specific logic.
- WhoAmI/TestAndEnableMailbox: useful samples; ensure they are invoked via CLI commands with clear descriptions.

Recommendations
- Keep Customisation focused on org-level settings and configs. Move security role export/import logic to Security project and keep only orchestration or convenience shells if needed.

## CloudAwesome.Dataverse.Processes
Purpose
- Plugin registration and management of processes/workflows.

Observations
- PluginRegistration has correctness bugs and namespace typos.
- Commented-out blocks for Custom APIs; entity image registration disabled accidentally due to inverted condition.
- Unregister stops processing after first missing assembly due to early return.

Recommendations
- Fix namespace import typo.
- Fix entity image loop condition and early return in Unregister.
- Introduce idempotent upsert behavior; feature flags for custom API registration.
- Extract Dataverse query/builders into dedicated classes to reduce method size.

## CloudAwesome.Dataverse.Security
Purpose
- Security role export/import between teams and business units.

Observations
- ExportSecurityRoles has good shape but lacks manifest validation and clearer distinction between team not found vs no roles.
- ImportSecurityRoles contains add/remove helpers; ensure business unit mismatches are handled clearly.

Recommendations
- Provide a diff mode to compare current vs manifest and output actions without applying.
- Validate manifests and normalize inputs (role names case-insensitive matching).
- Add retry policies on associate/disassociate operations.

## CloudAwesome.Dataverse.Cli
Purpose
- CLI entry point and commands using Spectre.Console.Cli.

Observations
- Program.cs wires branches; many placeholders remain.
- Lacks global exception handling and command descriptions/examples.
- ExportSecurityRolesCommand couples to Customisation.SecurityRoleAssignment instead of Security.ExportSecurityRoles service.

Recommendations
- Add global exception handling around cli.Run(args) and set non-zero exit codes.
- Add command descriptions and examples via Spectre attributes.
- Ensure commands call into the appropriate library services; remove cross-project coupling.
- Provide sample manifests for all available operations.

## Selected File-Level Notes
- Processes/PluginRegistration.cs
  - using CloudAwesome.Dataverse.Processe.Plugins; typo – should be Processes.
  - if (pluginStep.EntityImages.Any()) continue; inverted logic; should be if (!pluginStep.EntityImages.Any()) continue; then register images.
  - if (existingAssembly == null) return; should be continue; in Unregister loop.
- Cli/Commands/ExportSecurityRolesCommand.cs
  - Prefer calling Security.ExportSecurityRoles.Run(...) instead of Customisation.SecurityRoleAssignment.Export(...).
  - Resolve precedence between settings.OutputFilePath and manifest.OutputFilePath (CLI arg should override).

## Tooling and Quality Gates
- Add .editorconfig and StyleCop.Analyzers; enable warnings as errors for style and nullable.
- Configure CI to run dotnet build + dotnet test and produce artifacts; avoid committing bin/obj/nupkg.
- Consider a solution-level Directory.Build.props to centralize settings (nullable, analyzers, langversion).

## Documentation
- Create docs for each manifest with JSON samples.
- Provide a quickstart: connection setup, commands, examples.
