# Work Package: Plugin Registration

## Goal

Restore and modernise plugin registration as one of the first mature legacy parity features.

## Source Signals

- Current issue: #6 Port over Plugin Registration functionality.
- Deprecated docs: plugin assemblies, plugin types, steps, entity images, service endpoints, clobber, update-assembly-only, and solution override.
- Deprecated tests: `RegisterPluginsTests`, `RegisterServiceEndpointTests`, `PluginManifestValidatorTests`, `ValidatePluginManifestTests`, model and validator tests.

## Current State

- Core classes exist in `CloudAwesome.Dataverse.Processes`.
- CLI branch is wired to `PlaceholderCommand`.
- `PluginRegistrationCommand` exists but only writes test text.
- Entity images are likely skipped due to an inverted `Any()` check.
- Custom API registration is commented out.
- Service endpoints/webhooks/workflow assemblies are not active.
- Manifest validation is absent.

## Intended Behaviour

- Register or update plugin assemblies from a JSON manifest.
- Register plugin types, steps, and entity images.
- Support `clobber` by deleting manifest-referenced child records safely before re-registering.
- Support `updateAssemblyOnly`.
- Resolve solution name from assembly-level override, falling back to root-level value.
- Validate messages, primary entities, filters, assembly path, signing, and solution existence before mutation.
- Support service endpoints after assembly/type/step baseline is stable.
- Emit clear logs and non-zero CLI exit codes on failure.

## Bite-Sized Tasks

1. Port old model default tests for plugin assemblies, plugins, steps, images, custom APIs, request parameters, and response properties.
2. Add manifest validation tests before implementing validators.
3. Wire CLI `plugins register/unregister` to real command classes.
4. Fix manifest loading and connection override behaviour.
5. Add regression test and fix for entity image creation.
6. Add tests for `clobber`, `updateAssemblyOnly`, missing assembly, and solution override.
7. Re-enable or re-port custom API registration with tests.
8. Re-port service endpoint model/registration with tests.
9. Add live integration test using a sample signed plugin assembly.
10. Document JSON manifest structure and migration notes from old XML manifests.

## Acceptance Criteria

- Unit tests cover the old mature plugin registration behaviours.
- CLI can register and unregister a sample plugin manifest.
- Integration test verifies Dataverse records for assembly, type, step, and image.
- Unsupported manifest nodes fail with clear messages, not silent omission.

