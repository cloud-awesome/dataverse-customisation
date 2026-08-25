# Dataverse Customisation Recovery Plan

Generated: 2026-08-25

This planning set captures the current state of the .NET 8 port, the deprecated `cds-customisation` baseline, the public feature backlog, and a staged plan for restoring confidence before new feature work continues.

## Sources Reviewed

- Current repository: <https://github.com/cloud-awesome/dataverse-customisation>
- Current open issues: <https://github.com/cloud-awesome/dataverse-customisation/issues>
- Deprecated repository: <https://github.com/apng1982/cds-customisation>
- Deprecated feature docs: <https://docs.cloudawesome.uk/cds-customisation/>
- Deprecated repo feature docs:
  - <https://raw.githubusercontent.com/apng1982/cds-customisation/master/documentation/features/plugin-registration/plugin-registration.md>
  - <https://raw.githubusercontent.com/apng1982/cds-customisation/master/documentation/features/generate-customisations/generate-customisations.md>
  - <https://raw.githubusercontent.com/apng1982/cds-customisation/master/documentation/features/toggle-process-status/toggle-process-status.md>
  - <https://raw.githubusercontent.com/apng1982/cds-customisation/master/documentation/features/bulk-deletion-jobs/bulk-deletion-jobs.md>
- Deprecated test inventory: <https://github.com/apng1982/cds-customisation/tree/master/src/CloudAwesome.Xrm.Customisation/CloudAwesome.Xrm.Customisation.Tests>

## Delivery Stages

### Stage 1: Testing Foundation

Goal: make the current port testable and establish a reliable quality gate before fixing or extending functionality.

Exit criteria:

- Test projects are added to the solution and run from `dotnet test`.
- Unit test harness supports Dataverse SDK request/response behaviour without a live environment.
- Integration test harness can be configured against a vanilla Dataverse environment but is opt-in locally and in CI.
- Regression tests exist for known high-risk behaviours listed in [current-state.md](current-state.md).
- CI runs build, unit tests, formatting/analyzer checks, and produces a clear artifact/report.

Primary docs:

- [testing-strategy.md](testing-strategy.md)
- [work-packages/foundation.md](work-packages/foundation.md)
- [public-interface-decisions.md](public-interface-decisions.md)

### Stage 2: Bug Fixes And Refactoring

Goal: refactor toward the intended behaviour, using tests as the specification rather than fitting tests around the current implementation.

Exit criteria:

- Known implementation bugs are fixed with regression coverage.
- Manifest loading, validation, connection handling, logging, and command execution are consistent across features.
- Public API boundaries are documented and stable enough for package publication.
- Nullable warnings and unimplemented runtime paths are either fixed or explicitly isolated behind unsupported-feature errors.

Primary docs:

- [current-state.md](current-state.md)
- [work-packages/foundation.md](work-packages/foundation.md)
- Feature-specific work packages in [work-packages](work-packages)

### Stage 3: Ported And New Functionality

Goal: progress legacy parity and new issue backlog only after the foundation is testable and functional.

Exit criteria:

- Ported features have unit, integration, and regression coverage.
- New features start with requirements, schema, CLI/API design notes, tests, and docs.
- NuGet publishing pipelines exist for CLI and API packages.
- Public documentation replaces the deprecated Docusaurus content.

Primary docs:

- [backlog-roadmap.md](backlog-roadmap.md)
- [work-packages/plugin-registration.md](work-packages/plugin-registration.md)
- [work-packages/customisation-generation.md](work-packages/customisation-generation.md)
- [work-packages/process-activation.md](work-packages/process-activation.md)
- [work-packages/security-role-assignment.md](work-packages/security-role-assignment.md)
- [work-packages/bulk-deletion-and-dependencies.md](work-packages/bulk-deletion-and-dependencies.md)
- [work-packages/documentation-generation.md](work-packages/documentation-generation.md)
- [work-packages/power-pages-and-projectops.md](work-packages/power-pages-and-projectops.md)
- [work-packages/publishing-and-docs.md](work-packages/publishing-and-docs.md)

## Operating Rules For Future Sessions

- Do not start new feature work until Stage 1 and Stage 2 exit criteria are met, except for documentation or issue triage needed to unblock those stages.
- For each feature, write or port tests against intended behaviour first.
- Treat the deprecated repo as a requirements and regression source, not as code to copy blindly.
- Prefer small work packages that can be completed, built, and tested in one session.
- Keep integration tests opt-in and environment-safe. Any test that mutates Dataverse must use a clearly named solution/publisher prefix and have cleanup guidance.

## Open Questions

- Which test harness should be standard for SDK-level unit tests in the modern .NET 8 repo: a current FakeXrmEasy package, CloudAwesome.Dataverse.Simulate, or a thin in-repo fake around `IOrganizationService`?
- What authentication profile should the vanilla Dataverse integration environment support first: app registration, interactive user auth, or both?
- Should the public package split be one CLI tool plus one API package, or multiple API packages matching the current project split?
- Should schemas be published at `schema.cloudawesome.xyz`, through GitHub Pages, or through the future docs site build?
