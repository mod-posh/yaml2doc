# MarkdownRenderMode

Controls the verbosity and structure of Markdown rendering.

**Remarks**

[Basic](MarkdownRenderMode.md#yaml2doc.markdown.markdownrendermode.basic) mode provides the stable v1 contract: a title and a root keys overview, plus minimal dialect-aware sections. This mode should remain stable across minor releases, except for bug fixes.

[Rich](MarkdownRenderMode.md#yaml2doc.markdown.markdownrendermode.rich) mode may include additional sections and structure, and is allowed to evolve faster without strict golden-test guarantees.

<a id="yaml2doc.markdown.markdownrendermode.basic"></a>
## Field: Basic
Stable baseline output that reflects the current v1 behavior.

<a id="yaml2doc.markdown.markdownrendermode.rich"></a>
## Field: Rich
Opt-in richer output with additional sections and structure.

