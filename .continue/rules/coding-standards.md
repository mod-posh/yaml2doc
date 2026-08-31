---
name: Yaml2Doc C# standards
globs: ["**/*.cs", "**/*.csproj", "Directory.Build.props"]
alwaysApply: false
description: C# and MSBuild conventions for the .NET 9 solution
---

# C# Standards

- Follow existing .NET naming: PascalCase for types and public members, camelCase for parameters and locals, and `_camelCase` for private fields.
- Preserve nullable-reference-type correctness; `Directory.Build.props` enables `Nullable` and `ImplicitUsings` for the solution.
- Keep public API XML documentation accurate because packable projects generate documentation files.
- Prefer immutable/read-only contracts where the surrounding model permits them, and validate public arguments consistently with nearby code.
- For new asynchronous APIs, return `Task` or `Task<T>`, use the `Async` suffix, accept and propagate `CancellationToken` when cancellation is meaningful, and avoid blocking on tasks.
- Use constructor injection for required collaborators, following `Yaml2DocEngine`; keep composition in the CLI rather than adding a DI framework without a demonstrated need.
- Preserve project dependency direction described in [architecture.md](architecture.md) and all guardrails in [agent-workflow.md](agent-workflow.md).
