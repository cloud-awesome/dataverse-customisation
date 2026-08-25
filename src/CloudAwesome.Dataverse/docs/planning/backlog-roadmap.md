# Backlog Roadmap

This roadmap merges three sources:

- the maintainer's requested sequence: test foundation, bug/refactor foundation, then features;
- current open issues in <https://github.com/cloud-awesome/dataverse-customisation/issues>;
- legacy feature and issue signals from <https://github.com/apng1982/cds-customisation>.

## Current Repo Issues By Theme

### Foundation And Architecture

- #33 Create high-level requirements for existing and planned features.
- #8 Document design decisions for the public interface.
- #18 Support reduced CLI options without a manifest.
- #28 Support user credentials login.
- #5 Generate manifest for features accepting a manifest.

### DevOps And Publishing

- #7 Initial pipelines for releasing CLI and APIs.

### Plugin Registration

- #6 Port over plugin registration functionality.

### Customisation Generation

- #4 Port existing configuration generation functionality.
- #10 List missing dependencies from an exported solution.
- #29 Research solution layer report for components with multiple managed/unmanaged layers.

### Process Activation

- #1 Initial process activation code.
- #14 Process activation for solution business rules.
- #15 Process activation for plugins by assembly.
- #16 Process activation for listed cloud flows.
- #17 JSON schema for process activation manifest.
- #27 Generate manifest for process activation.
- #31 Process activation for BPFs by solution.

### Security

- #32 Optional console output of teams and assigned roles.

### Legacy-Only Until Re-Triaged

- Bulk deletion job migration.
- Documentation generation.
- Power Pages generation.
- Project Operations generation.

## Legacy Feature Inventory

### Legacy Features Documented

- Plugin registration:
  - plugin assemblies;
  - plugin types;
  - steps;
  - entity images;
  - service endpoints;
  - clobber;
  - update assembly only;
  - solution override at root and assembly level.
- Generate customisations:
  - entities;
  - attributes;
  - forms;
  - views;
  - global option sets;
  - security roles;
  - field-level security profiles;
  - model-driven apps;
  - sitemaps;
  - clobber.
- Toggle process status:
  - plugin steps;
  - workflows;
  - modern flows;
  - record creation rules.
- Bulk deletion job migration.
- Documentation generation:
  - metadata/system configuration docs;
  - PDF/Markdown outputs;
  - entity relationship diagrams;
  - security roles;
  - workflows/cloud processes;
  - Power Pages site documentation.
- Power Pages generation:
  - web pages;
  - basic forms;
  - navigation from a Visio-originated design.

### Legacy Open Issues Worth Re-Triaging

These are not automatically in scope for the new repo, but should be reviewed when building the Stage 3 backlog:

- plugin registration safeguards: verify messages/entities/filters before registration, signed assembly check, solution validation, relative paths, no filtering attributes, no steps, service endpoints, custom APIs, plugin packages;
- customisation generation: entity creation order, granular solution assignment, model-driven app/sitemap, view attributes, autonumbering, clobber behaviour;
- process activation: workflows, record creation rules, SLAs;
- security: assign roles to teams;
- dependencies: missing dependency report;
- schemas and generated blank manifests;
- Project Operations generation and pricing dimension configuration;
- test/demo data generation and language translation support.

## Recommended Sequence

### 1. Foundation First

Complete:

- test projects and harness decision;
- command/manifest loading consistency;
- public API design notes;
- build warnings triage;
- first CI build/test pipeline.

Reason: this reduces the risk of turning the port into a larger untested rewrite.

### 2. Fix Existing Behaviour

Complete:

- plugin registration wiring and known bug fixes;
- process activation correctness for flows, plugin steps, and SLAs;
- security export/import correctness and audit output;
- environment variable update correctness;
- customisation generation entity/attribute baseline.

Reason: this creates a functional minimum product before adding new or legacy parity features.

### 3. Port Mature Legacy Features

Complete:

- full plugin registration parity;
- customisation generation parity;
- process activation parity;
- missing dependency report;
- manifest generation and schema publication.

Reason: these are already understood and have old code/tests/docs to mine.

### 4. Add New Functionality

Complete only after the relevant foundation exists:

- user/interactive authentication;
- solution layers report;
- bulk deletion job migration;
- documentation generation;
- ProjectOps;
- Power Pages generation/documentation;
- public documentation site replacement.
