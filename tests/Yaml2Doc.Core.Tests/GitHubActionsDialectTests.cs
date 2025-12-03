using System.Collections.Generic;
using Yaml2Doc.Core.Dialects;
using Yaml2Doc.Core.Models;
using Yaml2Doc.Core.Parsing;
using Xunit;

namespace Yaml2Doc.Core.Tests.Dialects
{
    public class GitHubActionsDialectTests
    {
        private const string SampleWorkflowYaml = @"
name: CI
on:
  push:
    branches: [ ""main"" ]
jobs:
  build:
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v4
      - name: Run tests
        run: dotnet test
";

        private static YamlDocumentContext CreateContext()
            => YamlDocumentContext.FromString(SampleWorkflowYaml);

        [Fact]
        public void CanHandle_ReturnsTrue_ForGithubActionsShape()
        {
            // Arrange
            var context = CreateContext();
            var loader = new YamlLoader();
            var dialect = new GitHubActionsDialect(loader);

            // Act
            var canHandle = dialect.CanHandle(context);

            // Assert
            Assert.True(canHandle);
        }

        [Fact]
        public void Parse_ProducesPipelineDocumentWithExpectedKeys()
        {
            // Arrange
            var context = CreateContext();
            var loader = new YamlLoader();
            var dialect = new GitHubActionsDialect(loader);

            // Act
            PipelineDocument doc = dialect.Parse(context);

            // Assert
            Assert.Equal("CI", doc.Name);

            Assert.True(doc.Root.ContainsKey("on"));
            Assert.True(doc.Root.ContainsKey("jobs"));

            var jobs = Assert.IsType<Dictionary<string, object?>>(doc.Root["jobs"]);
            Assert.True(jobs.ContainsKey("build"));

            var buildJob = Assert.IsType<Dictionary<string, object?>>(jobs["build"]);
            Assert.True(buildJob.ContainsKey("steps"));

            var steps = Assert.IsType<List<object?>>(buildJob["steps"]);
            Assert.NotEmpty(steps);
        }

        [Fact]
        public void ResolveDialect_WithForcedGhaId_ReturnsGitHubActionsDialect()
        {
            // Arrange
            var context = CreateContext();
            var loader = new YamlLoader();

            var registry = new Yaml2DocRegistry(new IYamlDialect[]
            {
                new GitHubActionsDialect(loader),
                new StandardYamlDialect(loader)
            });

            // Act
            var dialect = registry.ResolveDialect(context, forcedId: "gha");

            // Assert
            Assert.IsType<GitHubActionsDialect>(dialect);
        }
    }
}
