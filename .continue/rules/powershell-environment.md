---
name: Yaml2Doc PowerShell environment
globs: [".github/workflows/*.{yml,yaml}"]
alwaysApply: false
description: PowerShell 7 local shell and mixed-OS GitHub Actions constraints
---

# PowerShell Environment

- Local development uses PowerShell 7; PowerShell 7.6.5 was present when this configuration was created.
- Product code is C#/.NET. Do not introduce PowerShell product scripts or module conventions unless the repository intentionally adopts them.
- Keep routine `dotnet build` and `dotnet test` commands shell-neutral.
- Build and test jobs run on `windows-latest`; release and notification jobs run on `ubuntu-latest`.
- In workflow scripts, use `shell: pwsh` when PowerShell syntax is required. Do not assume Windows paths or Windows-only cmdlets in Ubuntu jobs.
- Follow the workflow and generated-output constraints in [agent-workflow.md](agent-workflow.md).
