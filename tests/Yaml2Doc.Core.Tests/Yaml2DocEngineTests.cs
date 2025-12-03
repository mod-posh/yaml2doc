using System.Collections.Generic;
using Xunit;
using Yaml2Doc.Core;
using Yaml2Doc.Core.Dialects;
using Yaml2Doc.Core.Engine;
using Yaml2Doc.Core.Parsing;
using Yaml2Doc.Markdown;

namespace Yaml2Doc.Core.Tests
{
    public sealed class Yaml2DocEngineTests
    {
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
