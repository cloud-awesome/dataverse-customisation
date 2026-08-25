# Testing Strategy

## Intent

Testing should describe intended behaviour, not preserve current implementation accidents. The test suite should make it safe to refactor toward a consistent public API, robust CLI behaviour, and predictable Dataverse mutations.

## Test Layers

### Unit Tests

Purpose: prove pure logic, manifest parsing, validation, model defaults, query construction, and command composition without a live Dataverse environment.

Initial targets:

- `CloudAwesome.Dataverse.Core.Tests`
- `CloudAwesome.Dataverse.Customisation.Tests`
- `CloudAwesome.Dataverse.Processes.Tests`
- `CloudAwesome.Dataverse.Security.Tests`
- `CloudAwesome.Dataverse.Cli.Tests`

Recommended scope:

- model defaults and serialization;
- JSON schema generation/validation once schema approach is selected;
- manifest validation;
- command setting binding and override precedence;
- plugin registration orchestration;
- process activation target state/status selection;
- security role diff logic;
- environment variable update/create behaviour.

### SDK Simulation Tests

Purpose: exercise Dataverse SDK interactions against an in-memory or fake `IOrganizationService`.

Candidates to evaluate:

- Modern FakeXrmEasy packages, because the deprecated tests used FakeXrmEasy successfully.
- CloudAwesome.Dataverse.Simulate, because it is aligned with the Cloud Awesome ecosystem.
- A minimal in-repo test double for request/response paths that are not supported by either library.

Decision criteria:

- .NET 8 compatibility;
- support for `Create`, `Retrieve`, `Update`, `Delete`, `Associate`, `Disassociate`, `RetrieveMultiple`, and `Execute`;
- support or extensibility for SDK messages such as `SetStateRequest`;
- clear setup of early-bound entities;
- easy assertion of created records and executed requests.

Decision:

Prefer CloudAwesome.Dataverse.Simulate. It is within our control to add any missing functionality and will be mutually beneficial to both projects.

### Integration Tests

Purpose: prove the CLI/API works against a real vanilla Dataverse environment.

Integration tests should be opt-in and controlled by environment variables. They must never run by default on a developer machine or public CI without explicit configuration.

Proposed required variables:

- `CA_DATAVERSE_URL`
- `CA_DATAVERSE_CLIENT_ID`
- `CA_DATAVERSE_CLIENT_SECRET`
- `CA_DATAVERSE_SOLUTION`
- `CA_DATAVERSE_PUBLISHER_PREFIX`

Optional variables:

- `CA_DATAVERSE_USERNAME`
- `CA_DATAVERSE_PASSWORD`
- `CA_DATAVERSE_TENANT_ID`

Initial integration scenarios:

- `who-am-i` connection smoke test;
- create/update one environment variable value;
- process activation against a disposable solution containing test artifacts;
- plugin registration using a sample signed plugin assembly;
- security role export/import against controlled test teams.

Integration tests should clean up after themselves, leaving the environment as it was found, enabling subsequent re-runs without becoming flaky.

### Regression Tests

Purpose: lock down fixes for known bugs and old behaviours that are easy to break.

Initial regression list:

- plugin entity images are created when present in the manifest;
- plugin-step process activation updates `sdkmessageprocessingstep`, not `workflow`;
- environment variable manifest with multiple new values processes every item;
- `--password` populates password, not username;
- plugin CLI command loads the manifest and invokes registration;
- missing manifest fails cleanly with a non-zero exit code;
- `updateAssemblyOnly` updates assembly and skips plugin type/step registration;
- `clobber` removes deleted child steps/images before re-registering;
- JSON deserialization failure returns a clear error rather than a null dereference.

### End-To-End Tests

Purpose: prove full CLI flows from command invocation to Dataverse result.

These should wait until Stage 2 fixes stabilize the command API. They can run against the vanilla Dataverse environment the maintainer can provide.

Candidate E2E flows:

- package CLI tool, install locally, run `dvcli test who-am-i`;
- generate a sample manifest, edit only connection and solution details, execute feature;
- register sample plugin assembly, verify plugin assembly/type/step/image records, unregister/clobber cleanup;
- generate a simple table with one text column, add to solution, then teardown if teardown is implemented;
- export team role mapping, import it into another team, audit console output.

## CI Quality Gate

Stage 1 CI should run:

```powershell
dotnet restore
dotnet build CloudAwesome.Dataverse.sln --configuration Release --no-restore
dotnet test CloudAwesome.Dataverse.sln --configuration Release --no-build
```

Stage 2 should add:

- warnings-as-errors after nullable cleanup is complete or after a scoped warning baseline is agreed;
- test result publishing;
- code coverage collection;
- package build verification;
- optional integration job gated by environment approval/secrets.

## Test Data Rules

- Keep sample manifests in test projects, not under production projects unless they are shipped samples.
- Use stable GUIDs only where deterministic assertions require them.
- Prefix all live Dataverse integration artifacts with a dedicated test prefix and solution name.
- Every integration test that creates data must document cleanup and preferably clean up automatically.

