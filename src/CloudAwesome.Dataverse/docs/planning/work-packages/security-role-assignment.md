# Work Package: Security Role Assignment

## Goal

Stabilize team-role import/export and add pipeline-friendly audit output.

## Source Signals

- Current issue: #32 Optional output to console of teams and roles assigned.
- Legacy issue: assign security roles to teams.

## Current State

- `ExportSecurityRoles` can produce an import manifest for supplied teams.
- `ImportSecurityRoles` preloads roles by name and business unit, adds missing team roles, and removes surplus team roles.
- CLI output path override is validated and applied to the manifest before export.
- Unit tests cover export manifest output, no-op import, add-only import, remove-only import, mixed add/remove import, and duplicate role names across business units.

## Intended Behaviour

- Export current role assignments for named teams.
- Import target role assignments idempotently.
- Add missing roles and remove surplus roles only when this behaviour is explicitly accepted and documented.
- Optionally output final team-role assignments to console for pipeline logs.
- Treat duplicate role names across business units correctly.

## Bite-Sized Tasks

1. Complete: add tests for add-only, remove-only, no-op, and mixed diff cases.
2. Complete: add tests for duplicate roles in different business units.
3. Complete: fix CLI `--output-filepath` override behaviour.
4. Optional: extract role diff calculation into a unit-testable function if the import logic grows further.
5. Add optional audit output flag to manifest and CLI.
6. Add integration test against controlled test teams and roles.
7. Document destructive behaviour and safe pipeline usage.

## Acceptance Criteria

- Import is idempotent when Dataverse state already matches the manifest.
- CLI output override works.
- Optional final audit output lists effective team-role assignments.
- Tests prove surplus role removal behaviour.
