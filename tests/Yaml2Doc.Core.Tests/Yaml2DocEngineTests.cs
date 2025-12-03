using System.Collections.Generic;
using Xunit;
using Yaml2Doc.Core;
using Yaml2Doc.Core.Dialects;
using Yaml2Doc.Core.Engine;
using Yaml2Doc.Core.Models;
using Yaml2Doc.Core.Parsing;
using Yaml2Doc.Markdown;

namespace Yaml2Doc.Core.Tests
{
    public sealed class Yaml2DocEngineTests
    {
        private static Yaml2DocEngine CreateEngine()
        {
            var loader = new YamlLoader();
            var standardDialect = new StandardYamlDialect(loader);
            var registry = new Yaml2DocRegistry(new[] { standardDialect });

            var markdownRenderer = new BasicMarkdownRenderer();

            return new Yaml2DocEngine(registry, markdownRenderer);
        }

        [Fact]
        public void Convert_WithMalformedYaml_ThrowsYaml2DocParseException_WithClearMessage()
        {
            // missing closing bracket / invalid sequence
            const string invalidYaml = "root:\n  list: [1, 2, 3\n";

            var engine = CreateEngine();

            var ex = Assert.Throws<Yaml2DocParseException>(() => engine.Convert(invalidYaml));

            Assert.Contains("Failed to parse YAML", ex.Message, StringComparison.OrdinalIgnoreCase);
        }

        [Fact]
        public void Convert_GivenSimpleYaml_ReturnsMarkdownWithRootKeys()
        {
            // Arrange
            const string yaml = """
                name: Sample Pipeline
                steps:
                  - script: echo "Hello"
                """;

            var loader = new YamlLoader();
            var dialects = new List<IYamlDialect>
            {
                new StandardYamlDialect(loader)
            };

            var registry = new Yaml2DocRegistry(dialects);
            var renderer = new BasicMarkdownRenderer();

            var engine = new Yaml2DocEngine(registry, renderer);

            // Act
            var markdown = engine.Convert(yaml);

            // Assert
            Assert.False(string.IsNullOrWhiteSpace(markdown));
            Assert.Contains("# Sample Pipeline", markdown);
            Assert.Contains("## Root Keys", markdown);
            Assert.Contains("steps", markdown);
        }
    }
}
