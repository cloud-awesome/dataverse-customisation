# Work Package: Publishing And Documentation

## Goal

Create reliable package publishing and replace deprecated public documentation after the foundation and core behaviours are stable.

## Source Signals

- Current issue: #7 Initial pipelines for releasing CLI and APIs.
- Deprecated docs: Docusaurus site under <https://docs.cloudawesome.uk/cds-customisation/>.
- Current repo has no releases published on GitHub at the time of review.
- CLI project is already configured as a .NET tool with package metadata and version `0.0.0.8`.

## Current State

- No GitHub Actions workflows were found in the local solution directory.
- CLI package metadata exists.
- API package boundaries are not yet settled.
- Public docs are still the deprecated solution docs.
- Schemas are not part of the current repo in the same way as the old project.

## Intended Behaviour

- CI runs restore, build, unit tests, package verification, and optional integration tests.
- Release workflow publishes:
  - CLI .NET tool package;
  - API package or packages.
- Release notes are generated from tags/issues/PRs.
- Docs site explains current JSON manifests and modern CLI/API usage.
- Schema files are versioned, published, and referenced by generated manifests.

## Bite-Sized Tasks

1. Decide package split: single API package or package per project/feature.
2. Normalize package metadata across projects.
3. Add CI build/test workflow.
4. Add package validation workflow using local `dotnet pack`.
5. Add release workflow gated by tags and NuGet secret.
6. Add generated release notes.
7. Add repo docs for each stable feature.
8. Design public docs site replacement.
9. Publish JSON schemas and link them from generated manifests.
10. Add docs deployment workflow.

## Acceptance Criteria

- CI blocks release if tests fail.
- `dotnet pack` works for intended packages.
- A dry-run or prerelease package can be created without manual local steps.
- Public docs are current for every released feature.

