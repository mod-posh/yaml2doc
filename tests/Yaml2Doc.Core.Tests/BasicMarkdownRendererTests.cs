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
}
