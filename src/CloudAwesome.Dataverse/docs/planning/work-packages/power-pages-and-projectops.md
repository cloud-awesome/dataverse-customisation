# Work Package: Power Pages And ProjectOps

## Goal

Treat Power Pages and Project Operations as later-stage feature areas with separate discovery, requirements, and integration plans.

## Source Signals

- Deprecated docs advertised Power Pages content generation from a Visio diagram, including web pages and basic forms.
- Deprecated docs also advertised Power Pages documentation of sitemap, web roles, and entity permissions hierarchy.
- Current repo contains empty `CloudAwesome.Dataverse.PowerPages` and `CloudAwesome.Dataverse.ProjectOps` projects.
- Legacy issues reference Project Operations base generation and custom pricing dimension configuration.

## Current State

- Current projects exist but have no source implementation.
- CLI contains placeholder `project-ops` and `document` branches.
- No current issues in the new repo explicitly cover Power Pages or ProjectOps, so these should be re-triaged before implementation.

## Intended Behaviour

Power Pages:

- generate or update Power Pages configuration records from a structured manifest;
- avoid taking a hard dependency on Visio as the only input path unless that remains a core requirement;
- document web pages, web roles, table/entity permissions, basic forms, and site navigation;
- include integration tests against a controlled portal-enabled environment only when such an environment is available.

ProjectOps:

- generate base Project Operations configuration where it is repeatable and safe;
- support custom pricing dimension configuration if still needed;
- clearly separate project-specific implementation from generic Dataverse customisation features.

## Bite-Sized Tasks

1. Re-read old Power Pages generation and documentation code.
2. Capture current Power Pages requirements and decide whether Visio remains a v1 input.
3. Define a JSON manifest for Power Pages generation.
4. Add pure tests for manifest parsing and planned record graph generation.
5. Implement read-only Power Pages documentation before mutating generation.
6. Add guarded integration tests only when a portal-enabled environment exists.
7. Re-read old ProjectOps issues and define a v1 requirements doc.
8. Decide whether ProjectOps belongs in this repo or a separate package/plugin.
9. Implement ProjectOps base generation only after requirements and tests exist.

## Acceptance Criteria

- Power Pages and ProjectOps have explicit requirements before code is written.
- Empty projects are either populated with tested implementation or removed/deferred deliberately.
- Integration requirements are documented and do not block foundation work.

