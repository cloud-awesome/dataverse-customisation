# CloudAwesome.Dataverse – Architecture Review

Date: 2025-10-28
Reviewer: Junie (JetBrains autonomous programmer)

Overview
The solution targets Dataverse automation with two entry modes: a Spectre.Console-based CLI and programmatic use via project libraries (.Security, .Processes, .Customisation, .Core). Overall layering is sensible: Core provides shared utilities/models; feature libraries implement domain operations; CLI wires commands to those operations. The codebase is early-stage with workable patterns but has inconsistencies and several correctness bugs to address.

Strengths
- Clear separation of concerns between CLI and libraries. CLI avoids duplicating logic and mostly delegates to libraries.
- Use of TracingHelper abstraction to standardize logging across libraries and CLI; configurable log level.
- Manifest-driven operations (e.g., ExportSecurityRolesManifest) encourage repeatable, declarative workflows.
- Typed EarlyBound models for Dataverse entities reduce magic strings in queries.

Key Architectural Observations & Risks
1. Inconsistent library entry points and coupling to CLI types
   - Some CLI commands call processes in Customisation instead of dedicated feature libraries (e.g., ExportSecurityRolesCommand uses Customisation.SecurityRoleAssignment rather than Security.ExportSecurityRoles).
   - Settings classes sometimes mix CLI concerns (arguments) with domain defaults (e.g., OutputFilePath duplication between CLI and manifest). This causes ambiguity about precedence.
   Recommendation: Make CLI a thin layer. Each command should invoke a single public application service in its library (e.g., Security.ExportSecurityRoles.Run). Enforce input normalization in the CLI before invoking domain services.

2. Trace/logging propagation
   - TracingHelper is passed explicitly per method. While explicit is good, it leads to plumbing. There’s no consistent context (operation ID, correlation ID, environment) propagated.
   Recommendation: Introduce an OperationContext (request ID, environment, solution name) and extend TracingHelper to accept scopes. Consider Microsoft.Extensions.Logging.Abstractions and scopes to standardize structured logs.

3. Error handling strategy
   - Methods largely return silently or log and continue on critical issues (e.g., assembly not found). In Unregister, a null check returns early, skipping the rest of the assemblies.
   Recommendation: Adopt a consistent fail-fast vs continue-on-error policy per command flag (e.g., --continue-on-error). Throw domain-specific exceptions from libraries and let CLI decide exit codes.

4. Manifest validation and schema
   - Manifests are deserialized and used with minimal validation. Properties like manifest.SolutionName, UpdateAssemblyOnly, Clobber significantly alter behavior but are not validated.
   Recommendation: Add validators (FluentValidation) per manifest; add schema versioning to manifests.

5. Naming and namespaces consistency
   - E.g., CloudAwesome.Dataverse.Processe.Plugins (typo) vs .Processes.Plugins in PluginRegistration.cs using directives. Such inconsistencies cause runtime or compile-time issues and erode maintainability.
   Recommendation: Enforce EditorConfig and StyleCop/Analyzers. Add solution-wide namespace validation; enable nullable reference types.

6. Abstraction boundaries for Dataverse
   - Direct use of IOrganizationService and raw QueryExpression in application services. This is acceptable but produces repetitive patterns and complex testability.
   Recommendation: Wrap common Dataverse operations in repository/services (e.g., ITeamRoleService). Keep EarlyBound entities but expose higher-level methods.

7. Testing strategy
   - No unit or integration tests evident. Risky for Dataverse operations. Early-bound queries can be validated with mocks.
   Recommendation: Introduce unit tests for manifest validation and mapping; add integration test harness behind an environment-variable guard to connect to a sandbox.

8. CLI UX and discoverability
   - Commands grouped sensibly, but some branches have placeholders. Missing help text/command descriptions and examples. No top-level --version or --help overrides visible.
   Recommendation: Annotate commands with descriptions, examples, and validation; provide sample manifests for each feature.

9. Idempotency and re-runs
   - Plugin registration supports clobber, but custom APIs registration is commented out; entity image registration loop has a logic error preventing images from registering.
   Recommendation: Clarify idempotent behavior: detect and update existing resources without clobber where safe.

10. Packaging and versioning
   - CLI has packaged .nupkg artifacts in repo. Versioning via AssemblyInformationalVersion is read; ensure CI builds set this value consistently. Avoid committing built artifacts.

Concrete Code Findings (selected)
- PluginRegistration.cs
  - Using directive typo: using CloudAwesome.Dataverse.Processe.Plugins; should be .Processes.
  - Logic bug: if (pluginStep.EntityImages.Any()) continue; prevents any entity images from registering. Should be if (!pluginStep.EntityImages.Any()) continue; then loop.
  - Unregister early return: if (existingAssembly == null) return; should continue; to process other assemblies.
  - TODOs for Custom APIs are commented with large blocks; consider feature flags to include/exclude.
- ExportSecurityRolesCommand.cs
  - Delegates to Customisation.SecurityRoleAssignment.Export instead of Security.ExportSecurityRoles.Run, creating cross-package coupling; use the Security library for export/import.
  - OutputFilePath precedence unclear between CLI and manifest. Decide rule: CLI arg overrides manifest; apply before calling service.
- ExportSecurityRoles.cs
  - Good use of AliasedValue and EarlyBound models. Consider projecting directly via ColumnSet(new[] { Name }) and null checks.
  - Warning message conflates not found vs no roles. You already query TeamRoles, so team existence is not strictly validated. Optionally fetch Team entity to validate ID.
- Program.cs (CLI)
  - commands for plugins are placeholders; if plugin registration is supported in libraries, wire them in with proper settings.
  - Missing top-level exception handler to convert unhandled exceptions into non-zero exit code with friendly message.

Cross-cutting Recommendations
- Enable Nullable Reference Types (nullable enable) in all projects; fix warnings.
- Introduce style analyzers (StyleCop.Analyzers, Roslynator) and an .editorconfig for naming, spacing, and import ordering.
- Centralize Dataverse connection handling with retry policies (Polly) for transient faults.
- Add a DomainErrors static class with well-known error codes/messages for consistency.
- Introduce a lightweight options pattern for library operations (e.g., ProcessActivationOptions) instead of passing many scalar arguments.

Proposed Project Roles
- CloudAwesome.Dataverse.Core: Shared infrastructure (tracing, serialization, models, option types). Avoid domain logic here.
- CloudAwesome.Dataverse.Customisation: Operations that modify Dataverse metadata/config (mailboxes, env variables); keep them isolated from security and processes.
- CloudAwesome.Dataverse.Processes: Workflow/plugin registration; focus on idempotent registration and clean unregister with dependency management.
- CloudAwesome.Dataverse.Security: Role and team management; import/export, diff, and reconcile.
- CloudAwesome.Dataverse.Cli: Wiring, validation, output formatting, examples.

Security & Secrets
- Ensure CLI never logs access tokens or secrets. Scrub logs. Provide --verbose to opt-in to additional diagnostic output.

Documentation
- Add docs/ samples for each manifest type and a quickstart for the CLI. Provide architecture diagrams (mermaid) for flows.

