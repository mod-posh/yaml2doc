# v1.0.0

> Initial release of Yaml2Doc.
> Scope: given a standard YAML file (no CI-specific dialect assumptions), parse it and generate a useful Markdown summary on the command line.
>
> Goals:
>
> * CLI that accepts an input `.yml/.yaml` file and writes Markdown to stdout or a file.
> * Robust parsing of “vanilla” YAML into an internal document model.
> * A Markdown renderer that produces a readable document (not just a debug dump).
> * Golden YAML + golden Markdown tests for regression.
>
> Non-goals for this milestone:
>
> * GitHub Actions / Azure DevOps specific semantics.
> * Plugin-style dialect or DSL architecture beyond what’s needed to support “standard”.
> * Complex formatting or theming of Markdown.

## ENHANCEMENT

* issue-10: Handle errors gracefully in engine and CLI

## CHORE

* issue-12: Align CI pipelines, NuGet publishing, and shared build props
* issue-1: Create Yaml2Doc solution structure (.NET 9)

## FEATURE

* issue-7: Implement Yaml2Doc CLI entrypoint
* issue-6: Implement Yaml2Doc engine to convert YAML to Markdown
* issue-5: Implement baseline Markdown renderer for PipelineDocument
* issue-4: Implement Standard YAML dialect and registry
* issue-3: Implement Yaml loader to convert YAML text into PipelineDocument
* issue-2: Define neutral PipelineDocument model for loaded YAML

## TESTS, FEATURE

* issue-8: Add standard-golden YAML sample and expected Markdown

## TESTS

* issue-9: Add golden snapshot test for standard YAML → Markdown

## DOCUMENTATION

* issue-11: Document v1 usage, scope, and limitations in README

