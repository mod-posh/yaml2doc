namespace Yaml2Doc.Markdown
{
    /// <summary>
    /// Controls the verbosity and structure of Markdown rendering.
    /// </summary>
    /// <remarks>
    /// <para>
    /// <see cref="Basic"/> mode provides the stable v1 contract: a title and a root keys overview,
    /// plus minimal dialect-aware sections. This mode should remain stable across minor releases,
    /// except for bug fixes.
    /// </para>
    /// <para>
    /// <see cref="Rich"/> mode may include additional sections and structure, and is allowed to evolve
    /// faster without strict golden-test guarantees.
    /// </para>
    /// </remarks>
    public enum MarkdownRenderMode
    {
        /// <summary>
        /// Stable baseline output that reflects the current v1 behavior.
        /// </summary>
        Basic = 0,

        /// <summary>
        /// Opt-in richer output with additional sections and structure.
        /// </summary>
        Rich = 1
    }
}
