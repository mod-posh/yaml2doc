using Yaml2Doc.Core.Dialects;
using Yaml2Doc.Core.Models;
using Yaml2Doc.Core.Parsing;
using Xunit;

namespace Yaml2Doc.Core.Tests;

public class StandardYamlDialectTests
{
    private static YamlDocumentContext CreateContextFromText(string yaml)
    {
        return YamlDocumentContext.FromString(yaml);
    }

    [Fact]
    public void ResolveDialect_ReturnsStandardDialect_AndParsesDocument()
    {
        // Arrange
        const string yaml = @"
name: simple-pipeline
steps:
  - run: echo ""hello""
";

        var context = CreateContextFromText(yaml);

        var loader = new YamlLoader();
        var standard = new StandardYamlDialect(loader);
        var registry = new Yaml2DocRegistry(new[] { standard });

        // Act: Resolve dialect
        var resolvedDialect = registry.ResolveDialect(context);

        // Assert: we got StandardYamlDialect
        var typedDialect = Assert.IsType<StandardYamlDialect>(resolvedDialect);
        Assert.Equal("standard", typedDialect.Id);

        // Act: Parse into PipelineDocument
        var doc = typedDialect.Parse(context);

        // Assert: PipelineDocument as expected
        Assert.NotNull(doc);
        Assert.Equal("simple-pipeline", doc.Name);
        Assert.NotNull(doc.Root);
        Assert.True(doc.Root.ContainsKey("steps"));
    }

    [Fact]
    public void CreateDefaultRegistry_ParsesViaRegistryParse()
    {
        // Arrange
        const string yaml = @"
name: another-pipeline
foo: bar
";

        var context = CreateContextFromText(yaml);
        var registry = Yaml2DocRegistry.CreateDefault();

        // Act
        PipelineDocument doc = registry.Parse(context);

        // Assert
        Assert.Equal("another-pipeline", doc.Name);
        Assert.True(doc.Root.ContainsKey("foo"));
        Assert.Equal("bar", doc.Root["foo"]);
    }
}
