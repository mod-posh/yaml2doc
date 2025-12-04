# Changelog

All changes to this project should be reflected in this document.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/), and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

---

## [1.1.0](https://github.com/mod-posh/yaml2doc/releases/tag/v1.1.0) - 2025-12-03

### Added

- **Pluggable pipeline dialects**:
  - `GitHubActionsYamlDialect` (`gha`) for `.github/workflows/*.yml`.
  - `AzurePipelinesYamlDialect` (`ado`) for `azure-pipelines.yml` and multi-stage Azure DevOps YAML.
- **CLI dialect selection**:
  - `--dialect <id>` to explicitly choose `standard`, `gha`, or `ado`.
- **Dialect-aware Markdown**:
  - Extended `BasicMarkdownRenderer` to surface extra sections when dialect metadata is present (e.g., `## Trigger`, `## Jobs`, `## Steps`) while preserving the original `# <Name>` and `## Root Keys` sections.
- **Golden fixtures and tests for dialects**:
  - Added sample GitHub Actions and Azure Pipelines “golden” YAML/Markdown pairs.
  - Added CLI tests that run with `--dialect gha` / `--dialect ado` and assert the rendered Markdown matches the golden files.

### Changed

- **Dialect registry**:
  - Updated `Yaml2DocRegistry` to support resolving dialects by ID (for the CLI’s `--dialect` flag) in addition to detection.
- **Documentation**:
  - Updated the top-level `README.md` to introduce dialects, `--dialect` usage, and explain why this release is a non-breaking **v1.1.0**.
  - Updated project docs (`Yaml2Doc.Core`, `Yaml2Doc.Markdown`, `Yaml2Doc.Cli`) to describe dialect usage and the new behavior.
- **Standard behavior explicitly locked in**:
  - Added a regression test using `samples/pipelines/standard-golden.yml` → `standard-golden.md` to guarantee that running Yaml2Doc **without** a dialect flag still behaves exactly like v1.0.0.

---

## [1.0.0](https://github.com/mod-posh/yaml2doc/releases/tag/v1.0.0) - 2025-12-02

### Added

- **Initial solution structure**:
  - `Yaml2Doc.Core` – core model, loader, and dialect abstraction.
  - `Yaml2Doc.Markdown` – baseline Markdown renderer.
  - `Yaml2Doc.Cli` – console application / CLI host.
  - `Yaml2Doc.Core.Tests` – xUnit test project.

- **Core YAML model and loader**:
  - `PipelineDocument` neutral in-memory model for generic YAML.
  - `YamlDocumentContext` wrapper around `YamlStream` with root-node validation.
  - `YamlLoader` to convert YAML mappings/sequences into `PipelineDocument` (root dictionary + `Name`).

- **Standard YAML dialect**:
  - `StandardYamlDialect` as the default catch-all dialect for generic YAML.
  - `Yaml2DocRegistry` to resolve dialects (standard only in v1.0.0).

- **Baseline Markdown renderer**:
  - `IMarkdownRenderer` interface and `BasicMarkdownRenderer` implementation.
  - Output shape:
    - `# <Name>` (or `# YAML Document` if no `name`).
    - `## Root Keys` section listing top-level keys.

- **CLI host**:
  - `Yaml2DocCli` entry point and `program.cs` wrapper.
  - Safe path handling (no escaping working directory, no UNC/device paths, no traversal via symlinks/junctions).
  - `--output` parameter to write directly to a file that must not already exist.
  - Exit codes and error reporting for missing input, invalid paths, and parse failures.

- **Golden sample and tests**:
  - `samples/pipelines/standard-golden.yml` and `samples/pipelines/standard-golden.md` as the baseline “golden” pair.
  - Unit tests covering:
    - YAML loading into `PipelineDocument`.
    - Standard dialect resolution.
    - BasicMarkdownRenderer behavior.
    - CLI success/error paths.

- **Documentation**:
  - Initial README describing Yaml2Doc v1 scope and limitations:
    - “Standard YAML in, straightforward Markdown out.”
    - Explicitly calls out that CI/CD-specific semantics (GitHub Actions, Azure DevOps, Jenkins, etc.) are out of scope for v1.0.0.

---
