using Yaml2Doc.Core.Models;
using Yaml2Doc.Markdown;
using Xunit;

namespace Yaml2Doc.Core.Tests
{
    public class BasicMarkdownRendererTests
    {
        [Fact]
        public void Render_IncludesTitleAndRootKeysSectionAndBullets()
        {
            // Arrange
            var document = new PipelineDocument
            {
                Name = "Sample Pipeline"
            };

            document.Root["name"] = "sample-pipeline";
            document.Root["steps"] = new object();
            document.Root["variables"] = new object();

            var renderer = new BasicMarkdownRenderer();

            // Act
            var markdown = renderer.Render(document);

            // Assert
            Assert.Contains("# Sample Pipeline", markdown);
            Assert.Contains("## Root Keys", markdown);
            Assert.Contains("- name", markdown);
            Assert.Contains("- steps", markdown);
            Assert.Contains("- variables", markdown);
        }

        [Fact]
        public void Render_UsesFallbackTitleWhenNameIsMissing()
        {
            // Arrange
            var document = new PipelineDocument();
            document.Root["foo"] = "bar";

            var renderer = new BasicMarkdownRenderer();

            // Act
            var markdown = renderer.Render(document);

            // Assert
            Assert.Contains("# YAML Document", markdown);
            Assert.Contains("- foo", markdown);
        }

        [Fact]
        public void Render_WritesNoRootKeysMessageWhenEmpty()
        {
            // Arrange
            var document = new PipelineDocument
            {
                Name = "Empty Root"
            };

            var renderer = new BasicMarkdownRenderer();

            // Act
            var markdown = renderer.Render(document);

            // Assert
            Assert.Contains("# Empty Root", markdown);
            Assert.Contains("## Root Keys", markdown);
            Assert.Contains("_(no root keys)_", markdown);
        }
    }

    public class BasicMarkdownRendererDialectTests
    {
        [Fact]
        public void Render_WithGitHubActionsDialect_AddsTriggersAndJobsSections()
        {
            // Arrange
            var document = new PipelineDocument
            {
                Name = "GitHub Actions Golden",
                DialectId = "gha"
            };

            // Shape the Root similar to what the loader would produce for a GHA workflow.
            var jobs = new Dictionary<string, object?>
            {
                ["build"] = new Dictionary<string, object?>
                {
                    ["name"] = "Build",
                    ["runs-on"] = "ubuntu-latest",
                    ["steps"] = new List<object?>
                    {
                        new Dictionary<string, object?>
                        {
                            ["name"] = "Checkout",
                            ["uses"] = "actions/checkout@v4"
                        },
                        new Dictionary<string, object?>
                        {
                            ["name"] = "Run tests",
                            ["run"] = "dotnet test"
                        }
                    }
                }
            };

            document.Root["name"] = "CI"; // root-level name key
            document.Root["on"] = new List<object?> { "push", "pull_request" };
            document.Root["jobs"] = jobs;

            var renderer = new BasicMarkdownRenderer();

            // Act
            var markdown = renderer.Render(document);

            // Assert: baseline header + Root Keys section are still there
            Assert.Contains("# GitHub Actions Golden", markdown);
            Assert.Contains("## Root Keys", markdown);
            Assert.Contains("- name", markdown);
            Assert.Contains("- on", markdown);
            Assert.Contains("- jobs", markdown);

            // Assert: dialect-specific sections
            Assert.Contains("## Triggers", markdown);
            Assert.Contains("- push", markdown);
            Assert.Contains("- pull_request", markdown);

            Assert.Contains("## Jobs", markdown);
            Assert.Contains("### build", markdown);
            Assert.Contains("**Name:** Build", markdown);
            Assert.Contains("**Runs on:** ubuntu-latest", markdown);
            Assert.Contains("**Steps:**", markdown);
            Assert.Contains("Checkout", markdown);
            Assert.Contains("Run tests", markdown);
        }

        [Fact]
        public void Render_WithAzurePipelinesDialect_AddsTriggerAndStagesSections()
        {
            // Arrange
            var document = new PipelineDocument
            {
                Name = "Azure Pipelines Golden",
                DialectId = "ado"
            };

            // trigger: [ main ]
            document.Root["name"] = "CI";
            document.Root["trigger"] = new List<object?> { "main" };

            // stages:
            //   - stage: Build
            //     jobs:
            //       - job: BuildJob
            var buildStage = new Dictionary<string, object?>
            {
                ["stage"] = "Build",
                ["jobs"] = new List<object?>
                {
                    new Dictionary<string, object?>
                    {
                        ["job"] = "BuildJob"
                    }
                }
            };

            document.Root["stages"] = new List<object?> { buildStage };

            var renderer = new BasicMarkdownRenderer();

            // Act
            var markdown = renderer.Render(document);

            // Assert: baseline
            Assert.Contains("# Azure Pipelines Golden", markdown);
            Assert.Contains("## Root Keys", markdown);
            Assert.Contains("- name", markdown);
            Assert.Contains("- trigger", markdown);
            Assert.Contains("- stages", markdown);

            // Assert: dialect-specific
            Assert.Contains("## Trigger", markdown);
            Assert.Contains("Branch: main", markdown);

            Assert.Contains("## Stages", markdown);
            Assert.Contains("### Build", markdown);
            Assert.Contains("**Jobs:**", markdown);
            Assert.Contains("- BuildJob", markdown);
        }
    }
}
