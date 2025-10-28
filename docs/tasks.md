1. [x] Phase A — Pipeline Design and Reuse Strategy: Establish reusable building blocks
   1. [x] Create a reusable workflow file at .github/workflows/template-dotnet.yml encapsulating: restore, build, placeholder test, pack, and conditional publish.
   2. [x] Define inputs for the reusable workflow: project_name (string), project_path (string), should_publish (bool), dotnet_version (string, default "8.0"), version (string), is_cli (bool).
   3. [x] Implement steps in the reusable workflow:
      1. [x] Checkout repository using actions/checkout@v4.
      2. [x] Setup .NET using actions/setup-dotnet@v4 with NuGet caching and input-configured dotnet-version.
      3. [x] Restore NuGet packages: dotnet restore {project_path}.
      4. [x] Build the project in Release: dotnet build {project_path} -c Release --no-restore.
      5. [x] Add a placeholder Test stage that intentionally does no work (clear no-op) and always succeeds.
      6. [x] Pack the project to ./artifacts with a provided version: dotnet pack {project_path} -c Release -o ./artifacts /p:Version=${{ inputs.version }} --no-build.
      7. [x] Upload the produced .nupkg(s) as an artifact named nupkgs-${{ inputs.project_name }}.
      8. [x] Conditionally publish to NuGet.org when should_publish is true and the run is on main: dotnet nuget push ./artifacts/*.nupkg --source https://api.nuget.org/v3/index.json --api-key ${{ secrets.NUGET_API_KEY }} --skip-duplicate.
   4. [ ] (Optional) Create a composite action for common .NET steps (setup and caching) to further reduce duplication.

2. [x] Phase B — Change Detection and Matrix Orchestration
   1. [x] Create a primary orchestration workflow at .github/workflows/cicd.yml triggered on push to main.
   2. [x] Implement a detector job that:
      1. [x] Checks out full git history (fetch-depth: 0).
      2. [x] Computes the diff between github.event.before and github.sha; if unavailable, fall back to the merge-base with origin/main.
      3. [x] Lists changed files and maps changes to projects using a centralized project map (see Phase C Step 1).
      4. [x] Applies feature flags to exclude disabled projects (ENABLE_POWERPAGES, ENABLE_PROJECTOPS).
      5. [x] Applies dependency rule: include .Cli when any other public project (excluding disabled) changes; include only .Cli when only .Cli changes; ignore .Core-only changes.
      6. [x] Handles edge cases: treat shared/global files (e.g., Directory.Build.props, solution or shared targets) as affecting all public projects and .Cli.
      7. [x] Produces a unique, de-duplicated matrix array of project descriptors in the form [{ name, path, should_publish: true }].
      8. [x] Computes a single VERSION string for the run using UTC in the format yyyy.MM.dd.HHmm and exposes it as a job output.
   3. [x] Implement a build_publish job that:
      1. [x] Consumes the detector matrix output via a strategy.matrix.
      2. [x] Invokes the reusable workflow for each matrix entry with inputs: project_name, project_path, should_publish: true, version: detector.VERSION.
      3. [x] Inherits or passes required secrets (NUGET_API_KEY) to the reusable workflow.
   4. [x] Configure workflow permissions minimally: contents: read; packages: write.
   5. [x] Add workflow concurrency to prevent overlapping publishes for the same ref.

3. [x] Phase C — Project Map, Flags, and Configuration Centralization
   1. [x] Define a single project map (JSON file in repo) with identifiers to project paths:
      1. [x] cli → CloudAwesome.Dataverse.Cli/CloudAwesome.Dataverse.Cli.csproj
      2. [x] customisation → CloudAwesome.Dataverse.Customisation/CloudAwesome.Dataverse.Customisation.csproj
      3. [x] processes → CloudAwesome.Dataverse.Processes/CloudAwesome.Dataverse.Processes.csproj
      4. [x] security → CloudAwesome.Dataverse.Security/CloudAwesome.Dataverse.Security.csproj
      5. [x] powerpages → CloudAwesome.Dataverse.PowerPages/CloudAwesome.Dataverse.PowerPages.csproj (disabled)
      6. [x] projectops → CloudAwesome.Dataverse.ProjectOps/CloudAwesome.Dataverse.ProjectOps.csproj (disabled)
      7. [x] core → CloudAwesome.Dataverse.Core/CloudAwesome.Dataverse.Core.csproj (non-public)
   2. [x] Add feature flags in workflow env to enable/disable projects:
      1. [x] ENABLE_POWERPAGES=false
      2. [x] ENABLE_PROJECTOPS=false
   3. [x] Define path patterns that indicate global changes affecting all public projects and .Cli (e.g., Directory.Build.props, solution files, common targets).

4. [x] Phase D — Build, Placeholder Test, Pack, and Publish Behavior
   1. [x] Ensure the reusable workflow sets up .NET 8.0 and caches NuGet packages.
   2. [x] Ensure builds are Release configuration only and fail on errors.
   3. [x] Implement the placeholder Test stage as a clear no-op step (preferred over running tests with continue-on-error) to avoid false signals.
   4. [x] Implement packaging with dotnet pack using the run-wide VERSION environment variable for consistent package versions.
   5. [x] Upload .nupkg artifacts for each project regardless of publish outcome for traceability.
   6. [x] Publish to NuGet.org on push to main using dotnet nuget push with --skip-duplicate to mitigate rerun collisions.

5. [x] Phase E — Quality, Documentation, and Operations
   1. [x] Add README documentation in .github/ that explains:
      1. [x] How change detection works and how the matrix is constructed.
      2. [x] How to enable currently disabled projects (toggle flags, remove excludes).
      3. [x] How to add a new project: extend the project map and verify .Cli inclusion rules.
   2. [x] Add status badges to the main README referencing the CI/CD workflow(s).
   3. [x] Implement simple failure notifications (e.g., GitHub Actions annotations) for failed steps.
   4. [x] Configure concurrency to avoid overlapping publishes for the same ref (e.g., using concurrency.group and cancel-in-progress).

6. [x] Phase F — Secrets and Security
   1. [x] Ensure no secrets are committed to source control.
   2. [x] Configure the NUGET_API_KEY as a GitHub repository or organization secret.
   3. [x] Optionally protect a publish environment with required reviewers for production publishing.

7. [ ] Phase G — Pull Request Validation (No Publish)
   1. [ ] Add a separate PR validation workflow triggered on pull_request that:
      1. [ ] Restores, builds, and runs the placeholder test stage (no publish).
      2. [ ] Uses the same reusable workflow where possible (with should_publish=false) or an equivalent job definition.

8. [ ] Phase H — Acceptance Criteria Verification
   1. [ ] Verify that on push to origin/main only changed public projects are built.
   2. [ ] Verify that .Cli is included whenever any other public project changes (in addition to .Cli’s own changes).
   3. [ ] Verify the Test stage exists but is deactivated (no tests run yet).
   4. [ ] Verify that successful builds are packed with a unified yyyy.MM.dd.HHmm UTC version across all affected projects in a single run.
   5. [ ] Verify that packages are published to NuGet.org using the configured GitHub secret.
   6. [ ] Verify that workflows use a reusable workflow to minimize duplication and simplify future additions.

9. [ ] Phase I — Risks, Edge Cases, and Future Enhancements
   1. [ ] Implement handling for shared file updates (e.g., Directory.Build.props) to trigger all public builds and .Cli.
   2. [ ] Mitigate version collision risk by using --skip-duplicate and Actions concurrency settings.
   3. [ ] Exclude disabled projects from publication even if they change while flags are false.
   4. [ ] Confirm .Cli packaging metadata is appropriate for a DotNetTool (no code changes yet; just readiness check).
   5. [ ] Plan for future test enablement: replace the no-op with real dotnet test runs, collect TRX, and fail on test failures when ready.
