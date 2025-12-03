using System;
using System.IO;
using Xunit;
using Yaml2Doc.Core.Parsing;
using Yaml2Doc.Markdown;

namespace Yaml2Doc.Core.Tests.Golden
{
    public sealed class StandardGoldenIntegrationTests
    {
        [Fact]
        public void StandardGoldenYaml_RendersExpectedMarkdown()
        {
            // NOTE: this assumes the golden files are copied to the test output under a "golden" folder.
            var baseDir = AppContext.BaseDirectory;
            var yamlPath = Path.Combine(baseDir, "golden", "standard-golden.yml");
            var mdPath = Path.Combine(baseDir, "golden", "standard-golden.md");

            Assert.True(File.Exists(yamlPath), $"Golden YAML not found at: {yamlPath}");
            Assert.True(File.Exists(mdPath), $"Golden Markdown not found at: {mdPath}");

            var yamlText = File.ReadAllText(yamlPath);
            var expectedMarkdown = File.ReadAllText(mdPath);

            // Load YAML → PipelineDocument
            var context = YamlDocumentContext.FromString(yamlText);
            var loader = new YamlLoader();
            var document = loader.Load(context);

            // Render Markdown
            var renderer = new BasicMarkdownRenderer();
            var actualMarkdown = renderer.Render(document);

            // Normalize line endings and trailing whitespace before comparing
            Assert.Equal(
                Normalize(expectedMarkdown),
                Normalize(actualMarkdown)
            );
        }

        private static string Normalize(string value) =>
            value.Replace("\r\n", "\n").Trim();
    }
}
