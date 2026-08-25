# Work Package: Documentation Generation

## Goal

Rebuild documentation generation as a tested feature after core Dataverse query and manifest foundations are reliable.

## Source Signals

- Deprecated docs advertised metadata and system configuration documentation as PDF or Markdown, including entity relationship diagrams, security roles, workflows/cloud processes, and Power Pages site documentation.
- Deprecated repo contains `DocumentationGenerator` and `PortalDocumentationGenerator` classes.
- The maintainer wants proper documentation to replace the old Docusaurus site, but generated customer/system docs are a separate product feature from project docs.

## Current State

- No current documentation generation project or CLI command is active.
- CLI contains a `document` branch with a placeholder command.
- The old public Docusaurus documentation is still the main public reference for this feature.

## Intended Behaviour

- Generate environment documentation from Dataverse metadata and configuration.
- Support Markdown first, then PDF once content and layout are stable.
- Include:
  - tables/entities and columns;
  - relationships;
  - security roles;
  - workflows/cloud processes;
  - Power Pages configuration where relevant;
  - generated diagrams if a supported renderer is selected.
- Keep generated docs deterministic enough for regression tests.

## Bite-Sized Tasks

1. Inventory old `DocumentationGenerator` and `PortalDocumentationGenerator` behaviour.
2. Define v1 output format and folder structure.
3. Add tests for Markdown generation from fake metadata inputs.
4. Add Dataverse metadata query abstraction.
5. Implement entity/table documentation.
6. Implement relationship documentation.
7. Implement security role documentation.
8. Implement workflow/cloud process documentation.
9. Evaluate diagram generation library and CI compatibility.
10. Add public feature docs and sample output.

## Acceptance Criteria

- Given deterministic metadata input, generated Markdown is stable and snapshot-testable.
- CLI can generate docs for a small vanilla environment.
- PDF support is not started until Markdown output is stable.

