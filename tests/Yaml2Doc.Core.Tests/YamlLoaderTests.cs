using System.Collections.Generic;
using Xunit;
using Yaml2Doc.Core.Models;
using Yaml2Doc.Core.Parsing;

namespace Yaml2Doc.Core.Tests.Parsing
{
    public class YamlLoaderTests
    {
        [Fact]
        public void Load_SimpleYaml_ReturnsExpectedPipelineDocument()
        {
            var yaml = @"
name: sample-pipeline
trigger: push
jobs:
  build:
    runs-on: ubuntu-latest
    steps:
      - name: Checkout
        uses: actions/checkout@v4
";

            var context = YamlDocumentContext.FromString(yaml);
            var loader = new YamlLoader();

            PipelineDocument doc = loader.Load(context);

            Assert.NotNull(doc);
            Assert.Equal("sample-pipeline", doc.Name);

            // Root keys are case-insensitive
            Assert.Equal("push", doc["trigger"]);
            Assert.Equal("push", doc["TRIGGER"]);

            Assert.True(doc["jobs"] is IDictionary<string, object?>);
            var jobs = (IDictionary<string, object?>)doc["jobs"]!;

            Assert.True(jobs["build"] is IDictionary<string, object?>);
            var build = (IDictionary<string, object?>)jobs["build"]!;

            Assert.Equal("ubuntu-latest", build["runs-on"]);

            Assert.True(build["steps"] is IList<object?>);
            var steps = (IList<object?>)build["steps"]!;
            Assert.Single(steps);

            Assert.True(steps[0] is IDictionary<string, object?>);
            var firstStep = (IDictionary<string, object?>)steps[0]!;

            Assert.Equal("Checkout", firstStep["name"]);
            Assert.Equal("actions/checkout@v4", firstStep["uses"]);
        }

        [Fact]
        public void Load_EmptyYaml_Throws()
        {
            var ex = Assert.Throws<YamlLoadException>(
                () => YamlDocumentContext.FromString(string.Empty));

            Assert.Contains("empty", ex.Message.ToLowerInvariant());
        }

        [Fact]
        public void Load_MalformedYaml_Throws()
        {
            var malformed = "name: foo: bar"; // invalid YAML

            var ex = Assert.Throws<YamlLoadException>(
                () => YamlDocumentContext.FromString(malformed));

            Assert.Contains("parse", ex.Message.ToLowerInvariant());
        }

        [Fact]
        public void Load_NonMappingRoot_Throws()
        {
            var yaml = "- a\n- b\n- c\n"; // root is a sequence, not a mapping

            var ex = Assert.Throws<YamlLoadException>(
                () => YamlDocumentContext.FromString(yaml));

            Assert.Contains("mapping", ex.Message.ToLowerInvariant());
        }
    }
}
