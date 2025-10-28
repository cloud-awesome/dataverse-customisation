Pipeline Build Plan for CloudAwesome Dataverse Projects

1. Goal Overview
- Implement CI/CD pipelines in GitHub Actions for all public-facing projects in this repository, ensuring that on push to origin/main only the changed projects are built, (placeholder) tested, and, upon success, packaged and published to NuGet with a time-based version. Pipelines must minimize duplication, be easy to extend for new projects, and keep all secrets outside source control.

2. Scope and Targets
- Public-facing projects to include:
  - CloudAwesome.Dataverse.Cli (.Cli)
  - CloudAwesome.Dataverse.Customisation (.Customisation)
  - CloudAwesome.Dataverse.Processes (.Processes)
  - CloudAwesome.Dataverse.Security (.Security)
  - CloudAwesome.Dataverse.PowerPages (.PowerPages) — currently disabled
  - CloudAwesome.Dataverse.ProjectOps (.ProjectOps) — currently disabled
- Non-public library not directly published: CloudAwesome.Dataverse.Core (.Core)
- Special dependency rule: Any changes to any other public class libraries must also trigger .Cli (because .Cli consumes all other libraries).

3. High-Level Phases
- Phase A — Pipeline Design and Reuse Strategy
  - Create a reusable workflow (e.g., .github/workflows/reusable-dotnet.yml) that encapsulates restore, build, test (placeholder), pack, and optional publish steps. Input parameters: project_name, project_path, should_publish, dotnet_version, include_cli_trigger.
  - Create a lightweight composite action (optional) for .NET setup and caching to further reduce duplication across jobs.

- Phase B — Change Detection and Matrix Orchestration
  - Add a primary workflow (e.g., .github/workflows/cicd.yml) triggered on push to main. A first job (“detector”) computes which projects changed in the commit and outputs a matrix for downstream jobs.
  - Detection rules:
    - If only .Core changed: no direct publication (not public) but if .Core change impacts public projects consumed by .Cli, choose to trigger .Cli only if .Core is an explicit dependency. Since requirement does not mandate .Cli to trigger for .Core changes and .Core is not a public library, default to not triggering .Cli solely for .Core changes.
    - If a public library changed (Customisation, Processes, Security, PowerPages, ProjectOps): include that project in matrix. If any public library (excluding PowerPages/ProjectOps if disabled) changed, also include .Cli.
    - If only .Cli changed: include .Cli in matrix.
    - Exclude disabled projects (PowerPages, ProjectOps) from matrix even if changed, until flags are enabled.
  - Output a JSON array of project descriptors for a strategy matrix used to call the reusable workflow once per affected project.

- Phase C — Build and Placeholder Test
  - For each affected project:
    - Setup .NET (net8.0), restore with caching of NuGet packages.
    - Build Release configuration.
    - Testing placeholder: create a dedicated step/stage intended to run NUnit tests in the future, currently deactivated. Implementation options:
      - Option 1: A no-op “Test” step that echoes a message and succeeds.
      - Option 2: Run dotnet test with continue-on-error: true or condition: false to act as a placeholder. Prefer clear no-op to avoid false signals.

- Phase D — Pack and Publish
  - If build and placeholder test succeed, run dotnet pack to produce a NuGet package (.nupkg) per project.
  - Versioning: compute a version string at runtime in format yyyy.MM.dd.HHmm based on execution time (UTC recommended) and pass it to pack (/p:Version=…). Apply a single VERSION environment variable for the entire workflow run to keep all packages aligned.
  - Publish: Use dotnet nuget push to nuget.org when pushing to main. Authenticate using a GitHub secret (e.g., NUGET_API_KEY). Upload .nupkg artifacts regardless of publish outcome for traceability.

- Phase E — Quality and Operations
  - Add status badges and basic documentation in README referencing the pipeline and how to add new projects.
  - Implement concurrency and protections to avoid overlapping publishes for the same ref.
  - Add simple notifications (e.g., Actions annotations) for failures.

4. Detailed Implementation Steps
- Step 1: Define project map and flags
  - Create a small YAML/JSON map (inline in workflow env or as a repo file) mapping project identifiers to paths:
    - cli: src/CloudAwesome.Dataverse/CloudAwesome.Dataverse.Cli/CloudAwesome.Dataverse.Cli.csproj
    - customisation: src/CloudAwesome.Dataverse/CloudAwesome.Dataverse.Customisation/CloudAwesome.Dataverse.Customisation.csproj
    - processes: src/CloudAwesome.Dataverse/CloudAwesome.Dataverse.Processes/CloudAwesome.Dataverse.Processes.csproj
    - security: src/CloudAwesome.Dataverse/CloudAwesome.Dataverse.Security/CloudAwesome.Dataverse.Security.csproj
    - powerpages: src/CloudAwesome.Dataverse/CloudAwesome.Dataverse.PowerPages/CloudAwesome.Dataverse.PowerPages.csproj (disabled)
    - projectops: src/CloudAwesome.Dataverse/CloudAwesome.Dataverse.ProjectOps/CloudAwesome.Dataverse.ProjectOps.csproj (disabled)
    - core: src/CloudAwesome.Dataverse/CloudAwesome.Dataverse.Core/CloudAwesome.Dataverse.Core.csproj (non-public)
  - Define feature flags in workflow env:
    - ENABLE_POWERPAGES=false
    - ENABLE_PROJECTOPS=false

- Step 2: Reusable workflow (.github/workflows/reusable-dotnet.yml)
  - Inputs: project_name, project_path, should_publish (bool), dotnet_version (default 8.0), version (string), is_cli (bool).
  - Steps:
    1) actions/checkout@v4
    2) actions/setup-dotnet@v4 with caching for NuGet, dotnet-version: input
    3) dotnet restore {project_path}
    4) dotnet build {project_path} -c Release --no-restore
    5) Placeholder tests stage (no-op for now)
    6) dotnet pack {project_path} -c Release -o ./artifacts /p:Version=${{ inputs.version }} --no-build
    7) Upload artifact: "nupkgs-${{ inputs.project_name }}"
    8) Conditional publish: if should_publish == true and on push to main, run dotnet nuget push ./artifacts/*.nupkg --source https://api.nuget.org/v3/index.json --api-key ${{ secrets.NUGET_API_KEY }} --skip-duplicate

- Step 3: Orchestration workflow (.github/workflows/cicd.yml)
  - on: push: branches: [main]
  - Jobs:
    - detector:
      - Checkout full history (fetch-depth: 0) to compute diff from github.event.before to github.sha.
      - Use a script (bash, PowerShell, or actions/github-script) to:
        - List changed files.
        - Map changes to projects using the project map.
        - Remove duplicates.
        - Apply flags (exclude disabled projects, include .Cli when any other public project is selected).
        - Output a matrix as JSON via set-output (or GHA job outputs) with an array of objects: [{ name, path, should_publish: true }].
      - Also compute a single VERSION string (UTC now): e.g., date -u +"%Y.%m.%d.%H%M" and set as job output.
    - build_publish:
      - strategy: matrix: from detector output.
      - uses: ./.github/workflows/reusable-dotnet.yml
      - with: project_name, project_path, should_publish: true, version: from detector output.
      - secrets: inherit (or pass NUGET_API_KEY explicitly).

- Step 4: Documentation
  - Add a README within the .github folder explaining:
    - How change detection works.
    - How to enable currently disabled projects (toggle flags and remove excludes).
    - How to add a new project: extend the map, confirm dependency rules (.Cli inclusion), commit.

5. Change Detection Strategy Details
- Trigger: on push to main only (as required). Add pull_request for validation (build and test only) without publishing to nuget.
- Diff base: github.event.before..github.sha for single push; if unavailable, fallback to origin/main merge-base.
- Mapping logic examples:
  - If a file under CloudAwesome.Dataverse.Customisation/** changed → include customisation; also include cli.
  - If a file under CloudAwesome.Dataverse.Security/** changed → include security; also include cli.
  - If only .Cli dir changed → include cli only.
  - If PowerPages changed but ENABLE_POWERPAGES=false → ignore.
  - If only .Core changed → no public builds; do nothing (per requirement).
- Edge cases: Solution or shared files updated (e.g., Directory.Build.props) — treat as affecting all public projects (and .Cli). Implement via path patterns.

6. Versioning and Packaging Details
- Version format: yyyy.MM.dd.HHmm (24-hour, UTC).
- Use one version per workflow execution to keep all produced packages consistent.
- Command example: dotnet pack <csproj> /p:Version=${VERSION}
- NuGet publish: dotnet nuget push artifacts/*.nupkg --skip-duplicate to avoid failure on re-runs within same minute.

7. Secrets and Security
- No secrets in source. Configure in GitHub repo or org secrets:
  - NUGET_API_KEY: API key for nuget.org publishing.
- Optionally protect a production environment for publish with required reviewers.
- Use permissions: contents: read, packages: write minimal permissions in workflows.

8. Minimizing Duplication and Ease of Extension
- Prefer reusable workflow with clear inputs over copy-paste job definitions.
- Optionally create a composite action for common .NET steps.
- Centralize the project map in a single place consumed by the detector job.
- Adding a new project:
  - Add entry to project map.
  - If it is a public lib, ensure detector adds .Cli when this project changes.
  - That’s it — no new workflow file needed.

9. Dependencies, Risks, and Considerations
- Dependencies
  - GitHub Actions runners (ubuntu-latest assumed), actions/checkout, actions/setup-dotnet.
  - NuGet.org availability and credentials (NUGET_API_KEY stored as secret).
- Risks
  - Version collision if multiple runs occur within the same minute; mitigated by --skip-duplicate and by Actions concurrency to cancel in-progress runs for same ref.
  - False negatives in change detection (e.g., shared props/targets updates). Mitigate by defining a list of global paths that affect all projects.
  - Disabled project paths may still change; ensure they are filtered out to avoid accidental publish.
  - .Cli packaging requirements (as a DotNetTool) may require specific csproj metadata; since no code changes are allowed yet, confirm packaging settings before first publish.
- Considerations
  - Keep logs and artifacts for later inspection (upload nupkgs).
  - Future test enablement: replace placeholder with real dotnet test across test projects, collect TRX artifacts, fail on test failures.
  - Add PR validation workflow (no publish) to catch issues before merging to main.

10. Acceptance Criteria Traceability
- On push to origin/main:
  - Only changed public projects are built.
  - .Cli builds whenever any other public project changes (in addition to its own changes).
  - Test stage exists but is deactivated (no tests run yet).
  - If build and placeholder test succeed, affected project(s) are packed with version yyyy.MM.dd.HHmm and published to nuget.org using GitHub secrets.
  - Workflows are designed using a reusable workflow to minimize duplication and simplify future project additions.
