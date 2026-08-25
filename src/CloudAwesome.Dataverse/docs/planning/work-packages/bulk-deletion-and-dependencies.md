# Work Package: Bulk Deletion Jobs And Dependency Reports

## Goal

Re-triage and port non-solution-aware migration and dependency inspection features after core foundations are stable.

## Source Signals

- Current issue: #10 List missing dependencies from an exported solution.
- Deprecated docs: bulk deletion jobs are not solution aware, so the old API/CLI migrated referenced jobs from source to target.
- Legacy issue: get missing dependencies for an exported solution file.

## Current State

- No current project contains a visible bulk deletion job implementation.
- `dependencies` exists as a CLI branch but only contains a placeholder command.
- Missing dependency reporting has a current issue and should probably be prioritised before older bulk deletion migration because it supports local and PR validation workflows.

## Intended Behaviour

- Dependency report:
  - inspect one exported solution zip;
  - identify missing dependencies with machine-readable and human-readable output;
  - return non-zero or structured results when missing dependencies exist, depending on command mode;
  - support pipeline usage without needing a live Dataverse environment if the exported solution contains enough data.
- Bulk deletion migration:
  - export selected bulk deletion jobs from a source Dataverse environment;
  - import or recreate them in a target Dataverse environment;
  - avoid accidental deletion or execution of jobs during migration;
  - support dry-run/audit output.

## Bite-Sized Tasks

1. Define output contract for missing dependency report: console, JSON file, process exit codes.
2. Add parser tests using small solution zip fixtures.
3. Implement `dependencies missing` or equivalent CLI command.
4. Add pipeline-focused documentation for PR validation.
5. Re-read deprecated bulk deletion implementation and document Dataverse entities/messages involved.
6. Add manifest/schema for bulk deletion migration.
7. Implement read-only export first.
8. Implement target import with dry-run mode.
9. Add live integration test against a vanilla environment only after cleanup rules are clear.

## Acceptance Criteria

- Missing dependency command can be used in CI to fail a PR when dependencies are present.
- Output can be consumed programmatically.
- Bulk deletion migration has a dry-run mode before any mutating import is released.

