# v1.1.0 – Dialect-aware parsing & richer Markdown

**Goal:**
Extend Yaml2Doc with explicit dialect selection (standard / GitHub Actions / Azure DevOps) and richer, dialect-aware Markdown sections **without breaking** existing v1 behavior.

**Versioning note:**
This is a **minor** version bump (`1.1.0`), not `2.0.0`, because:

* Standard YAML behavior and current CLI usage remain unchanged by default.
* New behavior is opt-in via dialect flags or dialect auto-detection on new inputs.

## FEATURE

* issue-20: V1.1.0
* issue-17: Extend Markdown renderer with dialect-aware sections
* issue-16: Implement Azure DevOps pipelines dialect
* issue-15: Implement GitHub Actions dialect
* issue-14: Wire CLI dialect selection through Yaml2DocRegistry
* issue-13: Add dialect selection flags to CLI

## DOCUMENTATION

* issue-19: Document dialect usage and CLI flags in README

## TESTS

* issue-18: Add regression tests to ensure standard YAML behavior is unchanged

