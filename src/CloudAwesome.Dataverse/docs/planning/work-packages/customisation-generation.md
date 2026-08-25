# Work Package: Customisation Generation

## Goal

Rebuild customisation generation around tested intended behaviour, then port legacy parity incrementally.

## Source Signals

- Current issue: #4 Port over existing Configuration Generation functionality.
- Current issue: #10 List missing dependencies from an exported solution.
- Current issue: #29 Solution layer report research.
- Deprecated docs: entities, attributes, forms, views, global option sets, security roles, field-level security profiles, model-driven apps, sitemaps, and clobber.
- Deprecated tests: customisation extensions, configuration manifest validation, entity/attribute model validation.

## Current State

- `GenerateConfigurations.Run` retrieves the publisher prefix and calls `EntityModel.Generate`.
- TODOs remain for manifest validation, global option sets, security roles, and model-driven apps.
- Current manifest only exposes `solutionName`, `clobber`, logging, and entities. Option sets, security roles, and model-driven apps are commented out.
- `EntityModel` contains TODOs for adding attributes to views and refactoring role creation.

## Intended Behaviour

- Process manifest nodes in dependency order.
- Create/update global option sets before attributes that use them.
- Create/update entities before dependent attributes, views, forms, and security permissions.
- Generate or update security roles and field-level security profiles predictably.
- Create model-driven apps and sitemaps after components exist.
- Add created/updated components to the intended solution without over-adding existing entities where avoidable.
- Support clobber only with explicit safeguards and clear sandbox warnings.

## Bite-Sized Tasks

1. Port customisation extension tests: labels, logical names, pluralisation.
2. Add manifest validation tests for required solution/publisher and valid child nodes.
3. Add entity/attribute model default and validation tests.
4. Stabilize entity and attribute generation first.
5. Add option set generation.
6. Add security role and field-level profile generation.
7. Add form and view generation.
8. Add model-driven app and sitemap generation.
9. Implement missing dependency report from exported solution zip.
10. Research solution layer report after dependency report is stable.

## Acceptance Criteria

- A minimal JSON manifest can create a table with one text column in a test Dataverse environment.
- Unit tests cover schema name derivation and publisher prefix behaviour.
- Integration tests confirm components are added to the target solution.
- Unsupported legacy nodes are either implemented or rejected with clear validation messages.

