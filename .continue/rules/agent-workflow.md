---
name: Yaml2Doc agent workflow
alwaysApply: true
description: Core workflow and repository guardrails for all Yaml2Doc work
---

# Agent Workflow

- Search for the owning implementation and nearby tests before editing.
- Prefer the smallest change that satisfies the request and preserves existing public behavior.
- Ask before a broad refactor, public API break, dependency replacement, or multi-project redesign.
- Run the narrowest relevant test first, then `dotnet build` and `dotnet test` when the change can affect the solution.
- Do not hand-edit generated output in `Docs/`, `src/*/docs/`, `**/bin/`, `**/obj/`, `publish/`, or `**/TestResults/`.
- Treat `.vscode/` as local editor configuration and out of scope unless the user explicitly requests it.
- This repository consumes the sibling `xml2doc` project through `Xml2Doc.MSBuild`. Do not copy or reimplement xml2doc behavior here; propose changes in that dependency separately.
