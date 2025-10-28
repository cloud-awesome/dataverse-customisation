# Backlog – Features, Refactors, and Bug Fixes

Date: 2025-10-28
Owner: CloudAwesome.Dataverse

Legend
- Priority: P1 (urgent), P2 (important), P3 (nice-to-have)
- Effort: S (<= 0.5d), M (1–2d), L (3–5d), XL (> 1w)

## P1 – Urgent
1. Fix PluginRegistration defects (Processes)
   - Effort: S
   - Details: Correct namespace typo; fix entity image loop condition; change early return to continue in Unregister.
   - Outcome: Reliable plugin registration/unregistration.

2. Decouple CLI from Customisation for Security export/import (Cli + Security)
   - Effort: M
   - Details: Update ExportSecurityRolesCommand and ImportSecurityRolesCommand to use Security library services (ExportSecurityRoles.Run, ImportSecurityRoles.Run). Keep Customisation.SecurityRoleAssignment as optional wrapper or deprecate.
   - Outcome: Clear boundaries; easier maintenance.

3. Add manifest validation and schema versioning (All feature libs)
   - Effort: M
   - Details: Introduce FluentValidation validators per manifest, add SchemaVersion field, validate before execution.
   - Outcome: Early feedback on misconfigurations; safer operations.

4. Global exception handling and exit codes in CLI (Cli)
   - Effort: S
   - Details: Wrap cli.Run(args) with try/catch, print friendly errors, set non-zero exit code; add --verbose to increase detail.
   - Outcome: Better UX and scripting reliability.

5. Remove committed build artifacts (repo hygiene)
   - Effort: S
   - Details: Purge bin/ obj/ nupkg/ folders from source control; update .gitignore to exclude.
   - Outcome: Cleaner repo; smaller diffs.

## P2 – Important
6. Introduce OperationContext and structured logging (Core)
   - Effort: M
   - Details: Add correlation IDs; enrich TracingHelper with scopes; support ILogger abstractions.
   - Outcome: Traceable operations and better diagnostics.

7. Idempotent upsert for Processes (Processes)
   - Effort: L
   - Details: Detect existing plugin assemblies, types, steps, and images; update if changed. Provide flags: --clobber, --update-assembly-only.
   - Outcome: Safer repeatable deploys.

8. Security import diff and dry-run mode (Security)
   - Effort: M
   - Details: Compute differences between manifest and current state; support --what-if to log actions.
   - Outcome: Transparent changes before applying.

9. Retry policies for Dataverse operations (Core + Feature libs)
   - Effort: M
   - Details: Add Polly-based transient fault handling for Retrieve/Associate/Disassociate/Execute.
   - Outcome: Resilience to transient failures.

10. Testing foundation (Solution)
   - Effort: L
   - Details: Add unit test project; mock IOrganizationService; tests for manifest validation, mapping, and core flows. Add optional integration tests guarded by env vars.
   - Outcome: Prevent regressions.

## P3 – Nice-to-have
11. CLI polish: command descriptions and examples (Cli)
   - Effort: S
   - Details: Use Spectre.Console.Cli attributes and usage examples; add sample manifests in docs/Samples.
   - Outcome: Better discoverability.

12. Style and analyzers (Solution)
   - Effort: S
   - Details: Add .editorconfig, StyleCop.Analyzers, Roslynator; enable nullable and warnings-as-errors incrementally.
   - Outcome: Consistent code quality.

13. Repositories/services abstraction for Dataverse (Core + Feature libs)
   - Effort: L
   - Details: Wrap common QueryExpression patterns; expose higher-level services (e.g., ITeamRoleService).
   - Outcome: Cleaner code and testability.

14. Documentation and diagrams (Docs)
   - Effort: S
   - Details: Provide architecture diagrams (Mermaid), quickstart, and per-feature manifest samples.
   - Outcome: Onboarding acceleration.

15. Packaging and CI versioning (CI/CD)
   - Effort: M
   - Details: Set AssemblyInformationalVersion from CI; publish CLI as a tool; avoid committing artifacts.
   - Outcome: Predictable releases.

## Bugs/Tasks Cross-reference
- Processes/PluginRegistration.cs: fix using typo, entity image condition, Unregister return.
- Cli/Commands/ExportSecurityRolesCommand.cs: switch to Security.ExportSecurityRoles.
- Security/ExportSecurityRoles.cs: clarify warnings; add validations.
- Program.cs: add global exception handling.

## Notes
- Prioritize changes with highest correctness impact (P1) before adding new features.
- Adopt small PRs with focused scope and tests for each item.
