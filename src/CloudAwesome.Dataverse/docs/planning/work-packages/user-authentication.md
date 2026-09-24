# Work Package: User Authentication

## Goal

Enable manual CLI usage against Dataverse without requiring users to provide connection strings, bearer tokens, client secrets, or tenant-specific app registration details.

## Source Signals

- Roadmap priority: "0. User authentication functionality" in `docs/planning/backlog-roadmap.md`.
- Current issue: #28 Support user credentials login.
- Current state notes identify `UserNameAndPassword` as unimplemented and connection overrides as inconsistent.
- Testing strategy asks which authentication profile the vanilla integration environment should support first.

## Current State

- Existing connectivity routes must remain supported:
  - connection string;
  - app registration with client secret;
  - bearer token.
- `DataverseConnectionExtensions.GetServiceClient` already creates `Microsoft.PowerPlatform.Dataverse.Client.ServiceClient` instances.
- `ServiceClient` supports caller-managed authentication by accepting an access-token provider function.
- The CLI has a `test get-access-token` command that proves a basic MSAL interactive flow, but it is not integrated into normal command execution.
- `DataverseConnectionType.UserNameAndPassword` currently throws `NotImplementedException`.

## Recommended Direction

Use MSAL directly for interactive user authentication and pass tokens into `ServiceClient` through its token-provider constructor.

This should become a new first-class connection type, tentatively named `InteractiveUser`.

The default manual CLI path should be:

```text
dvcli test who-am-i --url https://example.crm11.dynamics.com
```

When no explicit connection type, connection string, app registration details, or bearer token are supplied, but a Dataverse URL is available, the CLI should assume interactive user authentication.

## Authentication Model

- Use a Cloud Awesome owned Microsoft Entra public-client app registration for the default experience.
- Use browser-based MSAL interactive authentication as the primary flow.
- Configure the public client with a `http://localhost` redirect URI for system-browser authentication.
- Request the Dataverse public-client delegated scope:

```text
<environment-url>/user_impersonation
```

- Attempt silent token acquisition first.
- Fall back to interactive browser login when no cached token is available, the cache is stale, consent is required, or Microsoft Entra requires user interaction.
- Store tokens using a persistent MSAL cache appropriate for a CLI tool.
- Allow an explicit `--client-id` override for organizations that require their own app registration.
- Allow an explicit `--tenant-id` or profile-selected tenant for single-tenant targeting, while keeping the model compatible with multi-tenant use.

## Options Considered

### MSAL Interactive Browser Plus ServiceClient Token Provider

Preferred implementation path.

Benefits:

- Best fit for a third-party CLI.
- Keeps authentication UX and profile handling inside the CLI.
- Avoids storing user passwords.
- Supports MFA and Conditional Access.
- Gives clear control over tenant, account, environment, cache, and future profile behavior.
- Keeps feature code on `IOrganizationService`.

Tradeoffs:

- Requires a Cloud Awesome public-client app registration before broad distribution.
- Requires token cache implementation and profile decisions.

### ServiceClient OAuth Connection String

Useful as a spike or fallback, but not the preferred long-term design.

Benefits:

- Quickest implementation.
- Uses existing `ServiceClient(string connectionString)` path.
- Microsoft documents OAuth connection strings with `LoginPrompt`, `RedirectUri`, and `TokenCacheStorePath`.

Tradeoffs:

- Pushes important auth behavior into connection-string configuration.
- Makes profile handling and diagnostics less explicit.
- Still requires a client ID and redirect URI.

### Device Code Flow

Useful as a later fallback for browser-restricted environments.

Benefits:

- Works over SSH or locked-down desktop sessions.
- Uses the same public-client registration and token cache model.

Tradeoffs:

- Slower and less polished for local developer workflows.
- Should not be the default when a browser is available.

### Windows Broker / WAM

Potential later enhancement for Windows-focused users.

Benefits:

- Better Windows SSO experience.
- Often aligns well with enterprise sign-in policies.

Tradeoffs:

- Adds platform-specific behavior.
- Browser flow is simpler and more portable for the first implementation.

### Username And Password

Do not implement as the manual-auth feature.

Reason:

- Poor fit for MFA and Conditional Access.
- Requires handling user credentials directly.
- Microsoft guidance treats password-based flows as a high-trust fallback, not the modern default.

The existing `UserNameAndPassword` enum value should either be deprecated or fail with a clear unsupported-auth message rather than throwing `NotImplementedException`.

## Intended Behaviour

- Existing pipeline-friendly authentication routes continue to work unchanged.
- A user can run a CLI command locally by providing only the target Dataverse URL when no saved profile exists.
- First run opens the system browser for Microsoft Entra authentication.
- Later runs use cached tokens silently where possible.
- Dataverse operations execute as the signed-in user and respect that user's Dataverse security roles.
- Authentication failures produce actionable messages:
  - invalid or missing environment URL;
  - tenant mismatch;
  - consent required;
  - user cancelled sign-in;
  - authenticated user lacks Dataverse access;
  - token acquisition failed.
- `test who-am-i` should be the initial smoke-test command for the feature.

## CLI And Profile Direction

First pass:

- Add `InteractiveUser` connection support.
- Require or infer only a Dataverse environment URL.
- Support optional `--tenant-id`.
- Support optional `--client-id` for private app registrations.
- Add integration smoke testing around `who-am-i`.

Follow-up:

- Add named profiles, similar in spirit to PAC CLI.
- Store profile records separately from the token cache.
- Profile data should include:
  - profile name;
  - environment URL;
  - tenant ID when known;
  - account/home account identifier when known;
  - client ID only when using a non-default app registration.
- Add commands such as:

```text
dvcli auth login --url <environment-url>
dvcli auth list
dvcli auth use <profile>
dvcli auth logout <profile>
```

Exact command names should be confirmed before implementation.

## Proposed Implementation Tasks

1. Add `DataverseConnectionType.InteractiveUser`.
2. Add connection-model properties for tenant ID, interactive client ID override, and profile name if needed.
3. Introduce a small authentication service in Core or CLI that:
   - builds an MSAL public client;
   - enables persistent token caching;
   - requests `<environment-url>/user_impersonation`;
   - tries silent acquisition before interactive acquisition.
4. Update `DataverseConnectionExtensions` or introduce a connection factory so `InteractiveUser` creates a `ServiceClient` with a token-provider function.
5. Update CLI connection resolution so missing explicit credentials imply interactive auth when a URL is available.
6. Update `test get-access-token` to use `/user_impersonation`, or replace it with the shared auth service.
7. Add a `test who-am-i` smoke path for manual verification.
8. Replace `UserNameAndPassword` `NotImplementedException` with a clear unsupported-auth exception or validation error.
9. Add unit tests for connection resolution, enum serialization, validation, and unsupported username/password behavior.
10. Add opt-in integration-test documentation for browser auth and `who-am-i`.

## Acceptance Criteria

- Existing connection string, app registration, and bearer-token paths still work.
- Running a command with only `--url` can authenticate through the browser and call Dataverse as the signed-in user.
- Token acquisition tries cached credentials before opening the browser.
- No user password is collected, stored, logged, or accepted for the new manual-auth path.
- Auth failures return clear messages and non-zero CLI exit codes.
- Unit tests cover connection selection and error paths without requiring a live Dataverse environment.
- Manual integration instructions document the app registration, environment URL, tenant, and consent assumptions.

## Open Questions

- Should the first Cloud Awesome public-client app registration be single-tenant for verification, then converted to multi-tenant, or should the implementation start multi-tenant?
- Where should local profiles live on Windows, macOS, and Linux?
- Should profile storage use plain JSON for non-secret metadata, with MSAL handling token secrecy separately?
- Should browser auth be the only first-pass interactive mode, or should device code be included at the same time?
- What exact CLI verbs should be used for login, profile selection, and logout?
- Should `UserNameAndPassword` remain in the public model as deprecated compatibility, or be replaced by `InteractiveUser` plus a migration note?

## Dependencies

- Cloud Awesome owned Microsoft Entra public-client app registration.
- Dataverse delegated permission for `user_impersonation`.
- Agreement on single-tenant versus multi-tenant registration for the first implementation.
- Persistent MSAL token cache package or implementation decision.
