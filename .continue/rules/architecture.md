---
name: Yaml2Doc architecture
globs: ["src/**", "tests/**", "samples/**", "*.sln", "*.props"]
alwaysApply: false
description: Project boundaries and dependency direction for Yaml2Doc
---

# Architecture

Yaml2Doc converts YAML documents into predictable, human-readable Markdown. Standard YAML is the default; GitHub Actions and Azure Pipelines are optional dialects.

- `src/Yaml2Doc.Core/` owns YAML loading, the neutral document model, dialects, registry behavior, and conversion orchestration.
- `src/Yaml2Doc.Markdown/` owns Markdown rendering and references `Yaml2Doc.Core`.
- `src/Yaml2Doc.Cli/` owns argument parsing, safe file handling, process exit behavior, and composition of Core and Markdown.
- `tests/Yaml2Doc.Core.Tests/` owns the xUnit unit, regression, integration, and CLI tests for all three product projects.
- `samples/pipelines/` contains checked-in YAML and Markdown golden fixtures used by tests.
- `Directory.Build.props` owns solution-wide .NET, package, nullable, documentation, and SourceLink defaults.

Follow the generated-path and external-dependency boundaries in [agent-workflow.md](agent-workflow.md).
