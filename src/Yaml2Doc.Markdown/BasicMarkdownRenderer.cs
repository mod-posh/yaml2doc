using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Yaml2Doc.Core.Models;
using Yaml2Doc.Core.Interfaces;

namespace Yaml2Doc.Markdown
{
    /// <summary>
    /// Baseline Markdown renderer that produces a simple overview of a <see cref="PipelineDocument"/>.
    /// </summary>
    /// <remarks>
    /// Renders:
    /// - H1 as the document name if present; otherwise, the title is &quot;YAML Document&quot;.
    /// - A &quot;Root Keys&quot; section listing top-level keys.
    /// For GitHub Actions (<c>gha</c>) and Azure Pipelines (<c>ado</c>) dialects, additional best-effort sections
    /// are appended while preserving the original baseline output.
    /// Output is deterministic for a given document and the input is not mutated.
    /// </remarks>
    public sealed class BasicMarkdownRenderer : IMarkdownRenderer
    {
        /// <summary>
        /// Converts the provided <paramref name="document"/> into a basic Markdown representation.
        /// </summary>
        /// <param name="document">
        /// The <see cref="PipelineDocument"/> to render. Must not be <see langword="null"/>.
        /// </param>
        /// <returns>
        /// A Markdown string containing the document title and a bullet list of root keys.
        /// If there are no root keys, the list is replaced with <c>_(no root keys)_</c>.
        /// For known dialects (<c>gha</c> or <c>ado</c>), additional sections are appended.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// Thrown when <paramref name="document"/> is <see langword="null"/>.
        /// </exception>
        public string Render(PipelineDocument document)
        {
            if (document is null)
            {
                throw new ArgumentNullException(nameof(document));
            }

            var dialectId = document.DialectId?.ToLowerInvariant();

            var isGitHubActions = string.Equals(dialectId, "gha", StringComparison.Ordinal);
            var isAzurePipelines = string.Equals(dialectId, "ado", StringComparison.Ordinal);

            // For non-dialect or standard docs, preserve the original behaviour exactly.
            if (!isGitHubActions && !isAzurePipelines)
            {
                return RenderBaseline(document);
            }

            // For known dialects, start from the baseline output and then append
            // dialect-aware sections.
            var sb = new StringBuilder();
            RenderBaselineInto(document, sb);

            // If there were no root keys, RenderBaselineInto has already written
            // the "no root keys" marker and returned, so we can still append sections.
            sb.AppendLine(); // ensure a blank line before dialect sections

            if (isGitHubActions)
            {
                AppendGitHubActionsSections(document, sb);
            }
            else if (isAzurePipelines)
            {
                AppendAzurePipelinesSections(document, sb);
            }

            return sb.ToString();
        }

        /// <summary>
        /// Baseline representation: title and &quot;Root Keys&quot; section (used for standard/unknown dialects).
        /// </summary>
        /// <param name="document">The document to render.</param>
        /// <returns>Baseline Markdown without dialect-specific sections.</returns>
        private static string RenderBaseline(PipelineDocument document)
        {
            var sb = new StringBuilder();

            // Title
            var title = string.IsNullOrWhiteSpace(document.Name)
                ? "YAML Document"
                : document.Name;

            sb.Append("# ")
              .AppendLine(title)
              .AppendLine();

            // Root keys section
            sb.AppendLine("## Root Keys")
              .AppendLine();

            if (document.Root is null || document.Root.Count == 0)
            {
                sb.AppendLine("_(no root keys)_");
                return sb.ToString();
            }

            // Use the dictionary's keys; order is whatever the caller inserted
            foreach (var key in document.Root.Keys)
            {
                sb.Append("- ")
                  .AppendLine(key);
            }

            return sb.ToString();
        }

        /// <summary>
        /// Writes the baseline representation into an existing <see cref="StringBuilder"/>.
        /// Must preserve identical textual output as <see cref="RenderBaseline"/>.
        /// </summary>
        /// <param name="document">The document to render.</param>
        /// <param name="sb">Destination buffer.</param>
        private static void RenderBaselineInto(PipelineDocument document, StringBuilder sb)
        {
            // Title
            var title = string.IsNullOrWhiteSpace(document.Name)
                ? "YAML Document"
                : document.Name;

            sb.Append("# ")
              .AppendLine(title)
              .AppendLine();

            // Root keys section
            sb.AppendLine("## Root Keys")
              .AppendLine();

            if (document.Root is null || document.Root.Count == 0)
            {
                sb.AppendLine("_(no root keys)_");
                return;
            }

            foreach (var key in document.Root.Keys)
            {
                sb.Append("- ")
                  .AppendLine(key);
            }
        }

        /// <summary>
        /// Appends GitHub Actions-specific sections (Triggers, Jobs) to the buffer.
        /// </summary>
        /// <param name="document">The document assumed to represent a GitHub Actions workflow.</param>
        /// <param name="sb">Destination buffer.</param>
        private static void AppendGitHubActionsSections(PipelineDocument document, StringBuilder sb)
        {
            // Triggers from "on"
            if (document.Root.TryGetValue("on", out var onValue))
            {
                sb.AppendLine("## Triggers")
                  .AppendLine();

                RenderGitHubActionsTriggers(onValue, sb);
                sb.AppendLine();
            }

            // Jobs from "jobs"
            if (document.Root.TryGetValue("jobs", out var jobsValue) &&
                jobsValue is IDictionary<string, object?> jobsMap &&
                jobsMap.Count > 0)
            {
                sb.AppendLine("## Jobs")
                  .AppendLine();

                foreach (var kvp in jobsMap)
                {
                    var jobId = kvp.Key;
                    sb.AppendLine($"### {jobId}")
                      .AppendLine();

                    if (kvp.Value is IDictionary<string, object?> jobMap)
                    {
                        if (jobMap.TryGetValue("name", out var jobNameObj) &&
                            jobNameObj is string jobName &&
                            !string.IsNullOrWhiteSpace(jobName))
                        {
                            sb.AppendLine($"**Name:** {jobName}")
                              .AppendLine();
                        }

                        if (jobMap.TryGetValue("runs-on", out var runsOnObj) &&
                            runsOnObj is string runsOn &&
                            !string.IsNullOrWhiteSpace(runsOn))
                        {
                            sb.AppendLine($"**Runs on:** {runsOn}")
                              .AppendLine();
                        }

                        if (jobMap.TryGetValue("steps", out var stepsObj) &&
                            stepsObj is IList<object?> stepsList &&
                            stepsList.Count > 0)
                        {
                            sb.AppendLine("**Steps:**")
                              .AppendLine();

                            foreach (var stepObj in stepsList)
                            {
                                if (stepObj is IDictionary<string, object?> stepMap)
                                {
                                    stepMap.TryGetValue("name", out var stepNameObj);
                                    stepMap.TryGetValue("uses", out var usesObj);
                                    stepMap.TryGetValue("run", out var runObj);

                                    var label =
                                        stepNameObj as string ??
                                        usesObj as string ??
                                        (runObj is string ? "(run step)" : null) ??
                                        "(unnamed step)";

                                    sb.Append("- ")
                                      .AppendLine(label);
                                }
                            }

                            sb.AppendLine();
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Renders GitHub Actions trigger configuration from the value of the <c>on</c> key.
        /// </summary>
        /// <param name="onValue">The typed value of the <c>on</c> key.</param>
        /// <param name="sb">Destination buffer.</param>
        private static void RenderGitHubActionsTriggers(object? onValue, StringBuilder sb)
        {
            switch (onValue)
            {
                case string singleEvent:
                    sb.AppendLine($"- {singleEvent}");
                    break;

                case IList<object?> list:
                    foreach (var item in list.OfType<string>())
                    {
                        sb.AppendLine($"- {item}");
                    }
                    break;

                case IDictionary<string, object?> map:
                    foreach (var kvp in map)
                    {
                        sb.AppendLine($"- {kvp.Key}");
                    }
                    break;

                case bool b when b:
                    sb.AppendLine("- workflow_dispatch");
                    break;

                default:
                    sb.AppendLine("- (unknown trigger shape)");
                    break;
            }
        }

        /// <summary>
        /// Appends Azure Pipelines-specific sections (Trigger, Stages/Jobs) to the buffer.
        /// </summary>
        /// <param name="document">The document assumed to represent an Azure DevOps pipeline.</param>
        /// <param name="sb">Destination buffer.</param>
        private static void AppendAzurePipelinesSections(PipelineDocument document, StringBuilder sb)
        {
            // Trigger
            if (document.Root.TryGetValue("trigger", out var triggerValue))
            {
                sb.AppendLine("## Trigger")
                  .AppendLine();

                RenderAzurePipelinesTrigger(triggerValue, sb);
                sb.AppendLine();
            }

            // Stages
            if (document.Root.TryGetValue("stages", out var stagesValue) &&
                stagesValue is IList<object?> stagesList &&
                stagesList.Count > 0)
            {
                sb.AppendLine("## Stages")
                  .AppendLine();

                foreach (var stageObj in stagesList)
                {
                    if (stageObj is IDictionary<string, object?> stageMap)
                    {
                        var stageName = stageMap.TryGetValue("stage", out var stageNameObj)
                            ? stageNameObj as string
                            : null;

                        if (string.IsNullOrWhiteSpace(stageName))
                        {
                            stageName = "(unnamed stage)";
                        }

                        sb.AppendLine($"### {stageName}")
                          .AppendLine();

                        if (stageMap.TryGetValue("jobs", out var jobsObj) &&
                            jobsObj is IList<object?> jobsList &&
                            jobsList.Count > 0)
                        {
                            sb.AppendLine("**Jobs:**")
                              .AppendLine();

                            foreach (var jobObj in jobsList)
                            {
                                if (jobObj is IDictionary<string, object?> jobMap)
                                {
                                    var jobName = jobMap.TryGetValue("job", out var jobNameObj)
                                        ? jobNameObj as string
                                        : null;

                                    if (string.IsNullOrWhiteSpace(jobName))
                                    {
                                        jobName = "(unnamed job)";
                                    }

                                    sb.Append("- ")
                                      .AppendLine(jobName);
                                }
                            }

                            sb.AppendLine();
                        }
                    }
                }
            }
            // Fallback: top-level jobs (single-stage pipelines)
            else if (document.Root.TryGetValue("jobs", out var jobsValue) &&
                     jobsValue is IList<object?> jobsList &&
                     jobsList.Count > 0)
            {
                sb.AppendLine("## Jobs")
                  .AppendLine();

                foreach (var jobObj in jobsList)
                {
                    if (jobObj is IDictionary<string, object?> jobMap)
                    {
                        var jobName = jobMap.TryGetValue("job", out var jobNameObj)
                            ? jobNameObj as string
                            : null;

                        if (string.IsNullOrWhiteSpace(jobName))
                        {
                            jobName = "(unnamed job)";
                        }

                        sb.AppendLine($"- {jobName}");
                    }
                }

                sb.AppendLine();
            }
        }

        /// <summary>
        /// Renders Azure Pipelines trigger configuration from the value of the <c>trigger</c> key.
        /// </summary>
        /// <param name="triggerValue">The typed value of the <c>trigger</c> key.</param>
        /// <param name="sb">Destination buffer.</param>
        private static void RenderAzurePipelinesTrigger(object? triggerValue, StringBuilder sb)
        {
            switch (triggerValue)
            {
                case string branch:
                    sb.AppendLine($"- Branch: {branch}");
                    break;

                case IList<object?> list:
                    foreach (var item in list.OfType<string>())
                    {
                        sb.AppendLine($"- Branch: {item}");
                    }
                    break;

                case IDictionary<string, object?> map:
                    foreach (var kvp in map)
                    {
                        sb.AppendLine($"- {kvp.Key}");
                    }
                    break;

                case bool b when !b:
                    sb.AppendLine("- disabled");
                    break;

                default:
                    sb.AppendLine("- (unknown trigger shape)");
                    break;
            }
        }
    }
}
