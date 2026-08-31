---
name: Yaml2Doc testing
globs:
  ["tests/**/*.cs", "tests/**/*.csproj", "samples/pipelines/*.{yml,yaml,md}"]
alwaysApply: false
description: xUnit and golden-fixture conventions used by Yaml2Doc
---

# Testing

- Use xUnit 2 and the existing `[Fact]` style in `tests/Yaml2Doc.Core.Tests/`; no NUnit, MSTest, or Pester setup exists.
- Place tests beside the existing test class for the owning behavior and name them after the observable scenario.
- Assert public behavior, error messages or exit codes where they are contractual, and avoid coupling tests to incidental implementation details.
- Preserve standard-YAML behavior and add regression coverage when changing loading, dialect resolution, rendering, or CLI path safety.
- Keep checked-in golden inputs and expected Markdown paired under `samples/pipelines/`; update them only when an intentional output change is part of the request.
- Run `dotnet test` locally; use the explicit Release command in [build-and-ci.md](build-and-ci.md) when reproducing CI.
- Follow all generated-path and external-dependency guardrails in [agent-workflow.md](agent-workflow.md).
