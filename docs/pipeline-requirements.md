# CI/CD Pipeline High-Level Requirements

## All public facing projects must be built, tested and published on pushing to `origin/main`
- Public projects are currently:
  - .Cli
  - .Customisation
  - .PowerPages (Currently disabled)
  - .Processes
  - .ProjectOps (Currently disabled)
  - .Security
  - i.e. not .Core
- Any changes to any other public class libraries should trigger .Cli (as it consumes all the other libraries)
- Unit testing has not yet been implemented, so only a deactivated placeholder stage (to execute NUnit test suites) should be included for now

## Pipelines should be built as GitHub actions
- Pipelines should follow best practices, minimising code duplication, e.g. making heavy use of /actions or child templates
- Adding a new project in the future should be quick and easy to include in a new pipeline (or extending the current pipeline with minimal risk to existing projects)

## If a project is not included in the commit, it should not be included in the pipeline. Only changed projects should trigger the pipeline.

## If build and test stages succeed, a nuget package for the affected project(s) should be created
- The version numbering for the nuget package should follow the format "yyyy.MM.dd.HHmm" as of execution time.
- The successfully generated nuget package(s) should then be published to nuget.org
- No secrets or authentication parameters can be included in source, they should all be github secrets or similar.


