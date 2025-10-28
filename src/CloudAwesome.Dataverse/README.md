# CloudAwesome.Dataverse

[![CI/CD](https://github.com/<owner>/<repo>/actions/workflows/cicd.yml/badge.svg?branch=main)](https://github.com/<owner>/<repo>/actions/workflows/cicd.yml)

This repository contains CloudAwesome Dataverse projects and a GitHub Actions pipeline that:
- Detects changes on push to main and builds only affected public projects.
- Runs a placeholder Test stage (no-op for now).
- Packs and publishes NuGet packages (on main) with a unified, time-based version.

See also:
- CI/CD Orchestration: .github/workflows/cicd.yml
- Reusable .NET workflow: .github/workflows/template-dotnet.yml
- Pipeline documentation: .github/README.md
