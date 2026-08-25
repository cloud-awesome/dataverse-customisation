# Work Package: Process Activation

## Goal

Make process activation/deactivation correct, testable, and broad enough for the current process backlog.

## Source Signals

- Current issues: #1, #14, #15, #16, #17, #27, #31.
- Deprecated docs: plugin steps, workflows, modern flows, and record creation rules.

## Current State

- The current implementation handles solution-level flags for flows, plugin steps, and SLAs.
- Manifest properties exist for plugin assemblies, solutions, entities, workflows, modern flows, and record creation rules.
- Only `Solutions` are processed.
- Plugin-step processing appears to retrieve solution component object IDs as `Workflow`, which is likely incorrect.
- There is no process activation test coverage.

## Intended Behaviour

- Activate/deactivate all selected process types from a manifest.
- Support solution-level selection and explicit listed records.
- Support plugin steps by solution and by assembly.
- Support listed cloud flows.
- Support solution business rules.
- Support BPFs by solution.
- Support SLAs and optional default SLA setting.
- Generate a full blank manifest for user discovery.
- Publish a JSON schema for manifest validation.

## Bite-Sized Tasks

1. Add tests for target state/status mapping for flows, plugin steps, and SLAs.
2. Add regression test proving plugin-step processing targets `sdkmessageprocessingstep`.
3. Add tests for solution component retrieval and empty result behaviour.
4. Implement listed cloud flow support.
5. Implement plugin steps by assembly.
6. Implement business rules by solution.
7. Implement BPFs by solution.
8. Implement process activation manifest generation.
9. Add JSON schema generation/publication.
10. Add live integration tests with disposable flow/plugin step/SLA artifacts where practical.

## Acceptance Criteria

- Unit tests cover each supported process type's target state/status.
- CLI supports useful no-manifest operation for common solution-level operations.
- Manifest schema validates all supported nodes.
- Integration tests prove at least flow, plugin step, and SLA activation/deactivation in a vanilla Dataverse environment.

