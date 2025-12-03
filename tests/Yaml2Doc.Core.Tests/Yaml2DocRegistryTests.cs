using System;
using Yaml2Doc.Core.Dialects;
using Yaml2Doc.Core.Parsing;
using Xunit;

namespace Yaml2Doc.Core.Tests.Dialects
{
    public class Yaml2DocRegistryTests
    {
        private static YamlDocumentContext CreateContext(string yaml)
        {
            return YamlDocumentContext.FromString(yaml);
        }

        [Fact]
        public void ResolveDialect_WithForcedStandardId_ReturnsStandardDialect()
        {
            // Arrange
            var yaml = "name: test\nfoo: bar";
            var context = CreateContext(yaml);
            var loader = new YamlLoader();
            var standard = new StandardYamlDialect(loader);
            var registry = new Yaml2DocRegistry(new[] { standard });

            // Act
            var dialect = registry.ResolveDialect(context, forcedId: "standard");

            // Assert
            Assert.Same(standard, dialect);
        }

        [Fact]
        public void ResolveDialect_WithNullForcedId_DefaultsToStandardDialect()
        {
            // Arrange
            var yaml = "name: test\nfoo: bar";
            var context = CreateContext(yaml);
            var loader = new YamlLoader();
            var standard = new StandardYamlDialect(loader);
            var registry = new Yaml2DocRegistry(new[] { standard });

            // Act
            var dialect = registry.ResolveDialect(context, forcedId: null);

            // Assert
            Assert.Same(standard, dialect);
        }

        [Fact]
        public void ResolveDialect_WithNonexistentForcedId_ThrowsInvalidOperationException()
        {
            // Arrange
            var yaml = "name: test\nfoo: bar";
            var context = CreateContext(yaml);
            var loader = new YamlLoader();
            var standard = new StandardYamlDialect(loader);
            var registry = new Yaml2DocRegistry(new[] { standard });

            // Act
            var ex = Assert.Throws<InvalidOperationException>(
                () => registry.ResolveDialect(context, forcedId: "nonexistent"));

            // Assert
            Assert.Contains("No dialect with id 'nonexistent' is registered.", ex.Message);
        }
    }
}
