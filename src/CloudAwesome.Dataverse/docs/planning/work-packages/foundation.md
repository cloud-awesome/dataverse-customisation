# Work Package: Foundation, Testing, And Public Interface

## Goal

Create a testable, consistent foundation before feature work continues.

## Current State

- No test project is included in the current solution.
- Build succeeds but produces 97 warnings.
- CLI command and manifest patterns vary by feature.
- Some runtime paths throw `NotImplementedException`.
- Issue #33 explicitly calls for requirements and tests before further functionality.
- Issue #8 calls for design decisions for the public interface.

## Deliverables

- Add test projects and include them in the solution.
- Decide and document SDK simulation strategy.
- Add a shared test utilities project only if duplication becomes meaningful.
- Add initial tests for serialization, connection settings, logging, and CLI command settings.
- Document public API boundaries:
  - CLI command contract;
  - manifest contract;
  - service/API contract;
  - package boundaries.
- Create a warning triage list and decide which warnings become errors.
- Add first GitHub Actions pipeline for build and unit tests.

## Bite-Sized Tasks

1. Add `CloudAwesome.Dataverse.Core.Tests` with serialization and connection option tests.
2. Add `CloudAwesome.Dataverse.Cli.Tests` with command settings tests.
4. Add regression tests for the current known bugs that do not need a live Dataverse environment.
5. Write `docs/planning/public-interface-decisions.md` covering API/CLI/manifest boundaries.
6. Add CI for restore, build, and unit tests.
7. Add an opt-in integration test project skeleton with environment variable requirements.

## Acceptance Criteria

- `dotnet test` runs locally without a live Dataverse environment.
- CI runs tests on every push/PR.
- At least one regression test fails against current implementation before the Stage 2 fix.
- Public interface decisions are explicit enough to guide feature work without re-litigating the same questions.

## Dependencies

- Network access to restore any new test packages.

