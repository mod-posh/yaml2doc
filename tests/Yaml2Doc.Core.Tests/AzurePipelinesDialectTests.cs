using System.Linq;
using Xunit;
using Yaml2Doc.Core.Dialects;
using Yaml2Doc.Core.Parsing;

namespace Yaml2Doc.Core.Tests.Dialects
{
    public sealed class AzurePipelinesDialectTests
    {
        [Fact]
        public void CanHandle_MinimalAzurePipeline_ReturnsTrue()
        {
            // Arrange
            const string yaml = @"
trigger:
- main

pool:
  vmImage: ubuntu-latest

steps:
- script: echo Hello
  displayName: 'Run a one-line script'
";

            var context = YamlDocumentContext.FromString(yaml);
            var loader = new YamlLoader();
            var dialect = new AzurePipelinesDialect(loader);

            // Act
            var canHandle = dialect.CanHandle(context);

            // Assert
            Assert.True(canHandle);
        }

        [Fact]
        public void CanHandle_NonAzureYaml_ReturnsFalse()
        {
            // Arrange
            const string yaml = @"
name: JustSomeYaml
foo: bar
items:
  - one
  - two
";

            var context = YamlDocumentContext.FromString(yaml);
            var loader = new YamlLoader();
            var dialect = new AzurePipelinesDialect(loader);

            // Act
            var canHandle = dialect.CanHandle(context);

            // Assert
            Assert.False(canHandle);
        }

        [Fact]
        public void Parse_MinimalAzurePipeline_PopulatesRootAndName()
        {
            // Arrange
            const string yaml = @"
name: CI
trigger:
- main

pool:
  vmImage: ubuntu-latest

steps:
- script: echo Hello
  displayName: 'Run a one-line script'
";

            var context = YamlDocumentContext.FromString(yaml);
            var loader = new YamlLoader();
            var dialect = new AzurePipelinesDialect(loader);

            // Act
            var document = dialect.Parse(context);

            // Assert
            Assert.Equal("CI", document.Name);
            Assert.NotNull(document.Root);

            // We expect ADO concepts to be preserved as root keys
            Assert.Contains("trigger", document.Root.Keys);
            Assert.Contains("pool", document.Root.Keys);
            Assert.Contains("steps", document.Root.Keys);
        }

        [Fact]
        public void Registry_ResolveDialect_WithForcedAdo_ReturnsAzurePipelinesDialect()
        {
            // Arrange
            const string yaml = "trigger: [ main ]";
            var context = YamlDocumentContext.FromString(yaml);
            var loader = new YamlLoader();

            var github = new GitHubActionsDialect(loader);
            var azure = new AzurePipelinesDialect(loader);
            var standard = new StandardYamlDialect(loader);

            var registry = new Yaml2DocRegistry(new IYamlDialect[]
            {
                github,
                azure,
                standard
            });

            // Act
            var dialect = registry.ResolveDialect(context, forcedId: "ado");

            // Assert
            var adoDialect = Assert.IsType<AzurePipelinesDialect>(dialect);
            Assert.Equal("ado", adoDialect.Id);
        }
    }
}
