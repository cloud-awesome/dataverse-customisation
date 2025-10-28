# CI/CD Workflows for CloudAwesome.Dataverse

This repository uses GitHub Actions to detect changes, build, (placeholder) test, pack, and publish NuGet packages for affected projects only.

Overview
- Orchestration workflow: .github/workflows/cicd.yml (triggered on push to main)
- Reusable workflow: .github/workflows/template-dotnet.yml (build/pack/publish)
- Central config: .github/project-map.json (maps project identifiers to .csproj paths)

How change detection works
1. On push to main, the detector job computes the changed files between the previous commit and the current commit. If the previous SHA is not available, it falls back to the merge-base with origin/main.
2. Changed file paths are mapped to projects using .github/project-map.json.
3. Feature flags exclude disabled projects (PowerPages and ProjectOps by default).
4. Dependency rule: if any public project (excluding disabled) changes, the CLI project (cli) is also included.
5. Global/shared file changes (e.g., Directory.Build.props, solution files) trigger all public projects and cli.
6. The detector emits a de-duplicated matrix like [{ name, path, should_publish: true }] and a single, run-wide VERSION in format yyyy.MM.dd.HHmm (UTC).

How to enable currently disabled projects
- Flags are set at the top-level env section of cicd.yml:
  - ENABLE_POWERPAGES: "false"
  - ENABLE_PROJECTOPS: "false"
- To enable, set a flag to "true" and ensure the project is not marked disabled in project-map.json.

How to add a new project
1. Add a new entry to .github/project-map.json:
   {
     "myproject": { "path": "src/CloudAwesome.Dataverse/My.Project/My.Project.csproj", "public": true }
   }
2. If it is a public library, it will automatically trigger the CLI build when it changes.
3. No changes to the workflows are required; the detector will include it based on path matching.

Reusable workflow behavior
- Uses .NET SDK 8.0 with NuGet caching.
- Builds Release configuration only.
- Placeholder Test stage: a deliberate no-op (no tests executed yet).
- Packs with a single VERSION value across all affected projects in the run.
- Uploads .nupkg artifacts for traceability.
- Publishes to nuget.org on pushes to main using dotnet nuget push with --skip-duplicate.

Concurrency and safety
- The CI/CD workflow uses concurrency to prevent overlapping publishes for the same ref and cancels in-progress runs.
- Minimal permissions are granted: contents: read; packages: write.

Failure notifications
- The workflow emits GitHub Actions annotations (e.g., ::error::) when configuration is missing or invalid (like the project map). Steps will fail fast to surface issues.

Secrets and security
- No secrets are stored in source control. Configure the following secret in your repository or organization settings:
  - NUGET_API_KEY: API key for nuget.org publishing.
- Optionally, configure a protected environment for publish with required reviewers.
