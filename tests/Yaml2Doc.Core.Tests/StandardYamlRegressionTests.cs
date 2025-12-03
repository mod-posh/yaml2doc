using System;
using System.IO;
using Xunit;
using Yaml2Doc.Core.Dialects;
using Yaml2Doc.Core.Engine;
using Yaml2Doc.Core.Parsing;
using Yaml2Doc.Markdown;

namespace Yaml2Doc.Core.Tests
{
    public class StandardYamlRegressionTests
    {
        [Fact]
        public void StandardDialect_StandardGoldenYaml_RendersExpectedMarkdown()
        {
            // Arrange: locate the linked golden files under bin/.../golden
            var outputDir = AppContext.BaseDirectory;
            var goldenDir = Path.Combine(outputDir, "golden");

            var inputPath = Path.Combine(goldenDir, "standard-golden.yml");
            var expectedMarkdownPath = Path.Combine(goldenDir, "standard-golden.md");

            Assert.True(File.Exists(inputPath), $"Missing input fixture: {inputPath}");
            Assert.True(File.Exists(expectedMarkdownPath), $"Missing golden markdown: {expectedMarkdownPath}");

            var yamlText = File.ReadAllText(inputPath);

            // Build the engine pieces: context → dialect → document → markdown
            var context = YamlDocumentContext.FromString(yamlText);

            var loader = new YamlLoader();
            var standardDialect = new StandardYamlDialect(loader);
            var registry = new Yaml2DocRegistry(new[] { standardDialect });

            var dialect = registry.ResolveDialect(context);
            Assert.Same(standardDialect, dialect); // sanity check: standard wins

            var document = dialect.Parse(context);

            var renderer = new BasicMarkdownRenderer();
            var actualMarkdown = renderer.Render(document);

            var expectedMarkdown = File.ReadAllText(expectedMarkdownPath);

            // Normalize line endings so the comparison is effectively byte-for-byte
            string Normalize(string s) => s.Replace("\r\n", "\n").Replace("\r", "\n");

            // Assert
            Assert.Equal(
                Normalize(expectedMarkdown),
                Normalize(actualMarkdown)
            );
        }
    }
}
